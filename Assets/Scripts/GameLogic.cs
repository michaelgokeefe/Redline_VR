using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour {
    public Transform BikeNose;
    public MeshRenderer FadeSphereRenderer;
    public Bike_Motor Motor;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Mathf.Abs(BikeNose.position.x) > 12 ) {
            GameOver();
        }
	}
    public void GameOver() {
        Invoke("Reset", 3);
        Motor.GameOver = true;
        FadeSphereRenderer.material.DOFade(1, 1.5f);

    }
    public void Reset() {
        SceneManager.LoadScene(0);
        //Application.LoadLevel(Application.loadedLevel);
    }

    void OnTriggerEnter(Collider col) {
        if (col.GetComponent<EnemyDrone>()) {
            GameOver();
        }
    }
}
