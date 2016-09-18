using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cartographer : MonoBehaviour { // MonoBehaviour or no?

	// Need some form of pooling for objects that are outside of vision.  Re-use objects once created and put into the pool?  Definitely!
	// Big ones probably can't change, but small ones...

	// So I need to build objects, place them, set their meshes and colors, and see what performance hit that is.

	// Step 1: basic object types.  How to build a quad.

	public Color32[] colors;
	public Material universalMat;
	public Material lightMat;

	private Transform playerTrans;

	private int covered;

	public CartPool pool;
	private List<GameObject> activePieces;

	public void Awake () {
		covered = 0;
		playerTrans = GameObject.FindWithTag("Player").transform;
		pool = new CartPool();
		activePieces = new List<GameObject>();
	}

	public void Start () {
        playerTrans.position += Vector3.forward * Camera.main.farClipPlane;//  new Vector3(, 0.5f, Camera.main.farClipPlane);
	}

	public void FixedUpdate () {
		float magicBuffer = 10f;
		float viewF = playerTrans.position.z + Camera.main.farClipPlane + magicBuffer;
		float viewB = playerTrans.position.z - (Camera.main.farClipPlane + magicBuffer);
		//Debug.Log(activePieces.Count);
		while (pool.Count("Rough Building") < 10) {
			pool.Add(BuildRough().gameObject);
		}

		// I should do this by chunk, even if objects aren't by chunk.  Chunks probably ought to be composed of the ground (highways?) and blocks of meshes.
		// Buildings should be their own objects and be created in one go.  Possibly build and archive some of start, then have a % chance of making a new building
		// whenever you need more.

		// Thing is, streets are their own chunks.

		// Start on basic straight with smaller side roads.  Need to do 

		List<GameObject> remaining = new List<GameObject>();
		// Archive past objects.
		foreach (GameObject current in activePieces) {
			/*Transform currentTrans = current.transform;

			float differenceX = currentTrans.position.x-playerTrans.position.x;
			if (differenceX < 0)
				differenceX *= -1; // I trust this more than Unity's absolute function - the latter has somehow (?!) gone wonky on me before.

			float differenceZ = currentTrans.position.z-playerTrans.position.z;
			if (differenceZ < 0)
				differenceZ *= -1;*/

			if (current.transform.position.z < viewB) {
				// deactivate case
				pool.Add(current);
			} else {
				// keep case
				remaining.Add(current);
			}
		}
		activePieces = remaining;

		// Instantiate or retrieve new chunks (which then need to repopulate themselves.
		float worldCovered = covered*10f;
		int safeguard = 0; // Just in case a line gets deleted below...
		while (viewF > worldCovered && safeguard++ < 50) {
			
			// Declare variables
			GameObject current;
			Transform currentTrans;

			// Get left side building
			if (pool.Get("Rough Building", out current)) {
				currentTrans = current.transform;
				currentTrans.gameObject.SetActive(true);
			} else {
				currentTrans = BuildRough();
			}
			activePieces.Add(currentTrans.gameObject);
			currentTrans.rotation = Quaternion.Euler(0f, -90f, 0f);
			currentTrans.position = new Vector3(-10f-Random.Range(2f, 6f), 0f, worldCovered);

			// Spawn right side building
			if (pool.Get("Rough Building", out current)) {
				currentTrans = current.transform;
				currentTrans.gameObject.SetActive(true);
			} else {
				currentTrans = BuildRough();
			}
			activePieces.Add(currentTrans.gameObject);
			currentTrans.rotation = Quaternion.Euler(0f, 90f, 0f);
			currentTrans.position = new Vector3(10f+Random.Range(2f, 4f), 0f, worldCovered);

			if (covered%2 == 0) {
				// Get left side streetlight
				if (pool.Get("Streetlight", out current)) {
					currentTrans = current.transform;
					currentTrans.gameObject.SetActive(true);
				} else {
					currentTrans = BuildStreetlight();
				}
				activePieces.Add(currentTrans.gameObject);
				currentTrans.position = new Vector3(-10.2f, 0f, worldCovered);

				// Get right side streetlight
				if (pool.Get("Streetlight", out current)) {
					currentTrans = current.transform;
					currentTrans.gameObject.SetActive(true);
				} else {
					currentTrans = BuildStreetlight();
				}
				activePieces.Add(currentTrans.gameObject);
				currentTrans.position = new Vector3(10.2f, 0f, worldCovered);

				// Get road chunk
				if (pool.Get("Road", out current)) {
					currentTrans = current.transform;
					currentTrans.gameObject.SetActive(true);
				} else {
					currentTrans = BuildRoad();
				}
				activePieces.Add(currentTrans.gameObject);
				currentTrans.position = new Vector3(0f, 0f, worldCovered);
			}

			// Increment variables.  Please no hard lock.
			covered++;
			worldCovered+=10f;
		}
		
		// Build out highway.
	}

	// ---- Building construction.

	// Build a rough apartment/townhouse.
	public Transform BuildRough () {
		GameObject center = new GameObject("Rough Building");
		Transform centerTrans = center.transform;
		/* sidewalks should be a separate thing, done by the street.
		
		float sidewalkHeight = 0.1f;*/

		//float sidewalkWidth = Random.Range (2f, 4f);
		int stories = Random.Range (2, 7);
		float height = 3f * stories;

		float lotWidth = 10f; // Need some system here, but that falls under block building.
		

		// make front
		Transform currentTrans = BuildQuad("Front", 0, lotWidth, height).transform;
		currentTrans.parent = centerTrans;

		// make sides
		currentTrans = BuildQuad("Right", 0, 10f, height).transform;
		currentTrans.position = new Vector3(-lotWidth/2f, 0f, 5f);
		currentTrans.rotation = Quaternion.Euler(0f, 90f, 0f);
		currentTrans.parent = centerTrans;

		currentTrans = BuildQuad("Left", 0, 10f, height).transform;
		currentTrans.position = new Vector3(lotWidth/2f, 0f, 5f);
		currentTrans.rotation = Quaternion.Euler(0f, -90f, 0f);
		currentTrans.parent = centerTrans;

		// make roof
		currentTrans = BuildQuad("Roof", 0, lotWidth, 10f).transform;
		currentTrans.position = new Vector3(0f, height, 0f);
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = centerTrans;

		// make rear
		currentTrans = BuildQuad("Roof", 0, lotWidth, height).transform;
		currentTrans.position = new Vector3(0f, 0f, 10f);
		currentTrans.rotation = Quaternion.Euler(0f, 180f, 0f);
		currentTrans.parent = centerTrans;

		// make windows and door
		// Divide house into columns 1m wide (so 10 of them)

		// Make each wall of windows it's own object, to make it easier to position the whole wall.
		GameObject windowWall = new GameObject();
		Transform wallTrans = windowWall.transform;

		// Choose a door slot
		int doorPos = Random.Range(0, 9);
		int doorPos2 = doorPos;
		if (Random.Range(0, 5) == 0)
			doorPos2 = Random.Range(0, 9);
		for (int why = 0; why < stories; why++) {
			float windowHeight = why*3f;
			for (int ecks = 0; ecks < 9; ecks++) {
				float windowCenter = -4f + ecks;
				if (why == 0 && (ecks == doorPos || ecks == doorPos2)) {
					// Make door
					Transform doorTrans = BuildDoor(1f, 2.4f).transform;
					doorTrans.position = new Vector3(windowCenter, windowHeight+0.1f, -0.1f);
					doorTrans.parent = wallTrans;
				} else {
					// Make window (possibly)
					if (Random.Range(0, 10) == 0) {
						Transform windowTrans = BuildWindow(0.8f, 1.2f).transform;
						windowTrans.position = new Vector3(windowCenter, windowHeight+1.2f, -0.1f);
						windowTrans.parent = wallTrans;
					}
				}
			}
		}
		wallTrans.parent = centerTrans;


		windowWall = new GameObject();
		wallTrans = windowWall.transform;
		for (int why = 0; why < stories; why++) {
			float windowHeight = why*3f;
			for (int ecks = 0; ecks < 9; ecks++) {
				float windowCenter = -4f + ecks;
				// Make window (possibly)
				if (Random.Range(0, 6) == 0) {
					Transform windowTrans = BuildWindow(0.8f, 1.2f).transform;
					windowTrans.position = new Vector3(windowCenter, windowHeight+1.2f, -0.1f);
					windowTrans.parent = wallTrans;
				}
			}
		}
		wallTrans.position = new Vector3((-lotWidth/2f)-0.1f, 0f, 5f);
		wallTrans.rotation = Quaternion.Euler(0f, 90f, 0f); 
		wallTrans.parent = centerTrans;

		windowWall = new GameObject();
		wallTrans = windowWall.transform;
		for (int why = 0; why < stories; why++) {
			float windowHeight = why*3f;
			for (int ecks = 0; ecks < 9; ecks++) {
				float windowCenter = -4f + ecks;
				// Make window (possibly)
				if (Random.Range(0, 6) == 0) {
					Transform windowTrans = BuildWindow(0.8f, 1.2f).transform;
					windowTrans.position = new Vector3(windowCenter, windowHeight+1.2f, -0.1f);
					windowTrans.parent = wallTrans;
				}
			}
		}
		wallTrans.position = new Vector3((lotWidth/2f)+0.1f, 0f, 5f);
		wallTrans.rotation = Quaternion.Euler(0f, -90f, 0f); 
		wallTrans.parent = centerTrans;


		//for (int ground = )
		//currentTrans.parent = centerTrans;
		
		/*BoxCollider col = center.AddComponent<BoxCollider>();
		col.size = new Vector3 (lotWidth, height, 10f);
		col.center = new Vector3(0f, height/2f, 5f);*/
		return centerTrans;
	}

	// ---- End Buildings

	public MeshFilter NewObj (string n, out MeshRenderer rend) {
		GameObject neo = new GameObject(n);
		MeshFilter f = neo.AddComponent<MeshFilter>();
		rend = neo.AddComponent<MeshRenderer>();

		return f;
	}

	// Get a new streetlight

	public Transform BuildStreetlight () {
		Transform slTrans = new GameObject("Streetlight").transform;
		//Transform lightTrans = new GameObject("Light").transform;
		//lightTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		//Light l = lightTrans.gameObject.AddComponent<Light>();
		//l.type = LightType.Spot;
		//l.intensity = 3f;
		//l.range = 40f;
		//l.spotAngle = 80f;
		//lightTrans.position = new Vector3(0f, 4f, 0.1f);
		//lightTrans.parent = slTrans;

		// Note: I really should have made a function to make a straight-up box in a single mesh rather than making so many quads.  REALLY SHOULD HAVE DONE THIS.

		// Pole
			// box, no top or bottom
		Transform poleTrans = BuildQuad("Poleside", 2, 0.2f, 4f).transform;
		poleTrans.parent = slTrans;

		poleTrans = BuildQuad("Poleside", 2, 0.2f, 4f).transform;
		poleTrans.position = new Vector3(-0.1f, 0f, 0.1f);
		poleTrans.rotation = Quaternion.Euler(0f, 90f, 0f);
		poleTrans.parent = slTrans;

		poleTrans = BuildQuad("Poleside", 2, 0.2f, 4f).transform;
		poleTrans.position = new Vector3(0.1f, 0f, 0.1f);
		poleTrans.rotation = Quaternion.Euler(0f, -90f, 0f);
		poleTrans.parent = slTrans;

		poleTrans = BuildQuad("Poleside", 2, 0.2f, 4f).transform;
		poleTrans.position = new Vector3(0f, 0f, 0.2f);
		poleTrans.rotation = Quaternion.Euler(0f, 180f, 0f);
		poleTrans.parent = slTrans;


		// Lantern
			// Bright box, no top or bottom
		poleTrans = BuildQuad("Poletop", 2, 0.5f, 0.3f).transform;
		poleTrans.position = new Vector3(0f, 3.75f, -0.15f);
		poleTrans.parent = slTrans;

		poleTrans = BuildQuad("Poletop", 2, 0.5f, 0.3f).transform;
		poleTrans.position = new Vector3(-0.25f, 3.75f, 0.1f);
		poleTrans.rotation = Quaternion.Euler(0f, 90f, 0f);
		poleTrans.parent = slTrans;

		poleTrans = BuildQuad("Poletop", 2, 0.5f, 0.3f).transform;
		poleTrans.position = new Vector3(0.25f, 3.75f, 0.1f);
		poleTrans.rotation = Quaternion.Euler(0f, -90f, 0f);
		poleTrans.parent = slTrans;

		poleTrans = BuildQuad("Poletop", 2, 0.5f, 0.3f).transform;
		poleTrans.position = new Vector3(0f, 3.75f, 0.35f);
		poleTrans.rotation = Quaternion.Euler(0f, 180f, 0f);
		poleTrans.parent = slTrans;

		poleTrans = BuildQuad("PoleLight", 3, 0.5f, 0.5f).transform;
		poleTrans.position = new Vector3(0f, 3.75f, 0.35f);
		poleTrans.rotation = Quaternion.Euler(-90f, 0f, 0f);
		poleTrans.parent = slTrans;

		/*BoxCollider col = slTrans.gameObject.AddComponent<BoxCollider>();
		col.size = new Vector3 (0.2f, 4f, 0.2f);
		col.center = new Vector3(0f, 2f, 0.1f);*/

		return slTrans;
	}

	// Get a new window

	public GameObject BuildWindow (float width, float height) {
		return BuildQuad("Rough Window", 1, width, height);
	}

	// Get a new door

	public GameObject BuildDoor (float width, float height) {
		return BuildQuad("Rough Door", 2, width, height);
	}

	// Get a new street section (size 100)

	public Transform BuildRoad () {
		Transform streetTrans = new GameObject("Road").transform;
		Transform currentTrans;
		// Make the street background.
		currentTrans = BuildQuad("Pavement", 2, 20f, 20f).transform;
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = streetTrans;

		// Make the left sidewalk.
		currentTrans = BuildQuad("Sidewalk", 5, 6f, 20f).transform;
		currentTrans.position = new Vector3(-13f, 0f, 0f);
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = streetTrans;

		// Make the right sidewalk.
		currentTrans = BuildQuad("Sidewalk", 5, 6f, 20f).transform;
		currentTrans.position = new Vector3(13f, 0f, 0f);
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = streetTrans;

		// Markings
			// Guideline Left
		currentTrans = BuildQuad("WarningLine", 6, 0.3f, 20f).transform;
		currentTrans.position = new Vector3(-8.5f, 0.05f, 0f);
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = streetTrans;

			// Guideline right
		currentTrans = BuildQuad("WarningLine", 6, 0.3f, 20f).transform;
		currentTrans.position = new Vector3(8.5f, 0.05f, 0f);
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = streetTrans;

			// Centerline Left
		currentTrans = BuildQuad("WarningLine", 6, 0.3f, 20f).transform;
		currentTrans.position = new Vector3(-0.25f, 0.05f, 0f);
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = streetTrans;

			// Centerline Right
		currentTrans = BuildQuad("WarningLine", 6, 0.3f, 20f).transform;
		currentTrans.position = new Vector3(0.25f, 0.05f, 0f);
		currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
		currentTrans.parent = streetTrans;

		for (int i = 0; i < 4; i++) {
			float dashPos = i*5f;
			// Dash Left
			currentTrans = BuildQuad("LaneLine", 7, 0.2f, 2f).transform;
			currentTrans.position = new Vector3(-4.375f, 0.05f, dashPos);
			currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
			currentTrans.parent = streetTrans;

			// Dash Right
			currentTrans = BuildQuad("LaneLine", 7, 0.2f, 2f).transform;
			currentTrans.position = new Vector3(4.375f, 0.05f, dashPos);
			currentTrans.rotation = Quaternion.Euler(90f, 0f, 0f);
			currentTrans.parent = streetTrans;
		}

		return streetTrans;
	}

	// Get a new gameobject that's a custom quad:

	public GameObject BuildQuad (string n) {
		return BuildQuad(n, 0, 1f, 1f);
	}

	public GameObject BuildQuad (string n, int color) {
		return BuildQuad(n, color, 1f, 1f);
	}

	public GameObject BuildQuad (string n, float width, float height) {
		return BuildQuad(n, 0, width, height);
	}

	public GameObject BuildQuad (string n, int type, float width, float height) {
		MeshRenderer r;
		MeshFilter f = NewObj(n, out r);
		Mesh m = new Mesh();
		int[] t;
		m.vertices = GetQuad(width, height, out t);
		m.triangles = t;
		//
		if (type == 0) { // Normal wall.
			m.colors32 = BlendedColorize(0, 1, 20f, m.vertices);
		} else if (type == 1) { // Window
			if (Random.Range(0, 2) == 0) {
				m.colors32 = RangeColorize(2, 3, m.vertices.Length);
			} else {
				m.colors32 = RangeColorize(5, 6, m.vertices.Length);
			}
		} else if (type == 2) {
			m.colors32 = Colorize(4, m.vertices.Length);
		} else if (type == 3) {
			m.colors32 = Colorize(7, m.vertices.Length);
		} else if (type == 5) {
			m.colors32 = Colorize(0, m.vertices.Length);
		} else if (type == 6) {
			m.colors32 = Colorize(8, m.vertices.Length);
		} else if (type == 7) {
			m.colors32 = Colorize(9, m.vertices.Length);
		}
		m.RecalculateNormals();

		f.mesh = m;

		r.sharedMaterial = universalMat;

		return f.gameObject;
	}


	// Get vertex colors of the given type (according to the master color array)

	public Color32[] Colorize(int colorDex, int length) {
		Color32[] vertColors = new Color32[length];
		for (int i = 0; i < vertColors.Length; i++)
			vertColors[i] = colors[colorDex];
		return vertColors;
	}

	public Color32[] RangeColorize(int color1, int color2, int length) {
		Color32[] vertColors = new Color32[length];
		float windowColor = Random.Range(0f, 1f);
		for (int i = 0; i < vertColors.Length; i++)
			vertColors[i] = Color32.Lerp(colors[color1], colors[color2], windowColor);
		return vertColors;
	}

	public Color32[] BlendedColorize(int color1, int color2, float maxHeight, Vector3[] verts) {
		Color32[] vertColors = new Color32[verts.Length];
		for (int i = 0; i < vertColors.Length; i++)

			vertColors[i] = Color32.LerpUnclamped(colors[color1], colors[color2], verts[i].y/maxHeight);
		return vertColors;
	}

	public Vector3[] GetQuad (out int[] tris) {
		return GetQuad(1f, 1f, out tris);
	}

	public Vector3[] GetQuad (float width, float height, out int[] tris) {
		Vector3[] verts = new Vector3[] {
			new Vector3(-0.5f*width, 0f, 0f),
			new Vector3(-0.5f*width, height, 0f),
			new Vector3(0.5f*width, height, 0f),
			new Vector3(0.5f*width, 0f, 0f)
		};
		tris = new int[] {
			0, 1, 2,
			0, 2, 3
		};
		return verts;
	}

}
