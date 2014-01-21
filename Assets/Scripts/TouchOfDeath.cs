using UnityEngine;
using System.Collections;

public class TouchOfDeath : MonoBehaviour {
    public float unitsToPixels = 100;
    public LayerMask layerMask = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        if (Input.touchCount > 0 || (Input.mousePresent && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))))
        {
            Vector2 positionIn2d;

            if (Input.mousePresent)
            {
                positionIn2d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                positionIn2d = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }

            Collider2D collider = Physics2D.OverlapCircle(positionIn2d, 0.3f, layerMask);

            if (collider != null)
            {
                MobBehaviour mobController = collider.gameObject.GetComponent<MobBehaviour>();

                if (mobController != null) mobController.OnGetTouched();
            }
        }
    }
}
