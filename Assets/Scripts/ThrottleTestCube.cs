using UnityEngine;
using System.Collections;

public class ThrottleTestCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().material.color = Color.white;
	}
    public void SetThrottle(bool ThrottlePressed) {
        if (ThrottlePressed) {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

}
