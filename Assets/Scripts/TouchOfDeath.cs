using UnityEngine;
using System.Collections;

public class TouchOfDeath : MonoBehaviour {
    public float unitsToPixels = 100;
    public LayerMask layerMask = -1;
    public Transform pointerHighlight;
    public float touchSlop = 0.3f;

    private static readonly Vector3 OUT_OF_CAMERA = new Vector3(-10, -10, -10);

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

            if (pointerHighlight != null)
            {
                pointerHighlight.position = positionIn2d;
                pointerHighlight.localScale = Vector3.one * touchSlop;
            }

            Collider2D collider = Physics2D.OverlapCircle(positionIn2d, touchSlop * 0.5f, layerMask);

            if (collider != null)
            {
                MobBehaviour mobController = collider.gameObject.GetComponent<MobBehaviour>();

                if (mobController != null) mobController.OnGetTouched();
            }
        }
        else
        {
            if (pointerHighlight != null)
            {
                pointerHighlight.position = OUT_OF_CAMERA; 
            }
        }
    }
}
