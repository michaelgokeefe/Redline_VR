using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartPool {

	// Need to store object list/dictionary.
	private Dictionary<string, List<GameObject>> contents; // store by name.

	public CartPool () {
		contents = new Dictionary<string, List<GameObject>>();
	}

	// Need to be able to check if there's contents.
	public bool Contains (string n) {
		if (contents.ContainsKey(n))
			return contents[n].Count > 0;

		return false;
	}

	// Need to be able to add an object, automatically disabling it when added.
	public void Add (GameObject neo) {
		// Disable the object
		neo.SetActive(false);

		// Build inventory entry if necessary.
		if (!contents.ContainsKey(neo.name))
			contents.Add(neo.name, new List<GameObject>());

		// Add the object to inventory.
		contents[neo.name].Add(neo);
	}

	public int Count (string n) {
		if (contents.ContainsKey(n))
			return contents[n].Count;
		return 0;
	}


	// Need to be able to pull an object, NOT automatically enabling it.
	public bool Get(string n, out GameObject old) {
		if (Contains(n)) {
			List<GameObject> currentList = contents[n];
			int dex = Random.Range(0, currentList.Count); // Pulling a random item since I want to inventory non-identical objects together and don't want strict looping.
			old = currentList[dex];
			currentList.RemoveAt(dex);
			return true;
		}
		old = null;
		return false;
		
	}


}
