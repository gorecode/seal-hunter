using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Seal : MobBehaviour
{
    public float speed = 0.001f;
    public AudioClip[] soundsOfDying;
    public AudioClip[] soundsOfFalling;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform parent;
    private bool dying;
    private bool dead;
    private bool crawling;
    private int dyingMethod;
    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        parent = transform.parent;
        dyingMethod = Random.Range(0, 2);
    }

    IEnumerator FallAndStartCrawl()
    {
        if (soundsOfFalling != null && soundsOfFalling.Length > 0)
        {
            AudioSource.PlayClipAtPoint(soundsOfFalling[Random.Range(0, soundsOfFalling.Length - 1)], Camera.main.transform.position);
        }
        crawling = true;
        animator.SetBool("Crawl", true);
        float oldSpeed = speed;
        speed = 0;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base.Crawl"))
        {
            yield return new WaitForFixedUpdate();
        }
        speed = oldSpeed * 0.5f;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dying)
        {
            parent.position += Vector3.right * speed * Time.deltaTime;
        }
        else
        {
            if (!dead && dying)
            {
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

                if (animState.IsName("Base.FallDeath") || animState.IsName("Base.CrawlDeath"))
                {
                    if (animState.normalizedTime >= 1.0f)
                    {
                        dead = true;

                        EventBus.EnemyDied.Publish(gameObject);
                    }
                }
            }
        }
    }

    public override void OnGetTouched()
    {
        if (dying) return;

        if (dyingMethod == 0)
        {
            animator.SetBool("Fall", true);
            StartDying();
        }
        else if (crawling)
        {
            if (speed > 0) StartDying();
        }
        else
        {
            StartCoroutine(FallAndStartCrawl());
        }
    }

    void StartDying()
    {
        if (dying)
        {
            return;
        }

        if (soundsOfDying != null && soundsOfDying.Length > 0)
        {
            AudioClip sound = soundsOfDying[Random.Range(0, soundsOfDying.Length - 1)];

            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position);
        }

        animator.SetBool("Dead", true);

        spriteRenderer.sortingLayerName = "Background";

        RemovePhysics();

        dying = true;

        speed = 0;
    }
}
