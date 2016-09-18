using UnityEngine;
using System.Collections;

public class Bike_Motor : MonoBehaviour {

    //public float CruiseSpeed;
    public float CurrentVelocity;
    //public float AccelerationTime;
    public AnimationCurve AccelerationCurve;
    public AnimationCurve DecelerationCurve;
    public bool ThrottleHeld;
    public float MaxAcceleration;
    public float MaxDeceleration;
    public float MaxVelocity;
    public bool GameOver;

    //public float PercentToCruiseSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (GameOver) {
            CurrentVelocity = 0;
            return;
        }
        if (ThrottleHeld) {
            CurrentVelocity = CurrentVelocity + ((AccelerationCurve.Evaluate(CurrentVelocity/MaxVelocity)) * MaxAcceleration * Time.deltaTime);
        }
        else {
            if (CurrentVelocity > 0) {
                CurrentVelocity = CurrentVelocity + (DecelerationCurve.Evaluate(CurrentVelocity/MaxVelocity) * MaxDeceleration * Time.deltaTime);
            }
            else {
                CurrentVelocity = 0;
            }
        }

        transform.position += transform.forward * Time.deltaTime * CurrentVelocity;
	}

    //public void 
}
