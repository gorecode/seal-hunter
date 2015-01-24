using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hunter : Creature2 {
    public enum AliveState {
        IDLE, WALKING, FALLING, STANDING_UP
    }

    public EasyJoystick joystick;
    public EasyButton fireButton;

    private Rigidbody2D rbody;
    private FSM<AliveState> aliveState;

    private float fallTime;
    private List<GameObject> gunList;
    private GameObject activeGun;

    public new void Awake()
    {
        base.Awake();

        gunList = new List<GameObject>();

        InitGunList();

        rbody = myParent.rigidbody2D;

        aliveState = new FSM<AliveState>();
        aliveState.AllowTransitionChain(AliveState.IDLE, AliveState.WALKING, AliveState.IDLE);
        aliveState.AllowTransitionChain(AliveState.FALLING, AliveState.STANDING_UP, AliveState.IDLE);
        aliveState.AllowTransitionChain(AliveState.IDLE, AliveState.FALLING);
        aliveState.AllowTransitionChain(AliveState.WALKING, AliveState.FALLING);
        aliveState.AllowTransitionChain(AliveState.STANDING_UP, AliveState.FALLING);

        aliveState.RegisterState(AliveState.IDLE, OnBecomeIdle);
        aliveState.RegisterState(AliveState.WALKING, OnBecomeWalking, OnWalking);
        aliveState.RegisterState(AliveState.FALLING, OnBecomeFalling, OnFalling);
        aliveState.RegisterState(AliveState.STANDING_UP, OnBecomeStandingUp, OnStandingUp);

        FindActiveGun();
    }

    public GameObject GetActiveGun()
    {
        for (int i = 0; i < gunList.Count; i++)
        {
            GameObject go = gunList[i];

            if (go.activeSelf) return go;
        }
        return null;
    }

    public void SetActiveGunByName(string name)
    {
        for (int i = 0; i < gunList.Count; i++)
        {
            GameObject go = gunList[i];

            bool sameName = go.name.Equals(name);

            go.SetActive(sameName);

            if (sameName) activeGun = go;
        }
    }

    void FindActiveGun()
    {
        for (int i = 0; i < gunList.Count; i++)
        {
            GameObject go = gunList[i];

            if (go.activeSelf)
            {
                activeGun = go;
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //
        // Hunter falls down on collision.
        //

        AliveState myState = aliveState.GetCurrentState();

        if (myState != AliveState.IDLE && myState != AliveState.WALKING) return;

        // Try legacy mobs without limbs.
        Creature2 anyMob = collider.gameObject.GetComponent<Creature2>();
        // Try new mobs with limbs.
        if (anyMob == null) anyMob = collider.gameObject.transform.parent.GetComponent<Creature2>();
        // This is not mob, return.
        if (anyMob == null) return;

        bool fall = false;

        MobType mobType = anyMob.GetMobType();

        float impulse = 0.0f;

        if (mobType == MobType.Seal)
        {
            Seal seal = anyMob as Seal;

            if (seal.GetAliveState() == Seal.Alive_SubState.Crawling)
            {
                fall = true;
            }
        }
        else if (!fall && mobType == MobType.SealChild)
        {
            fall = true;
        } else if (!fall && mobType == MobType.Pinguin)
        {
            Pinguin pinguin = anyMob as Pinguin;

            if (pinguin.GetAliveState() == Pinguin.Alive_SubState.Sliding)
            {
                fall = true;
            }
        } else if (!fall && mobType == MobType.BigBear)
        {
            BigBear bear = anyMob as BigBear;

            if (bear.GetAliveState() == BigBear.AliveSubState.RUNNING)
            {
                impulse = bear.currentSpeed;

                fall = true;
            }
        }

        if (fall && aliveState.Advance(AliveState.FALLING))
        {
            Vector2 force = Vector2.zero;
            force.x = impulse;
            myParent.rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        switch (aliveState.GetCurrentState())
        {
            case AliveState.IDLE:
            case AliveState.WALKING:
                if (HandleMoveInput())
                    aliveState.Advance(AliveState.WALKING);
                else
                    aliveState.Advance(AliveState.IDLE);

                HandleFireInput();
                break;
            case AliveState.FALLING:
                break;
        }

        ClampToBattleField();
    }

    private void InitGunList()
    {
        Transform gunsObject = myParent.transform.Find("Guns");
        foreach (Transform child in gunsObject)
            if (child.GetComponent<Gun>() != null)
                gunList.Add(child.gameObject);
    }

    private void HandleFireInput()
    {
        if (fireButton.buttonState == EasyButton.ButtonState.Down || fireButton.buttonState == EasyButton.ButtonState.Press)
        {
            Gun gun = myParent.gameObject.GetComponentInChildren<Gun>();

            gun.Fire();
        }
    }

    private bool HandleMoveInput()
    {
        float f = walkingSpeed;

        bool result = false;

        Vector2 joystickValue = joystick.JoystickAxis;

        if (Input.GetKey(KeyCode.A))
        {
            rbody.AddForce(-Vector2.right * f);
            result = true;
        } else if (Input.GetKey(KeyCode.D))
        {
            rbody.AddForce(Vector2.right * f);
            result = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            rbody.AddForce(Vector2.up * f);
            result = true;
        } else if (Input.GetKey(KeyCode.S))
        {
            rbody.AddForce(-Vector2.up * f);
            result = true;
        }

        if (!result)
        {
            if (Mathf.Abs(joystickValue.x) + Mathf.Abs(joystickValue.y) > 0.0f)
            {
                rbody.AddForce(joystickValue * f);
                result = true;
            }
        }

        return result;
    }

    private void ClampToBattleField()
    {
        float minX = -3.2f;
        float maxX = 3.2f;
        float minY = -2.4f;
        float maxY = 2.4f;

        Rigidbody2D r2d = myParent.rigidbody2D;

        Vector2 cp = r2d.position;
        Vector2 v = r2d.velocity;

        cp.x = Mathf.Clamp(cp.x, minX, maxX);
        cp.y = Mathf.Clamp(cp.y, minY, maxY);

        bool clampX = false;
        bool clampY = false;

        if (cp.x <= minX && v.x <= 0) clampX = true;
        if (cp.x >= maxX && v.x >= 0) clampX = true;
        if (cp.y <= minY && v.y <= 0) clampY = true;
        if (cp.y >= maxY && v.y >= 0) clampY = true;

        if (clampX || clampY)
        {
            Vector2 np = r2d.position;

            if (clampX) { v.x = 0; np.x = cp.x; }
            if (clampY) { v.y = 0; np.y = cp.y; }

            r2d.velocity = v;
            r2d.position = np;
        }
    }

    protected void OnBecomeFalling(object param)
    {
        myAnimation.Play("Fall");

        fallTime = Time.fixedTime;
    }

    protected void OnFalling()
    {
        activeGun.SetActive(false);

        if (!myAnimation.isPlaying && Time.fixedTime - fallTime >= 0.5f && myParent.rigidbody2D.velocity.magnitude <= 0.01f)
        {
            aliveState.Advance(AliveState.STANDING_UP);
        }
    }

    protected void OnBecomeStandingUp(object param)
    {
        activeGun.SetActive(false);

        myAnimation.Play("StandUp");
    }

    protected void OnStandingUp()
    {
        if (!myAnimation.isPlaying) aliveState.Advance(AliveState.IDLE);
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        mySpriteRenderer.sortingLayerID = SortingLayer.PLAYER;

        aliveState.ForceEnterState(AliveState.IDLE);
    }

    protected override void OnAlive()
    {
        base.OnAlive();

        aliveState.Update();

        AliveState state = aliveState.GetCurrentState();

        if (state == AliveState.IDLE || state == AliveState.WALKING)
        {
            if ((Mathf.Abs(rbody.velocity.x) + Mathf.Abs(rbody.velocity.y)) >= 0.05f)
            {
                aliveState.Advance(AliveState.WALKING);
            }
            else
            {
                aliveState.Advance(AliveState.IDLE);
            }
        }
    }

    private void OnBecomeIdle(object param)
    {
        activeGun.SetActive(true);

        myAnimation.Stop();

        mySpriteAnimator.sheet = 0;
        mySpriteAnimator.index = 0;
    }

    private void OnBecomeWalking(object param)
    {
        myAnimation.Play("Walk");
    }

    private void OnWalking()
    {
        myAnimation["Walk"].speed = Mathf.Max(Mathf.Abs(rbody.velocity.x), Mathf.Abs(rbody.velocity.y)) * 1.5f;
    }
}
