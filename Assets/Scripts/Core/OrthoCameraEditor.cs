using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(OrthoCamera))]
public class OrthoCameraEditor : Editor {
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

		if (GUILayout.Button ("Update"))
		{
			((OrthoCamera)target).UpdateProjectionMatrix();
		}

		if (GUILayout.Button("Reset Projection Matrix"))
		{
			((OrthoCamera)target).gameObject.camera.ResetProjectionMatrix();
		}
	}
}
