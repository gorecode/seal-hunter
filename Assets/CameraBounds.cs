using UnityEngine;
using UnityEngineExt;
using System.Collections;

[ExecuteInEditMode]
public class CameraBounds : MonoBehaviour {
	public float width;
	public float height;

	void Start () {
		Vector3 left = camera.ViewportToWorldPoint (Vector3.zero);
		Vector3 right = camera.ViewportToWorldPoint (Vector3.right);
		Vector3 bottom = camera.ViewportToWorldPoint (Vector3.zero);
		Vector3 top = camera.ViewportToWorldPoint (Vector3.up);

		height = top.y - bottom.y;
		width = right.x - left.x;

		Debug.Log ("rect = " + camera.rect);
	}

	void OnDrawGizmosSelected()
	{
		Vector2 bottomLeft;

		bottomLeft.x = 0.64f / 2;
		bottomLeft.y = 0.48f / 2;

		DrawPoint (bottomLeft);
		//DrawPoint (Vector2.right);
		//DrawPoint (Vector2.up);
		//DrawPoint (topRight);
	}

	private void DrawPoint(Vector2 viewportPosition)
	{
		Vector3 vp = viewportPosition.ToVector3 ();
		vp.z = camera.nearClipPlane;
		Vector3 wp = camera.ViewportToWorldPoint (vp);
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (wp, 0.1f);
	}
}
