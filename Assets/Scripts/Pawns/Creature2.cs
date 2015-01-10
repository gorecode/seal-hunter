﻿using UnityEngine;
using System.Collections;

public class Creature2 : FSMBehaviour<Creature2.State> {
    public enum MobType { Seal, Bear, Activist, Pinguin, SealChild, BigBear, Tortoise, Valrus, Unknown }
    public enum State { Alive, Dying, Dead }

    public float maxHealth;
    public float maxHealthIncrementByLevel;

    public float currentSpeed;
    public float walkingSpeed;
    public float runningSpeed;
    public float defaultWalkingSpeed;
    public float defaultRunningSpeed;

    public AudioClip[] soundsOfRessurection;
    public AudioClip[] soundsOfDying;

    protected float health;

    private float initialHealth;

    protected Level currentLevel;

    public void Kill()
    {
        Damage(health);
    }

    public virtual MobType GetMobType()
    {
        return MobType.Unknown;
    }

    public virtual void Damage(float damage)
    {
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

        AllowTransitionChain(State.Alive, State.Dying, State.Dead);

        RegisterState(State.Alive, OnBecomeAlive, OnAlive);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);
    }

    protected void Start()
    {
        currentLevel = PrefabLocator.INSTANCE.gameObject.GetComponent<Level>();
    }

    protected virtual void OnBecomeAlive(object param)
    {
        EventBus.OnBecomeAlive(myParent.gameObject);

        health = initialHealth = maxHealth + 1 * maxHealthIncrementByLevel;

        collider2D.enabled = true;
        
        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;
        
        AudioCenter.PlayRandomClipAtMainCamera(soundsOfRessurection);
    }

    protected virtual void OnAlive()
    {
    }

    protected virtual void OnBecomeDying(object param)
    {
        EventBus.OnBecomeDying(myParent.gameObject);

        collider2D.enabled = false;
        
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
