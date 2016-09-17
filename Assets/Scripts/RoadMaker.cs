using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadMaker : MonoBehaviour {

    public GameObject RoadSectionPrefab;
    public int RoadSectionLength;
    public GameObject RoadParent;

    public void BuildRoad() {

        var children = new List<GameObject>();
        foreach (Transform child in RoadParent.transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));

        for (int i = 0; i < RoadSectionLength; i++) {
            GameObject roadSectionGO = GameObject.Instantiate(RoadSectionPrefab, transform.position + (Vector3.forward * 9f * i), Quaternion.identity) as GameObject;
            roadSectionGO.transform.SetParent(RoadParent.transform);
        }
    }
}
