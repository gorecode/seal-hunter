//--------------------------------------------
//            NGUI: HUD Text
// Copyright © 2012 Tasharen Entertainment
//--------------------------------------------

using UnityEngine;

/// <summary>
/// Attaching this script to an object will make it visibly follow another object, even if the two are using different cameras to draw them.
/// </summary>

[AddComponentMenu("NGUI/Examples/Follow Target")]
public class UIFollowTarget : MonoBehaviour
{
	/// <summary>
	/// 3D target that this object will be positioned above.
	/// </summary>

	public Transform target;

	/// <summary>
	/// Game camera to use.
	/// </summary>

	public Camera gameCamera;

	/// <summary>
	/// Whether the children will be disabled when this object is no longer visible.
	/// </summary>

	public bool disableIfInvisible = true;

    public Bounds bounds;

	Transform mTrans;
	bool mIsVisible = false;

	/// <summary>
	/// Cache the transform;
	/// </summary>

	void Awake () { mTrans = transform; }

	/// <summary>
	/// Find both the UI camera and the game camera so they can be used for the position calculations
	/// </summary>

	void Start()
	{
		if (target != null)
		{
			if (gameCamera == null) gameCamera = NGUITools.FindCameraForLayer(target.gameObject.layer);
			SetVisible(false);
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
			enabled = false;
		}
	}

	/// <summary>
	/// Enable or disable child objects.
	/// </summary>

	void SetVisible (bool val)
	{
		mIsVisible = val;

		for (int i = 0, imax = mTrans.childCount; i < imax; ++i)
		{
			NGUITools.SetActive(mTrans.GetChild(i).gameObject, val);
		}
	}

	/// <summary>
	/// Update the position of the HUD object every frame such that is position correctly over top of its real world object.
	/// </summary>

	void Update ()
	{
		Vector3 vp = gameCamera.WorldToViewportPoint(target.position);

		bool isVisible = (gameCamera.isOrthoGraphic || vp.z > 0f) && (!disableIfInvisible || (vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f));

		// Update the visibility flag
		if (mIsVisible != isVisible) SetVisible(isVisible);

		// If visible, update the position
		if (isVisible)
		{
            Vector3 uip;
            uip.x = Mathf.Lerp(bounds.min.x, bounds.max.x, vp.x);
            uip.y = Mathf.Lerp(bounds.min.y, bounds.max.y, vp.y);
            uip.z = mTrans.localPosition.z;

			mTrans.localPosition = uip;
		}

		OnUpdate(isVisible);
	}

	/// <summary>
	/// Custom update function.
	/// </summary>

	protected virtual void OnUpdate (bool isVisible) { }
}
