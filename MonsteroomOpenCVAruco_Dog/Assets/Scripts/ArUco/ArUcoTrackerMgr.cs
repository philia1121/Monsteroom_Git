using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
// using OpenCvSharp.Aruco;
using OpenCvSharp;
using System;
using System.Threading;

namespace Awespire.OpenCV{
    public class PoseData
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;

        private Vector3 lowPass_pos = Vector3.zero;
        private Quaternion lowPass_rot = Quaternion.identity;

        private int lostTrackCnt = 0;

        public bool IsLostTrack {
            get {
                return lostTrackCnt > 5;
            }
        }

        // public Vector3 pos_LPFilter {
        //     get {
        //         return prev_pos;
        //     }
        // }

        // public Quaternion rot_LPFilter{
        //     get{
        //         return prev_rot;
        //     }
        // }

        public PoseData(){
            position = Vector3.zero;
            rotation = Quaternion.identity;

            lowPass_pos = Vector3.zero;
            lowPass_rot = Quaternion.identity;
        }

        public void Set2LowPassData(){
            lowPass_pos = position;
            lowPass_rot = rotation;
        }

        public void Set2LowPassData(PoseData val){
            lowPass_pos = val.position;
            lowPass_rot = val.rotation;
        }

        Matrix4x4 Rodrigues(Vector3 rvec){
            Vector3 r = rvec.normalized;
            float theta = rvec.magnitude;
            float cost = Mathf.Cos(theta);
            float sint = Mathf.Sin(theta);
            float ocost = 1 - cost;

            return new Matrix4x4(
                new Vector4(cost + ocost * r.x * r.x             ,        ocost * r.x * r.y - sint * r.z,        ocost * r.x * r.z + sint * r.y, 0),
                new Vector4(       ocost * r.y * r.x + sint * r.z, cost + ocost * r.y * r.y             ,        ocost * r.y * r.z - sint * r.x, 0),
                new Vector4(       ocost * r.z * r.x - sint * r.y,        ocost * r.z * r.y + sint * r.x, cost + ocost * r.z * r.z             , 0),
                new Vector4(0, 0, 0, 1)
            );
        }

        public void LostTrack() {
            lostTrackCnt++;
        }

        public void Target2Pose(ref GameObject obj, float coefficients = 0, bool isWorld = true) {
            LowpassFilter(coefficients);

            if (isWorld) {
                obj.transform.position = lowPass_pos;
                obj.transform.rotation = lowPass_rot;
            } else {
                obj.transform.localPosition = lowPass_pos;
                obj.transform.localRotation = lowPass_rot;
            }
        }

        public void LowpassFilter(float coefficients){
            lowPass_pos = Vector3.Lerp(position, lowPass_pos, coefficients);
            lowPass_rot = Quaternion.Lerp(rotation, lowPass_rot, coefficients);
        }

        public void LowpassFilter(PoseData prevPose, float coefficients){
            lowPass_pos = Vector3.Lerp(position, prevPose.position, coefficients);
            lowPass_rot = Quaternion.Lerp(rotation, prevPose.rotation, coefficients);
        }

        // https://stackoverflow.com/questions/66447642/how-to-get-camera-position-from-opencv-solvepnp-for-unity
        public void ConvertRvecTvec2Unity(float[] rtInfo) {
            position.x = rtInfo[3];
            position.y = rtInfo[4];
            position.z = rtInfo[5];

            Matrix4x4 uR = Rodrigues(new Vector3(rtInfo[0], rtInfo[1], rtInfo[2]));
            rotation = uR.rotation;

            // To Unity Axis Rule
            position.y = -position.y;
            rotation.y = -rotation.y;
            // rotation.w = -rotation.w;

            lostTrackCnt = 0;
        }

        public void ConvertRvecTvec2Unity(float[] rtInfo, Transform worldRT) {
            ConvertRvecTvec2Unity(rtInfo);
            position = worldRT.TransformPoint(position);
            rotation = worldRT.rotation * rotation;

            lostTrackCnt = 0;
        }
    }

    public class ArUcoTrackerMgr : MonoBehaviour
    {
        private static ArUcoTrackerMgr _instance;
        public static ArUcoTrackerMgr instance {
            get {
                if (_instance == null)
                    _instance = GameObject.FindFirstObjectByType<ArUcoTrackerMgr>();
                return _instance;
            }
        }

        public OpenCvSharp.Aruco.PredefinedDictionaryName ArUcoDicID = OpenCvSharp.Aruco.PredefinedDictionaryName.Dict4X4_50;
        private OpenCvSharp.Aruco.Dictionary dictionary;

        [Space]
        public float markerWidth = 0.1f;    // meters
        [Range(0, 0.99f)]
        public float poseFilterCoef = 0.5f;
        [Range(1, 4)]
        public int divImgResolusion = 1;

        private Mat _processRgbMat;
        private Mat _oriCamMat;
        private Mat _halfSizeMat;
        // private Mat _camIntrinsicMatrix;
        private float[] _camMatrixElement; 
        private Mat _camDistortionCoef;

        // ArUco detection related mats and variables
        OpenCvSharp.Aruco.DetectorParameters detectorParameters;
        Point2f[][] corners;
        int[] ids;
        Point2f[][] rejectedImgPoints;

        [Space]
        private bool estimateAll = true;
        public GameObject sampleGameObject;
        private Dictionary<int, GameObject> sampleGMDic = new Dictionary<int, GameObject>();
        private Dictionary<int, PoseData> PoseDic = new Dictionary<int, PoseData>();

        // https://stackoverflow.com/questions/58925316/opencv-dll-calls-from-unity3d-lead-to-fps-drop
        [Space]
        public Transform virtualTransform4Thread = null;
        private ConcurrentStack<Color32[]> stack_camTex = new ConcurrentStack<Color32[]>();
        // private ConcurrentStack<Vector3> stack_camPos = new ConcurrentStack<Vector3>();
        // private ConcurrentStack<Quaternion> stack_camRot = new ConcurrentStack<Quaternion>();
        private ConcurrentStack<int> stack_texSeq = new ConcurrentStack<int>();
        // private ConcurrentQueue<Vector3> stack_camPos = new ConcurrentQueue<Vector3>();
        // private ConcurrentQueue<Quaternion> stack_camRot = new ConcurrentQueue<Quaternion>();
        private Thread thread;
        private Vector2 webcamDimm = Vector2.zero;
        // private Vector3 lastPos = Vector3.zero;
        // private Quaternion lastRot = Quaternion.identity;
        private bool threadProcessEndLoop = false;
        // [Space]
        // public int PredictDelayFramesNum = 7;
        // private int initialStackingFrames = 0;

        private bool _isReady = false;
        public bool IsReady{
            get{
                return _isReady;
            }
        }

        private int stackCnt = 0;
        private int lastStackCnt = 0;
        
        public void Initialize(int imageWidth, int imageHeight, float cx, float cy, float fx, float fy) {
            InitializeMatrices(imageWidth, imageHeight, cx, cy, fx, fy);
        }

        public void DetectAndEstimateMarker_Thread(WebCamTexture camTex) {
            webcamDimm.x = camTex.width;
            webcamDimm.y = camTex.height;

            // if( initialStackingFrames <= PredictDelayFramesNum ){
            //     initialStackingFrames++;
            // } else {
            //     stack_camTex.Push(camTex.GetPixels32());
            //     stack_camPos.TryDequeue(out var pos_q);
            //     stack_camRot.TryDequeue(out var rot_q);
            // }
            // stack_camPos.Enqueue(camTransform.position);
            // stack_camRot.Enqueue(camTransform.rotation);

            stack_texSeq.Push(stackCnt++);
            stack_camTex.Push(camTex.GetPixels32());
            // stack_camPos.Push(camTransform.position);
            // stack_camRot.Push(camTransform.rotation);
            // lastPos = camTransform.position;
            // lastRot = camTransform.rotation;
        }

        public Texture2D DetectAndEstimateMarker(WebCamTexture camTex, Transform camTransform, bool drawResult = false, Texture2D debugTexture = null){
            debugTexture = DetectMarker(camTex, drawResult, debugTexture);
            EstimateMarkerPose(camTransform);

            return debugTexture;
        }

        public Texture2D DetectMarker(WebCamTexture camTex, bool drawResult = false, Texture2D debugTexture = null){
            if( _isReady ){
                if( camTex == null ) return null;

                _oriCamMat = OpenCvSharp.Unity.TextureToMat(camTex);
                if( divImgResolusion != 1 ){
                    Cv2.Resize(_oriCamMat, _halfSizeMat, _halfSizeMat.Size());
                    Cv2.CvtColor(_halfSizeMat, _processRgbMat, ColorConversionCodes.BGR2GRAY);
                } else {
                    Cv2.CvtColor(_oriCamMat, _processRgbMat, ColorConversionCodes.BGR2GRAY);
                }

                // CvAruco.DetectMarkers(_processRgbMat, dictionary, out corners, out ids, detectorParameters, out rejectedImgPoints);
                using (InputArray image = _processRgbMat)
                using (var cornersVec = new VectorOfVectorPoint2f())
                using (var idsVec = new VectorOfInt32())
                using (var rejectedImgPointsVec = new VectorOfVectorPoint2f()){
                    NativeMethods.aruco_detectMarkers(
                        image.CvPtr, dictionary.ptrObj.CvPtr, cornersVec.CvPtr, idsVec.CvPtr,
                        detectorParameters.ptrObj.CvPtr, rejectedImgPointsVec.CvPtr);
                    
                    corners = cornersVec.ToArray();
                    ids     = idsVec.ToArray();
                    rejectedImgPoints = rejectedImgPointsVec.ToArray();
                }

                if( drawResult ){
                    if( corners != null ){
                        using (var cornersAddress = new OpenCvSharp.Util.ArrayAddress2<Point2f>(corners)){
                            if( ids == null ){
                                using (InputArray image = _oriCamMat){
                                    NativeMethods.aruco_drawDetectedMarkers(image.CvPtr, 
                                        cornersAddress.Pointer, cornersAddress.Dim1Length, cornersAddress.Dim2Lengths, 
                                        IntPtr.Zero, 0, new Scalar(0, 255, 0));
                                }
                            } else {
                                using (InputArray image = _oriCamMat){
                                    int[] idxArray = OpenCvSharp.Util.EnumerableEx.ToArray(ids);

                                    NativeMethods.aruco_drawDetectedMarkers(image.CvPtr, 
                                        cornersAddress.Pointer, cornersAddress.Dim1Length, cornersAddress.Dim2Lengths, 
                                        idxArray, idxArray.Length, new Scalar(0, 255, 0));
                                }
                            }
                        }
                    }
                    
                    // CvAruco.DrawDetectedMarkers(_oriCamMat, corners, ids);
                    return OpenCvSharp.Unity.MatToTexture (_oriCamMat, debugTexture);
                } else {
                    return null;
                }
            }

            Debug.Log("<color=yellow>[ArUco]</color> Not Yet Ready to Detect ArUco.");
            return null;
        }

        public void EstimateMarkerPose(Transform camTransform){
            if( !_isReady || ids.Length == 0 ){
                Debug.Log("<color=yellow>[ArUco]</color> Noting Detected, reject pose estimation.");
                return;
            }

            // Define 3D coordinates of marker corners (marker center is at origin)
            if (estimateAll) {
                float[][] rtsInfo = new float[ids.Length][];
                for (int i = 0; i < ids.Length; i++) {
                    rtsInfo[i] = new float[6] { 0, 0, 0, 0, 0, 0 };
                }

                using (InputArray cameraDistCoeffArr = _camDistortionCoef)
                using (var cornersAddress = new OpenCvSharp.Util.ArrayAddress2<Point2f>(corners))
                using (var rtsInfoAddress = new OpenCvSharp.Util.ArrayAddress2<float>(rtsInfo)) {
                    NativeMethods.aruco_estimatePoseSingleMarkers_all(
                        cornersAddress.Pointer, cornersAddress.Dim1Length, cornersAddress.Dim2Lengths, markerWidth,
                        _camMatrixElement, cameraDistCoeffArr.CvPtr,
                        rtsInfoAddress
                    );

                    foreach (KeyValuePair<int, PoseData> poses in PoseDic) {
                        poses.Value.LostTrack();
                    }

                    for (int i = 0; i < ids.Length; i++) {
                        if (!sampleGMDic.ContainsKey(ids[i])) {
                            GameObject newObj = Instantiate(sampleGameObject, Vector3.zero, Quaternion.identity);
                            // newObj.name = $"Marker_Test_______{ids[i].ToString("000")}__";
                            newObj.name = $"MARKER_{ArUcoDicID.ToString()}_ID_{ids[i].ToString("000")}";
                            // newObj.transform.SetParent(camTransform);
                            sampleGMDic.Add(ids[i], newObj);
                        }

                        if (PoseDic.ContainsKey(ids[i])) {
                            // PoseData savedData = PoseDic[ids[i]];

                            PoseDic[ids[i]].ConvertRvecTvec2Unity(rtsInfo[i], camTransform);
                            // PoseDic[ids[i]] = savedData;
                        } else {
                            PoseData curPose = new PoseData();

                            curPose.ConvertRvecTvec2Unity(rtsInfo[i], camTransform);
                            PoseDic.Add(ids[i], curPose);
                        }
                    }
                }
                
            } else {
                // Estimate Each Object
            }
        }

        void DetectMarkerInThread(){
            while (true) {
                try {
                    if (threadProcessEndLoop) continue;

                    if (stack_camTex.TryPop(out var webcamPixels32)) {
                        OpenCvSharp.Unity.TextureConversionParams parameters = new OpenCvSharp.Unity.TextureConversionParams();
                        _oriCamMat = OpenCvSharp.Unity.PixelsToMat(webcamPixels32, (int) webcamDimm.x, (int) webcamDimm.y, parameters.FlipVertically, parameters.FlipHorizontally, parameters.RotationAngle);
                        if( divImgResolusion != 1 ){
                            Cv2.Resize(_oriCamMat, _halfSizeMat, _halfSizeMat.Size());
                            Cv2.CvtColor(_halfSizeMat, _processRgbMat, ColorConversionCodes.BGR2GRAY);
                        } else {
                            Cv2.CvtColor(_oriCamMat, _processRgbMat, ColorConversionCodes.BGR2GRAY);
                        }

                        if( !stack_texSeq.TryPop(out lastStackCnt) ){
                            lastStackCnt = stackCnt;
                        }

                        using (InputArray image = _processRgbMat)
                        using (var cornersVec = new VectorOfVectorPoint2f())
                        using (var idsVec = new VectorOfInt32())
                        using (var rejectedImgPointsVec = new VectorOfVectorPoint2f()){
                            NativeMethods.aruco_detectMarkers(
                                image.CvPtr, dictionary.ptrObj.CvPtr, cornersVec.CvPtr, idsVec.CvPtr,
                                detectorParameters.ptrObj.CvPtr, rejectedImgPointsVec.CvPtr);
                            
                            corners = cornersVec.ToArray();
                            ids     = idsVec.ToArray();
                            rejectedImgPoints = rejectedImgPointsVec.ToArray();
                        }

                        stack_camTex.Clear();
                        threadProcessEndLoop = true;
                    }
                } catch (ThreadAbortException ex){
                    Debug.LogWarning(ex.Message);
                }
            }
        }

        void UpdateMarkersGameObject(){
            foreach (KeyValuePair<int, PoseData> pose in PoseDic) {
                var target = sampleGMDic[pose.Key];

                // pose.Value.Target2Pose(ref target, poseFilterCoef, false);
                pose.Value.Target2Pose(ref target, poseFilterCoef);
                target.SetActive(!pose.Value.IsLostTrack);
            }
        }

        public Transform MatchArUcoIDObj(int id){
            if( sampleGMDic.ContainsKey(id) ){
                return sampleGMDic[id].transform;
            }

            return null;
        }

        public Transform MatchArUcoIDObj(int id, out PoseData poseData){
            if (PoseDic.ContainsKey(id)) {
                poseData = PoseDic[id];
            } else {
                poseData = null;
            }

            if (sampleGMDic.ContainsKey(id)) {
                return sampleGMDic[id].transform;
            }

            return null;
        }

#region Unity Mono Functions
        private void Awake() {
            // Check Instance
            if (_instance == null) {
                _instance = this;
            }

            if (ArUcoTrackerMgr.instance.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) {
                DontDestroyOnLoad(this.gameObject);
            } else {
                Destroy(this.gameObject);
            }
        }

        void Start() {
            stack_camTex.Clear();
            if( thread != null ){
                thread.Abort();
            }

            thread = new Thread(DetectMarkerInThread);
            thread.Start();
        }

        void Update() {
            if( threadProcessEndLoop ){
                WebCamMgr.EstimateCameraPose(stackCnt - lastStackCnt, out var estPos, out var estRot);
                virtualTransform4Thread.position = estPos;
                virtualTransform4Thread.rotation = estRot;

                EstimateMarkerPose(virtualTransform4Thread);
                threadProcessEndLoop = false;
            }

            UpdateMarkersGameObject();
        }

        void OnDestroy() {
            ReleaseResources();

            if(thread != null){
                thread.Abort();
                thread = null;
            }
        }
#endregion

#region OpenCV Related Functions
        private void InitializeMatrices(int originalWidth, int originalHeight, float cX, float cY, float fX, float fY){
            // Processing dimensions (scaled by divide number)
            int processingWidth = originalWidth / divImgResolusion;
            int processingHeight = originalHeight / divImgResolusion;
            fX = fX / divImgResolusion;
            fY = fY / divImgResolusion;
            cX = cX / divImgResolusion;
            cY = cY / divImgResolusion;

            // Create camera intrinsic matrix
            // _camIntrinsicMatrix = new Mat(3, 3, MatType.CV_64FC1);
            // _camIntrinsicMatrix.Set(0, 0, fX);
            // _camIntrinsicMatrix.Set(0, 1, 0);
            // _camIntrinsicMatrix.Set(0, 2, cX);
            // _camIntrinsicMatrix.Set(1, 0, 0);
            // _camIntrinsicMatrix.Set(1, 1, fY);
            // _camIntrinsicMatrix.Set(1, 2, cY);
            // _camIntrinsicMatrix.Set(2, 0, 0);
            // _camIntrinsicMatrix.Set(2, 1, 0);
            // _camIntrinsicMatrix.Set(2, 2, 1.0f);
            // will be 3x3
            _camMatrixElement = new float[9]{
                fX, 0, cX,
                0, fY, cY,
                0, 0, 1
            };

            // No distortion coefficients for Quest cameras
            _camDistortionCoef = new Mat(4, 1, MatType.CV_64FC1);
            _camDistortionCoef.Set(0, 0, 0);
            _camDistortionCoef.Set(1, 0, 0);
            _camDistortionCoef.Set(2, 0, 0);
            _camDistortionCoef.Set(3, 0, 0);

            // Initialize all processing mats
            _oriCamMat = new Mat(originalHeight, originalWidth, MatType.CV_8UC4);
            _halfSizeMat = new Mat(processingHeight, processingWidth, MatType.CV_8UC4);
            _processRgbMat = new Mat(processingHeight, processingWidth, MatType.CV_8UC3);

            // // Create ArUco detection mats
            // _detectedMarkerIds = new Mat();
            // _detectedMarkerCorners = new List<Mat>();
            // _rejectedMarkerCandidates = new List<Mat>();
            // markerDictionary = Objdetect.getPredefinedDictionary((int)_dictionaryId);
            // recoveredMarkerIndices = new Mat();
            
            // Configure detector parameters for optimal performance
            detectorParameters = OpenCvSharp.Aruco.DetectorParameters.Create();
            detectorParameters.MinDistanceToBorder = 3;
            detectorParameters.ErrorCorrectionRate = 0.8f;

            // dictionary = CvAruco.GetPredefinedDictionary(ArUcoDicID);
            dictionary = new OpenCvSharp.Aruco.Dictionary(
                NativeMethods.aruco_getPredefinedDictionary((int) ArUcoDicID)
            );

            _isReady = true;
        }

        private void ReleaseResources(){
            if( _processRgbMat != null )    _processRgbMat.Dispose();
            if( _oriCamMat != null )        _oriCamMat.Dispose();
            if( _halfSizeMat != null )      _halfSizeMat.Dispose();
            
            if( detectorParameters != null ) 
                detectorParameters.Dispose();
        }
#endregion
    }
}
