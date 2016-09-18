using UnityEngine;
using System.Collections;

public class Bike_Motor : MonoBehaviour {

    //public float CruiseSpeed;
    public float CurrentVelocity;
    //public float AccelerationTime;
    public AnimationCurve AccelerationCurve;
    public AnimationCurve DecelerationCurve;
    public bool ThrottleHeld;

    //public float PercentToCruiseSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W)) {
            //if (ThrottleHeld) {
            //PercentToCruiseSpeed += Time.deltaTime / AccelerationTime;
            //PercentToCruiseSpeed = Mathf.Clamp01(PercentToCruiseSpeed);

            CurrentVelocity = CurrentVelocity + (AccelerationCurve.Evaluate(CurrentVelocity) * Time.deltaTime);

        }
        else {
            if (CurrentVelocity > 0) {
                CurrentVelocity = CurrentVelocity + (DecelerationCurve.Evaluate(CurrentVelocity) * Time.deltaTime);
            }
            else {
                CurrentVelocity = 0;
            }
        }


	
	}

    //public void 
}
