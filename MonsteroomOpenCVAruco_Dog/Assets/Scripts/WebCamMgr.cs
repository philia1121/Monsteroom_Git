using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace Awespire
{
    public class WebCamMgr : MonoBehaviour {
        private static WebCamMgr _instance;
        public static WebCamMgr instance {
            get {
                if (_instance == null)
                    _instance = GameObject.FindFirstObjectByType<WebCamMgr>();
                return _instance;
            }
        }

        public PassthroughCameraSamples.WebCamTextureManager m_webCamTextureManager;
        [Header("Control Stuff")]
        public Material viewMat;
        public TextMesh tm;

        [Header("ArUco Tracking Stuff")]
        // public Awespire.OpenCV.ArUcoTrackerMgr ArUcoTrackerMgr;
        public Transform cameraAnchor;

#if UNITY_EDITOR
        public Transform testCamAnchor;
        WebCamTexture testWebcamTexture;
#endif

        private PassthroughCameraSamples.PassthroughCameraEye CameraEye => m_webCamTextureManager.Eye;
        private Vector2Int CameraResolution => m_webCamTextureManager.RequestedResolution;

        // [Header("Estimate Cam Motion")]
        // public float ReTrackingDstTH = 0.05f;
        // public float FwDst = 0.4f;
        private Vector3 nowEstCamPos = Vector3.zero;
        private Vector3 prevEstCamPos = Vector3.zero;

        [Header("Buffer Pos and Rot")]
        public int bufferSize = 15;
        private int bufferCnt = 0;
        private static int estimateWebCamTexDelay = 3;
        private static int usedBufferShift = 0;
        private static List<Vector3> bufferedPos = new List<Vector3>();
        private static List<Quaternion> bufferedRot = new List<Quaternion>();

        [Space]
        public bool ShowDebugTexture = false;
        private string debugText = "";
        private Texture2D debugTexture = null;

        private void Awake() {
            // Check Instance
            if (_instance == null) {
                _instance = this;
            }

            if (WebCamMgr.instance.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()) {
                DontDestroyOnLoad(this.gameObject);
            } else {
                Destroy(this.gameObject);
            }
        }

        private IEnumerator Start() {
#if UNITY_EDITOR
            testWebcamTexture = new WebCamTexture();
            testWebcamTexture.Play();
            viewMat.SetTexture("_BaseMap", testWebcamTexture);
            yield return null;

            // ArUcoTrackerMgr.Initialize(1920, 1080, 960, 540, 1920, 1080);
            Awespire.OpenCV.ArUcoTrackerMgr.instance.Initialize(testWebcamTexture.width, testWebcamTexture.height,
                testWebcamTexture.width * 0.5f, testWebcamTexture.height * 0.5f,
                testWebcamTexture.width, testWebcamTexture.height);
#else
            // Make sure the manager is disabled in scene and enable it only when the required permissions have been granted
            // Wait for camera permissions
            Assert.IsFalse(m_webCamTextureManager.enabled);
            while (PassthroughCameraSamples.PassthroughCameraPermissions.HasCameraPermission != true) {
                yield return null;
            }

            // Set the 'requestedResolution' and enable the manager
            m_webCamTextureManager.RequestedResolution = PassthroughCameraSamples.PassthroughCameraUtils.GetCameraIntrinsics(CameraEye).Resolution;
            m_webCamTextureManager.enabled = true;
            while (m_webCamTextureManager.WebCamTexture == null) {
                yield return null;
            }

            ShowDebugText($"WebCamTexture({CameraResolution.x} x {CameraResolution.y})\nready and playing.");
            if (viewMat != null) {
                viewMat.SetTexture("_BaseMap", m_webCamTextureManager.WebCamTexture);
            }
            InitialMarkerTracking();
#endif
            InitialEstimateDelay();
        }

        void Update() {
            debugText = PassthroughCameraSamples.PassthroughCameraPermissions.HasCameraPermission == true ? "Permission granted." : "No permission granted.";
            debugText += $"\n{CameraResolution.x} x {CameraResolution.y}";
            debugText += $"\n{usedBufferShift} + {estimateWebCamTexDelay} | {bufferSize}";
            ShowDebugText(debugText);

#if UNITY_EDITOR
            if (testWebcamTexture == null)
                return;
#else
            if (m_webCamTextureManager.WebCamTexture == null)
                return;
            
            if (OVRInput.GetDown(OVRInput.Button.Two)){
                estimateWebCamTexDelay++;
            }
            if (OVRInput.GetDown(OVRInput.Button.One)){
                estimateWebCamTexDelay--;
            }

            if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)) {
                ShowDebugTexture = !ShowDebugTexture;
            }
#endif
            
            if (Awespire.OpenCV.ArUcoTrackerMgr.instance.IsReady) {
#if UNITY_EDITOR
                cameraAnchor.position = Vector3.zero;
                cameraAnchor.rotation = Quaternion.identity;

                if (ShowDebugTexture) {
                    debugTexture = Awespire.OpenCV.ArUcoTrackerMgr.instance.DetectAndEstimateMarker(testWebcamTexture, testCamAnchor, true, debugTexture);
                } else {
                    Awespire.OpenCV.ArUcoTrackerMgr.instance.DetectAndEstimateMarker_Thread(testWebcamTexture);
                }
#else
                if (!CameraPosBufferHandler()) return;

                if (ShowDebugTexture) {
                    EstimateCameraPose(0, out var estPos, out var estRot);
                    cameraAnchor.position = estPos;
                    cameraAnchor.rotation = estRot;
                    debugTexture = Awespire.OpenCV.ArUcoTrackerMgr.instance.DetectAndEstimateMarker(m_webCamTextureManager.WebCamTexture, cameraAnchor, true, debugTexture);
                } else {
                    Awespire.OpenCV.ArUcoTrackerMgr.instance.DetectAndEstimateMarker_Thread(m_webCamTextureManager.WebCamTexture);
                }
#endif
                // prevEstCamPos = nowEstCamPos;

                if (ShowDebugTexture && debugTexture != null && viewMat != null) {
                    viewMat.SetTexture("_BaseMap", debugTexture);
                }
            } else {
                InitialMarkerTracking();
            }
        }

        void InitialEstimateDelay() {
            if (ShowDebugTexture) {
                estimateWebCamTexDelay = 3;
            } else {
                estimateWebCamTexDelay = 6;
            }
        }

        private void ShowDebugText(string val) {
            Debug.Log($"<color=white>[WebCamMgr] </color> {val}");
            if (tm == null) return;

            tm.text = val;
        }

        private void InitialMarkerTracking() {
            // These intrinsic parameters are essential for accurate marker pose estimation
            var intrinsics = PassthroughCameraSamples.PassthroughCameraUtils.GetCameraIntrinsics(CameraEye);
            var cx = intrinsics.PrincipalPoint.x;  // Principal point X (optical center)
            var cy = intrinsics.PrincipalPoint.y;  // Principal point Y (optical center)
            var fx = intrinsics.FocalLength.x;     // Focal length X
            var fy = intrinsics.FocalLength.y;     // Focal length Y
            var width = intrinsics.Resolution.x;   // Image width
            var height = intrinsics.Resolution.y;  // Image height

            // Initialize the ArUco tracking with camera parameters
            Awespire.OpenCV.ArUcoTrackerMgr.instance.Initialize(width, height, cx, cy, fx, fy);
        }

        // true - buffer avalible
        private bool CameraPosBufferHandler() {
            var cameraPose = PassthroughCameraSamples.PassthroughCameraUtils.GetCameraPoseInWorld(CameraEye);

            bufferedPos.Add(cameraPose.position);
            bufferedRot.Add(cameraPose.rotation);

            if (bufferCnt < bufferSize) {
                bufferCnt++;
                return false;
            } else {
                bufferedPos.RemoveAt(0);
                bufferedRot.RemoveAt(0);
            }

            return true;
        }

        #region Static Methods

        public static void EstimateCameraPose(out Vector3 pos, out Quaternion rot) {
            var cameraPose = PassthroughCameraSamples.PassthroughCameraUtils.GetCameraPoseInWorld(PassthroughCameraSamples.PassthroughCameraEye.Left);

            pos = cameraPose.position;
            rot = cameraPose.rotation;
        }

        public static void EstimateCameraPose(int bufferShift, out Vector3 pos, out Quaternion rot) {
            if (bufferedPos.Count == 0) {
                EstimateCameraPose(out pos, out rot);
                return;
            }

            int targetID = 0;

            if (bufferShift + estimateWebCamTexDelay >= bufferedPos.Count) {
                targetID = 0;
            } else {
                usedBufferShift = bufferShift;
                targetID = bufferedPos.Count - (bufferShift + estimateWebCamTexDelay);
            }

            pos = bufferedPos[targetID];
            rot = bufferedRot[targetID];
        }

        #endregion
    }

}


