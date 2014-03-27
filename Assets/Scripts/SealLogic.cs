using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class SealLogic : MobBehaviour
{
    public float speed = 0.001f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform parent;
    private bool dyingOrDead;
    private bool dead;
    private bool crawl;
    private bool crawlCoroutineIsRunning;

    void Awake()
    {
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
        if (!dyingOrDead)
        {
            parent.position += Vector3.right * speed * Time.deltaTime;
        }
        else
        {
            if (!dead)
            {
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

                if (animState.IsName("Base.CrawlDeath") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    dead = true;

                    EventBus.EnemyDied.Publish(gameObject);
                }
            }
        }
    }

    public override void OnGetTouched()
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

    void Die()
    {
        if (dyingOrDead)
        {
            return;
        }

        animator.SetBool("Dead", true);

        spriteRenderer.sortingLayerName = "Background";

        RemovePhysics();

        dyingOrDead = true;
    }
}
