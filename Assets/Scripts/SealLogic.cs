using UnityEngine;
using System.Collections;

public class SealLogic : MonoBehaviour
{
    public float speed = 0.001f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform parent;
    private bool dead;
    private bool crawl;
    private Vector2 touchPositionIn2d;
    private bool crawlCoroutineIsRunning;
    void Awake()
    {
        touchPositionIn2d = new Vector2();
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        parent = transform.parent;
    }

    IEnumerator FallAndStartCrawl()
    {
        crawl = true;
        animator.SetBool("Crawl", true);
        float oldSpeed = speed;
        speed = 0;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Crawl"))
        {
            yield return new WaitForFixedUpdate();
        }
        speed = oldSpeed * 0.5f;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            parent.position += Vector3.right * speed * Time.deltaTime;

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
                    if (!crawl)
                    {
                        StartCoroutine(FallAndStartCrawl());
                    }
                    else
                    {
                        if (speed > 0)
                        {
                            Die();
                        }
                    }
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

        animator.SetBool("Dead", true);

        spriteRenderer.sortingLayerName = "Background";

        dead = true;
    }
}
