using UnityEngine;

namespace Awespire.OpenCV {
    public class PairArUcoResult : MonoBehaviour, IArUcoTrackBehaviour {
        public int TargetID = 0;
        private Transform savedTransform = null;
        public bool isApplyLostTrack = true;

        bool IsTracking {
            get {
                return (savedTransform != null) && (!isApplyLostTrack || savedTransform.gameObject.activeSelf);
                // if (savedTransform == null) return false;
                // return !isApplyLostTrack || savedTransform.gameObject.activeSelf;
                // return isApplyLostTrack ? savedTransform.gameObject.activeSelf : true;
            }
        }

        #region IArUcoTrackBehaviour Implement
        public virtual void OnMarkerTrack() {
            this.transform.position = savedTransform.position;
            this.transform.forward = savedTransform.up;
        }

        public virtual void OnMarkerLostTrack() {
            this.transform.position = Vector3.zero;
        }
        #endregion

        #region Unity Mono
        public virtual void Start() {

        }

        public virtual void Update() {
            if (ArUcoTrackerMgr.instance == null) return;

            if (savedTransform == null) {
                savedTransform = ArUcoTrackerMgr.instance.MatchArUcoIDObj(TargetID);
            } else {
                if (IsTracking) {
                    OnMarkerTrack();
                } else {
                    OnMarkerLostTrack();
                }
            }
        }
        #endregion
    }

    #region Define Interface
    public interface IArUcoTrackBehaviour {
        void OnMarkerTrack();
        void OnMarkerLostTrack();
    }
    #endregion
}

