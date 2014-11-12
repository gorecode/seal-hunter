using UnityEngine;
using System.Collections;

public class BearController : MobBehaviour
{
    public float speed = 0.001f;
    public AudioClip death1Sound;
    public AudioClip death2Sound;
    public Transform movable;
    private Animator animator;
    private bool dying;
    private bool dead;
    private float sniffDelay;
    private float sniffDuration;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        if (!dying)
        {
            movable.position += Vector3.right * speed * Time.deltaTime;

            animator.SetFloat("Speed", speed);
        }
        else if (!dead)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName("Base.EyeShot") || animState.IsName("Base.HeadExplosion"))
            {
                if (animState.normalizedTime >= 1.0f)
                {
                    dead = true;

                    EventBus.OnDeath(transform.parent.gameObject);
                }
            }
        }
    }

    public override void OnTouch()
    {
        Die();
    }

    void Die()
    {
        if (dying)
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

        spriteRenderer.sortingLayerName = "Background";

        RemovePhysics();

        dying = true;
    }
}
