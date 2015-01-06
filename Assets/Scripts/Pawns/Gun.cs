using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Gun : FSMBehaviour<Gun.State> {
    public enum State {
        IDLE, FIRE, RELOAD
    }

    public AudioClip[] reloadSounds;
    public AudioClip[] fireSounds;

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

            if (reloadSounds != null) AudioCenter.PlayRandomClipAtMainCamera(reloadSounds);
        }
    }

    public void Fire()
    {
        if (numBullets == 0 || GetCurrentState() != State.IDLE) return;

        numBullets--;

        hunter.rigidbody2D.AddForce(Vector2.right * feedbackImpulse, ForceMode2D.Impulse);

        Advance(State.FIRE);

        readyToFireTime = Time.fixedTime + 1.0f / rateOfFire;

        if (fireSounds != null) AudioCenter.PlayRandomClipAtMainCamera(fireSounds);

        muzzle.SetActive(true);

        if ("Shotgun".Equals(gameObject.name) || "BigShotgun".Equals(gameObject.name))
        {
            bool isSmallShotgun = "Shotgun".Equals(gameObject.name);

            int bulletCount = isSmallShotgun ? 7 : 15;
            int angleAbs = isSmallShotgun ? 10 : 15;
            
            for (int i = -angleAbs; i <= angleAbs; i += (angleAbs * 2) / bulletCount) {
                float deviation = -1 + Random.value * 2;
                float angle = i + deviation;

                SpawnBullet(angle);
            }

            if (isSmallShotgun) numBullets = 0;
        } else
        {
            SpawnBullet(0);
        }
    }

    protected void SpawnBullet(float angle)
    {
        Vector2 position = firePoint.position.ToVector2();

        int layerMask = 1 << Layers.ENEMY;

        Vector2 dir = -Vector2.right.Rotate(angle);

        RaycastHit2D hit = Physics2D.Raycast(position, dir, 100, layerMask);

        if (hit.collider != null)
        {
            Component t = hit.transform.gameObject.GetComponent(typeof(ITouchable));

            if (t != null) 
            {
                (t as ITouchable).OnTouch();

                GameObject bloodSparksPrefab = PrefabLocator.INSTANCE.bloodSparksPrefab;
                GameObject bloodSparks = GameObjectPool.Instance.Instantiate(bloodSparksPrefab, hit.point.ToVector3(), Quaternion.identity);
                bloodSparks.GetComponent<BloodSparks>().Emit(Random.Range(15, 35));
            }
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
