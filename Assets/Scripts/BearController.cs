using UnityEngine;
using System.Collections;

public class BearController : MonoBehaviour
{
    public float speed = 0.001f;
    public AudioClip death1Sound;
    public AudioClip death2Sound;
    public Transform movable;
    private Animator animator;
    private bool dead;
    private float sniffDelay;
    private float sniffDuration;
    private Vector2 touchPositionIn2d;

    void Awake()
    {
        touchPositionIn2d = new Vector2();
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();

        sniffDelay = 1 + Random.value * 4;
        sniffDuration = 1 + Random.value * 3;

        StartCoroutine("SniffCoroutine");
    }

    IEnumerator SniffCoroutine()
    {
        yield return new WaitForSeconds(sniffDelay);
        float oldSpeed = speed;
        speed = 0.0f;
        yield return new WaitForSeconds(sniffDuration);
        speed = oldSpeed;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            movable.position += Vector3.right * speed * Time.deltaTime;

            animator.SetFloat("Speed", speed);

            if (Input.touchCount > 0 || (Input.mousePresent && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))))
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

        speed = 0;

        bool eyeShot = Random.value >= 0.5;

        AudioClip deathSound = eyeShot ? death1Sound : death2Sound;

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
        }

        animator.Play(eyeShot ? "EyeShot" : "HeadExplosion");

        gameObject.layer = LayerMask.NameToLayer("Background");

        dead = true;
    }
}
