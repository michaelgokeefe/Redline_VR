using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RedlinePistolAnimator : MonoBehaviour {
    public Animator AnimPistol;
    public Animator AnimSlide;
    public Animator AnimTrigger;

    AudioSource _audioSource;
    public AudioClip ShotSound;
    public Transform BarrelPoint;
    public float Damage;
    public Text AmmoCountText;
    public int AmmoCount;
    public int MagSize;
    public bool CanShoot;
    public float NextShotDelay;
    float _nextShotTimeLeft;
	// Use this for initialization
	void Start () {
        AmmoCount = MagSize;
        AmmoCountText.text = AmmoCount.ToString();
        CanShoot = true;
        _audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (!CanShoot) {
            _nextShotTimeLeft -= Time.deltaTime;
            if (_nextShotTimeLeft <= 0) {
                _nextShotTimeLeft = 0;
                CanShoot = true;
            }
        }
	}

    public void Reload() {
        AmmoCount = MagSize;
        AmmoCountText.text = AmmoCount.ToString();
    }

    public void PullTrigger() {
        AnimTrigger.Play("TriggerPull");

        if (AmmoCount > 0 && CanShoot) {
            AmmoCount--;
            CanShoot = false;

            _nextShotTimeLeft = NextShotDelay;

            RaycastHit hit;
            if (Physics.Raycast(BarrelPoint.position, BarrelPoint.forward, out hit, Mathf.Infinity, 1 << 8)) {
                hit.collider.GetComponent<EnemyDrone>().TakeHit(Damage);

            }
            _audioSource.clip = ShotSound;
            _audioSource.Play();

            if (AmmoCount == 0) {
                AmmoCountText.text = "--";
            }
            else {
                AmmoCountText.text = AmmoCount.ToString();
            }



            AnimPistol.Play("FirePistol");
            if (AmmoCount > 1) {

                AnimSlide.Play("SlideShot");

            }
            else {
                AnimSlide.Play("SlideStick");
            }


        }
    }
    public void ReleaseTrigger() {
        AnimTrigger.Play("TriggerRelease");
    }
}
