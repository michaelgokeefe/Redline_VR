using UnityEngine;
using System.Collections;
using VRTK;

public class RightHandleBar : VRTK_InteractableObject {

    public override void StartUsing(GameObject usingObject) {
        Debug.LogWarning("Handle Using script is being used.");
        base.StartUsing(usingObject);
        Accelerate(usingObject.transform.root); // CameraRig is the root of the handle bars
    }

    // Not needed currently, but keeping in case
    protected override void Start() {
        Debug.LogWarning("Handle Using script started.");
        base.Start();
    }

    private void Accelerate(Transform cameraRig) {
        // Jesse's acceleration
        Debug.LogWarning("ACCELERATINGGGGGGGGGGGGG!!!!!!!!!");
    }
}
