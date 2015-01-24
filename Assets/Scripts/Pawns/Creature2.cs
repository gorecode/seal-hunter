using UnityEngine;
using System.Collections;

public class Creature2 : FSMBehaviour<Creature2.State> {
    public enum Limb { Head, Body }
    public enum MoveStrategy { FORWARD, SIN }
    public enum MobType { Seal, Bear, Activist, Pinguin, SealChild, BigBear, Tortoise, Valrus, Unknown }
    public enum State { Alive, Dying, Dead }

    public float maxHealth;
    public float maxHealthIncrementByLevel;

    public float currentSpeed;
    public float walkingSpeed;
    public float runningSpeed;
    public float defaultWalkingSpeed;
    public float defaultRunningSpeed;

    public MoveStrategy moveStrategy = MoveStrategy.SIN;

    public AudioClip[] soundsOfRessurection;
    public AudioClip[] soundsOfDying;

    protected float health;
    protected float initialHealth;

    public Vector3 moveDir = Vector3.right;
    public float moveDirMaxYVariation;
    public float sinMoveAmp = 0.1f;
    public float sinMoveHz = 1.0f;

    public float headshotMultiplier = 2.0f;

    public Transform hudRoot;

    public void Kill()
    {
        Damage(health, Limb.Body);
    }

    public virtual MobType GetMobType()
    {
        return MobType.Unknown;
    }

    public virtual void Damage(float damage, Limb target)
    {
        if (target == Limb.Head) damage *= headshotMultiplier;

        health -= damage;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetInitialHealth()
    {
        return initialHealth;
    }

    protected new void Awake()
    {
        base.Awake();

        hudRoot = transform.parent.Find("HudRoot");

        AllowTransitionChain(State.Alive, State.Dying, State.Dead);

        RegisterState(State.Alive, OnBecomeAlive, OnAlive);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);
    }

    protected virtual void OnBecomeAlive(object param)
    {
        if (EventBus.OnBecomeAlive != null) EventBus.OnBecomeAlive(myParent.gameObject);

        int currentLevel = ServiceLocator.current.singlePlayerGame.GetLevelIndex() + 1;

        health = initialHealth = maxHealth + Mathf.Max(0f, currentLevel * maxHealthIncrementByLevel);

        EnableCollider();
        
        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;
        
        AudioCenter.PlayRandomClipAtMainCamera(soundsOfRessurection);
    }

    protected virtual void OnAlive()
    {
    }

    protected virtual void OnBecomeDying(object param)
    {
        EventBus.OnBecomeDying(myParent.gameObject);

        DisableCollider();
        
        mySpriteRenderer.sortingLayerID = SortingLayer.BACKGROUND;

        AudioCenter.PlayRandomClipAtMainCamera(soundsOfDying);

        if (param != null) myAnimation.Play((string)param);
    }

    protected virtual void OnDying()
    {
        if (!myAnimation.isPlaying)
        {
            Advance(State.Dead);
        }
    }

    protected virtual void OnBecomeDead(object param)
    {
        EventBus.OnBecomeDead(myParent.gameObject);
    }

    protected virtual void EnableCollider()
    {
        collider2D.enabled = true;
    }

    protected virtual void DisableCollider()
    {
        collider2D.enabled = false;
    }

    protected void UpdateDefaultMovement()
    {
        myParent.transform.position += moveDir * (currentSpeed * Time.deltaTime);

        if (moveStrategy == MoveStrategy.SIN)
        {
            float k = Mathf.Deg2Rad * 360f * sinMoveHz;
            float y1 = Mathf.Sin(Time.time * k) * sinMoveAmp;
            float y2 = Mathf.Sin((Time.time - Time.deltaTime) * k) * sinMoveAmp;
            Vector3 p = myParent.transform.position;
            p.y += y2 - y1;
            myParent.transform.position = p;
        }
    }

    public OnEnter Action_PlayAnimation(string clipName)
    {
        return delegate(object prop)
        {
            if (prop == null) prop = clipName;
            
            myAnimation.Play((string)prop);
        };
    }
    
    public OnUpdate Action_AdvanceAfterAnimation<T>(FSM<T> fsm, T nextState)
    {
        return delegate
        {
            if (!myAnimation.isPlaying) fsm.Advance(nextState);
        };
    }
}
