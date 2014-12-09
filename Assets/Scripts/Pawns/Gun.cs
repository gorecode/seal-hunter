using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Gun : FSMBehaviour<Gun.State> {
    public enum State {
        IDLE, FIRE, RELOAD
    }

    public AudioClip reloadSound;
    public AudioClip fireSound;

    public int numBullets;

    public int clipSize;
    public int pierce;
    public int damage;
    public float reloadTime;
    public float rateOfFire;
    public float feedbackImpulse;
    public float dispersion;

    public GameObject hunter;

    private float reloadCompletitionTime;
    private float readyToFireTime;

    private Transform firePoint;
    private GameObject muzzle;

    public new void Awake()
    {
        base.Awake();

        AllowTransitionChain(State.IDLE, State.RELOAD, State.IDLE);
        AllowTransitionChain(State.IDLE, State.FIRE, State.IDLE);

        ForceEnterState(State.IDLE);

        numBullets = clipSize;

        if (hunter == null) hunter = transform.parent.gameObject;

        firePoint = transform.FindChild("FirePoint");

        muzzle = transform.FindChild("Muzzle").gameObject;
        muzzle.transform.position = firePoint.position;
    }

    protected void Reload()
    {
        if (numBullets == clipSize) return;

        if (Advance(State.RELOAD))
        {
            Debug.Log("Start Reload");

            reloadCompletitionTime = Time.fixedTime + reloadTime;

            if (reloadSound != null) AudioCenter.PlayClipAtMainCamera(reloadSound);
        }
    }

    public void Fire()
    {
        if (numBullets == 0 || GetCurrentState() != State.IDLE) return;

        numBullets--;

        Debug.Log("Fire, bullets left = " + numBullets);

        hunter.rigidbody2D.AddForce(Vector2.right * feedbackImpulse, ForceMode2D.Impulse);

        Advance(State.FIRE);

        readyToFireTime = Time.fixedTime + 1.0f / rateOfFire;

        if (fireSound != null) AudioCenter.PlayClipAtMainCamera(fireSound);

        muzzle.SetActive(true);

        SpawnBullet();
    }

    protected void SpawnBullet()
    {
        Vector2 position = firePoint.position.ToVector2();

        int layerMask = 1 << Layers.ENEMY;

        RaycastHit2D hit = Physics2D.Raycast(position, -Vector2.right, 100, layerMask);

        if (hit.collider != null)
        {
            Component t = hit.transform.gameObject.GetComponent(typeof(ITouchable));
            if (t != null) (t as ITouchable).OnTouch();
        }
    }

    protected void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R)) Reload();
        if (Input.GetKey(KeyCode.F)) Fire();

        if (numBullets == 0) Reload();

        switch (GetCurrentState())
        {
            case State.RELOAD:
                if (Time.fixedTime >= reloadCompletitionTime)
                {
                    Debug.Log("Reload complete");

                    numBullets = clipSize;

                    Advance(State.IDLE);
                }
                break;
            case State.FIRE:
                if (Time.fixedTime >= readyToFireTime) Advance(State.IDLE);
                break;
        }
    }
}
