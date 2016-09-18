using UnityEngine;
using System.Collections;

public class EnemyDrone : MonoBehaviour {
    public float Health;
    public float MaxHealth;
	// Use this for initialization
	void Start () {
        Health = MaxHealth;
	}

    public void TakeHit(float damage) {
        Health -= damage;
        if (Health <= 0) {
            Die();
        }
    }
    public void Die() {
        Destroy(gameObject);
    }
}
