using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class Tortoise : Creature {
    public enum AliveSubState {
        WALKING, HIDDING, HIDDEN, APPEARING
    }

    private const string HIDE = "Hide";
    private const string SHOW = "Show";

    public float walkingSpeed = 0.5f;
    public float currentSpeed;

    public AudioClip[] soundsOfRessurection;
    public AudioClip[] soundsOfDying;

    private FSM<AliveSubState> aliveSubState;

    private Action hideAction;
    private Action showAction;

    public override void OnTouch()
    {
        base.OnTouch();

        if (GetCurrentState() == State.Alive)
        {
            switch (aliveSubState.GetCurrentState())
            {
                case AliveSubState.WALKING:
                    Advance(State.Dying, "Die1");
                    break;
                case AliveSubState.APPEARING:
                case AliveSubState.HIDDING:
                    Advance(State.Dying, "Die2");
                    break;
            }
        }
    }

    public new void Awake()
    {
        base.Awake();

        aliveSubState = new FSM<AliveSubState>();
        aliveSubState.AllowTransitionChain(AliveSubState.WALKING, AliveSubState.HIDDING, AliveSubState.HIDDEN, AliveSubState.APPEARING, AliveSubState.WALKING);
        aliveSubState.RegisterState(AliveSubState.WALKING, OnBecomeWalking);
        aliveSubState.RegisterState(AliveSubState.APPEARING, OnBecomeHiddingOrAppearing, OnHiddingOrAppearing);
        aliveSubState.RegisterState(AliveSubState.HIDDING, OnBecomeHiddingOrAppearing, OnHiddingOrAppearing);
        aliveSubState.RegisterState(AliveSubState.HIDDEN, OnBecomeHidden);

        RegisterState(State.Alive, OnBecomeAlive, OnLiving);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);

        hideAction = () => aliveSubState.Advance(AliveSubState.HIDDING, HIDE);
        showAction = () => aliveSubState.Advance(AliveSubState.APPEARING, SHOW);
    }

    void OnEnable()
    {
        ForceEnterState(State.Alive);
    }

    private void OnBecomeAlive(object param)
    {
        collider2D.enabled = true;

        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;

        AudioCenter.PlayRandomClipAtMainCamera(soundsOfRessurection);

        aliveSubState.ForceEnterState(AliveSubState.WALKING);
    }

    private void OnBecomeWalking(object param)
    {
        currentSpeed = walkingSpeed;

        Invoke(hideAction.GetMethodName(), UnityEngine.Random.Range(1.0f, 4.0f));

        animation.PlayImmediately("Walk");
    }

    private void OnBecomeHidden(object param)
    {
        Invoke(showAction.GetMethodName(), UnityEngine.Random.Range(3.0f, 6.0f));
    }

    private void OnBecomeHiddingOrAppearing(object param)
    {
        currentSpeed = 0;

        AnimationState clipState = animation["Hide"];

        if (SHOW.Equals(param))
        {
            clipState.speed = -1;
            clipState.normalizedTime = 1;
        } else
        {
            clipState.speed = 1;
            clipState.time = 0;
        }

        myAnimation.Play("Hide");
    }

    private void OnHiddingOrAppearing()
    {
        if (!myAnimation.isPlaying)
        {
            aliveSubState.Advance(myAnimation["Hide"].speed == 1 ? AliveSubState.HIDDEN : AliveSubState.WALKING);
        }
    }

    private void OnLiving()
    {
        aliveSubState.Update();

        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }

    private void OnBecomeDying(object param)
    {
        CancelInvoke();

        myAnimation.Play((string)param);

        collider2D.enabled = false;
        
        mySpriteRenderer.sortingLayerID = SortingLayer.BACKGROUND;
        
        EventBus.OnBecomeDying(myParent.gameObject);
    }

    private void OnDying()
    {
        if (!myAnimation.isPlaying)
        {
            Advance(State.Dead);
        }
    }

    private void OnBecomeDead(object param)
    {
        EventBus.OnBecomeDead(myParent.gameObject);
    }
}
