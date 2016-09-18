using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrafficControl : MonoBehaviour {
	private float[] lanes;

	private CartPool pool;
	private Transform playerTrans;
	private LinkedList<Transform>[] activeCars;
	private int[] spawnTimers;

	public int minDensity;
	public int maxDensity;
	public int densityIncrease; // Density increase per 10 seconds.

	public int minimumSpawnGap;

	public float southboundDensityMultiplier; // Really depends on car speeds.
	public float northboundGapMultiplier; // Same as above.
	private int northGap;

	private int currentDensity;

	private int counterMax;
	private int counter;

	private int timerBaseSouth;
	private int timerBaseNorth;

	public float maxLaneDrift;


	public GameObject[] carFabs; // Vehicle prefab list.  Not that these will work with batching, aside from potentially with each other.

	public void Awake () {
		float closeLane = 2.1875f; // Magic numbers incoming!
		float farLane = 6.5625f;
		lanes = new float[] {
			-farLane,
			-closeLane,
			closeLane,
			farLane
		};
		timerBaseSouth = 1500-minimumSpawnGap;
		northGap = Mathf.FloorToInt(minimumSpawnGap*northboundGapMultiplier);
		timerBaseNorth = 1500-northGap;
		currentDensity = minDensity;
		if (currentDensity < 1)
			currentDensity = 1;
		if (densityIncrease == 0) {
			maxDensity = -1;
		} else {
			counterMax = 500/densityIncrease;
		}
		counter = 0;
		playerTrans = GameObject.FindWithTag("Player").transform;
		activeCars = new LinkedList<Transform>[4];
		for (int i = 0; i < 4; i++) {
			activeCars[i] = new LinkedList<Transform>();
		}
		spawnTimers = new int[4];
		spawnTimers[0] = minimumSpawnGap + Random.Range(0, timerBaseSouth/currentDensity);
		spawnTimers[1] = minimumSpawnGap + Random.Range(0, timerBaseSouth/currentDensity);
		spawnTimers[2] = northGap + Random.Range(0, timerBaseNorth/currentDensity);
		spawnTimers[3] = northGap + Random.Range(0, timerBaseNorth/currentDensity);
	}

	// If this is on the cartographer object, use the already-created pool.
	public void Start () {
		Cartographer cart = gameObject.GetComponent<Cartographer>();
		if (cart != null) {
			pool = cart.pool;
		} else {
			pool = new CartPool();
		}
	}

	public void FixedUpdate () {
		// If there are no cars to build, then just don't bother.
		if (carFabs.Length == 0)
			return;

		// Increase traffic density over time.
		if (currentDensity < maxDensity && counter++ == counterMax) {
			counter = 0;
			currentDensity++;
		}

		// Despawn traffic outside view distance.
		float viewF = playerTrans.position.z + Camera.main.farClipPlane + 5f;
		float viewB = playerTrans.position.z - (Camera.main.farClipPlane + 5f);
		for (int i = 0; i < 4; i++) {
			// Check if first has past.
			if (activeCars[i].Count > 0 && activeCars[i].First.Value.position.z < viewB) {
				pool.Add(activeCars[i].First.Value.gameObject);
				activeCars[i].RemoveFirst();
			}
			// Check if last has gone ahead.
			if (activeCars[i].Count > 0 && activeCars[i].Last.Value.position.z > viewF) {
				pool.Add(activeCars[i].Last.Value.gameObject);
				activeCars[i].RemoveLast();
			}
		}

		// Spawn traffic (potentially).
		for (int i = 0; i < 4; i ++) {
			if (spawnTimers[i]-- == 0) {
				int carType = Random.Range(0, carFabs.Length);
				GameObject newCar;

				// Check inventory
				if (pool.Get(carFabs[carType].name, out newCar)) {
					newCar.SetActive(true);
				} else {
					newCar = (GameObject)GameObject.Instantiate(carFabs[carType]);
				}

				// Place in lane
				newCar.name = carFabs[carType].name;
				Transform carTrans = newCar.transform;
				
				if (i < 2) {
					carTrans.position = new Vector3(lanes[i] + Random.Range(-maxLaneDrift, maxLaneDrift), carTrans.position.y, playerTrans.position.z + Camera.main.farClipPlane);
					carTrans.rotation = Quaternion.Euler(0f, 180f, 0f);
					spawnTimers[i] = minimumSpawnGap + Random.Range(0, timerBaseSouth/currentDensity);
				} else {
					carTrans.position = new Vector3(lanes[i] + Random.Range(-maxLaneDrift, maxLaneDrift), carTrans.position.y, playerTrans.position.z + Camera.main.farClipPlane/1.5f);
					carTrans.rotation = Quaternion.Euler(0f, 0f, 0f);
					spawnTimers[i] = northGap + Random.Range(0, timerBaseNorth/currentDensity);
				}
				activeCars[i].AddLast(carTrans);
			}
		}


	}
}
