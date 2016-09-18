using UnityEngine;
using System.Collections;

public class ReloadLogic : MonoBehaviour {
    public RedlinePistolAnimator PistolLogic;
    public Animator MagAnimator;
    public bool CanReload;

    void ReloadingAvailable() {
        CanReload = true;
    }
    public void TriggerMagDispense() {
        if (CanReload) {
            CanReload = false;
            Invoke("ReloadingAvailable", 2);
            MagAnimator.Play("MagDispense");
        }


    }
    void OnTriggerEnter(Collider other) {
        //Destroy(other.gameObject);

        if (other.gameObject.GetComponent<RedlinePistolAnimator>()) {
            other.gameObject.GetComponent<RedlinePistolAnimator>().Reload();

            MagAnimator.Play("Init");

        }
    }
}
