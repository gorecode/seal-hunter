using UnityEngine;
using System.Collections;

public class BearController : MonoBehaviour
{
    public float speed = 0.001f;
    public AudioClip death1Sound;
    public AudioClip death2Sound;
    public Transform movable;
    private bool dead;

    private Vector2 touchPositionIn2d;

    void Awake()
    {
        touchPositionIn2d = new Vector2();
    }

    // Use this for initialization
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            movable.position += Vector3.right * speed * Time.deltaTime;

            if (Input.touchCount > 0 || (Input.mousePresent && Input.GetMouseButtonDown(0)))
            {
                Vector3 positionIn3d;

                if (Input.mousePresent)
                {
                    positionIn3d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else
                {
                    positionIn3d = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                }

                touchPositionIn2d.Set(positionIn3d.x, positionIn3d.y);

                if (gameObject.collider2D.OverlapPoint(touchPositionIn2d))
                {
                    Die();
                }
            }
        }
    }

    void Die()
    {
        if (dead)
        {
            return;
        }

        Animator animator = GetComponent<Animator>();

        if (Random.value >= 0.5)
        {
            animator.SetBool("Eyeshot", true);

            if (death1Sound != null) AudioSource.PlayClipAtPoint(death1Sound, Camera.main.transform.position);
        }

        if (death2Sound != null) AudioSource.PlayClipAtPoint(death2Sound, Camera.main.transform.position);

        animator.SetInteger("Health", 0);
        
        gameObject.layer = LayerMask.NameToLayer("Background");

        dead = true;
    }
}
