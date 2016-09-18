using UnityEngine;
using System.Collections;
using VRTK;

public class EnemyShot : VRTK_InteractableObject {

    public override void StartUsing(GameObject currentUsingObject) {
        OnInteractableObjectUsed(SetInteractableObjectEvent(currentUsingObject));
        usingObject = currentUsingObject;

        Destroy(usingObject);
        //
        //Accelerate(usingObject.transform.root); // CameraRig is the root of the handle bars
        //ThrottleTestCube.SetThrottle(true);
    }
}
