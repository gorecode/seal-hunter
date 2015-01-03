using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BloodSparks))]
[ExecuteInEditMode]
public class BloodSparksEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BloodSparks myScript = (BloodSparks)target;

        if (GUILayout.Button("Emit"))
        {
            myScript.particleSystem.Play();
        }
    }
}
