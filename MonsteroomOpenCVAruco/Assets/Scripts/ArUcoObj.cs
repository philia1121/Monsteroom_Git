using UnityEngine;
using Awespire.OpenCV;

public class ArUcoObj : PairArUcoResult, IArUcoTrackBehaviour {
    public override void Start() {
        base.Start();

    }

    public override void Update() {
        base.Update();

    }

    #region IArUcoTrackBehaviour Implement
    public override void OnMarkerTrack() {
        base.OnMarkerTrack();
    }

    public override void OnMarkerLostTrack() {
        base.OnMarkerLostTrack();
    }
    #endregion
}
