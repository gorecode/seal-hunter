using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class OrthoCamera : MonoBehaviour {
	public float top = 1.0f;
	public float bottom = -1.0f;
	public float left = -1.0f;
	public float right = 1.0f;
	public float zNear = -1.0f;
	public float zFar = 10.0f;

	void Start()
	{
		UpdateProjectionMatrix ();
	}

	public void UpdateProjectionMatrix()
	{
		camera.projectionMatrix = Matrix4x4.Ortho (left, right, bottom, top, zNear, zFar);
	}
}
