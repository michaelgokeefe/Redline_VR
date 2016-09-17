using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoadMaker))]
public class RoadMakerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        RoadMaker myTarget = (RoadMaker)target;


        if (GUILayout.Button("Build Roads")) {
            myTarget.BuildRoad();
        }

        //myTarget.experience = EditorGUILayout.IntField("Experience", myTarget.experience);
        //EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
    }
}
