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
    private Transform crosshair;

    private UISprite bulletBar;

    public new void Awake()
    {
        base.Awake();

        AllowTransitionChain(State.IDLE, State.RELOAD, State.IDLE);
        AllowTransitionChain(State.IDLE, State.FIRE, State.IDLE);

        ForceEnterState(State.IDLE);

        numBullets = clipSize;

        if (hunter == null) hunter = transform.parent.parent.gameObject;

        firePoint = transform.FindChild("FirePoint");

        muzzle = transform.FindChild("Muzzle").gameObject;
        muzzle.transform.position = firePoint.position;

        bulletBar = GameObject.Find("BulletBar").GetComponent<UISprite>();
    }

    void Start()
    {
        // Set fire point position to crosshair position.
        crosshair = myParent.transform.Find("Crosshair");
        Vector3 newLocalPosition = transform.localPosition;
        newLocalPosition.y += crosshair.position.y - firePoint.position.y;
        transform.localPosition = newLocalPosition;
    }

    void OnDisable()
    {
        StopReload();
    }

    public void Reload()
    {
        if (numBullets == clipSize) return;

        if (Advance(State.RELOAD))
        {
            reloadCompletitionTime = Time.fixedTime + reloadTime;

            if (reloadSounds != null && reloadSounds.Length > 0) AudioCenter.PlayClipAtMainCamera(reloadSounds[0]);
        }
    }

    public void StopReload()
    {
        if (GetCurrentState() == State.RELOAD)
        {
            if (reloadSounds != null && reloadSounds.Length > 0)
            {
                AudioSource reloadSoundSource = AudioCenter.instance.GetActiveAudioSourceForClip(reloadSounds[0]);
                
                if (reloadSoundSource != null) reloadSoundSource.Stop();
            }
            
            Advance(State.IDLE);
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

            if ("BigShotgun".Equals(gameObject.name)) numBullets = 0;
        } else
        {
            float angle = -dispersion + Random.value * dispersion * 2;

            SpawnBullet(angle);
        }
    }

    new void Update()
    {
        base.Update();

        float fillAmount = 0f;

        if (GetCurrentState() == State.RELOAD)
        {
            float reloadStartTime = reloadCompletitionTime - reloadTime;

            fillAmount = (Time.fixedTime - reloadStartTime) / reloadTime;
        } else
        {
            fillAmount = numBullets / (float)clipSize;
        }

        bulletBar.fillAmount = fillAmount;
    }

    protected void SpawnBullet(float angle)
    {
        float maxDistance = 100f;

        Vector2 position = firePoint.position.ToVector2();

        int layerMask = 1 << Layers.ENEMY;

        Vector2 dir = -Vector2.right.Rotate(angle);

        RaycastHit2D[] hits = Physics2D.RaycastAll(position, dir, maxDistance, layerMask);

        float damage = this.damage;

        RaycastHit2D? lastHit = null;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];

            /*
             * Do not allow to kill offscreen enemies cause
             * 1) it's not fair.
             * 2) all enemies have Animation set to "Based on Renderers", so if enemy will die offscreen - level glitches.
             */

            if (hit.point.x < Consts.BF_L) continue;

            Creature2 mob = hit.transform.gameObject.GetComponent(typeof(Creature2)) as Creature2;

            if (mob != null) 
            {
                if (damage < 1) break;

                mob.Damage(damage);

                GameObject bloodSparksPrefab = ServiceLocator.current.bloodSparksPrefab;
                GameObject bloodSparks = ServiceLocator.current.pool.Instantiate(bloodSparksPrefab, hit.point.ToVector3(), Quaternion.identity);
                bloodSparks.GetComponent<BloodSparks>().Emit(Random.Range(15, 35));

                damage = (damage * pierce) / 100f;

                lastHit = hit;
            }
        }

        float distance = maxDistance;

        if (lastHit != null && damage < 1) distance = lastHit.Value.distance;

        GameObject bullet = ServiceLocator.current.pool.Instantiate(ServiceLocator.current.bulletPrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = bullet.GetComponent<LineRenderer>();
        lr.SetPosition(0, firePoint.position);
        lr.SetPosition(1, firePoint.position + (dir * distance).ToVector3());
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
