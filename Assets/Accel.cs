using UnityEngine;
using System.Collections;

public class Accel : MonoBehaviour {

    public float Velocity;

    void Update() {
        transform.position += transform.forward * Time.deltaTime * Velocity;
    }
}
