using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class Tortoise : Creature2 {
    public enum AliveSubState {
        WALKING, HIDDING, HIDDEN, APPEARING
    }

    private const string HIDE = "Hide";
    private const string SHOW = "Show";

    private FSM<AliveSubState> aliveSubState;

    private Action hideAction;
    private Action showAction;

    public override MobType GetMobType()
    {
        return MobType.Tortoise;
    }

    public override void Damage(float damage)
    {
        if (GetCurrentState() != State.Alive) return;

        if (aliveSubState.GetCurrentState() == AliveSubState.HIDDEN)
        {
            Vector3 position = myParent.position;
            position.x -= 0.01f;
            myParent.position = position;
            return;
        }

        base.Damage(damage);

        if (health > 0) return;
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

    public new void Awake()
    {
        base.Awake();

        aliveSubState = new FSM<AliveSubState>();
        aliveSubState.AllowTransitionChain(AliveSubState.WALKING, AliveSubState.HIDDING, AliveSubState.HIDDEN, AliveSubState.APPEARING, AliveSubState.WALKING);
        aliveSubState.RegisterState(AliveSubState.WALKING, OnBecomeWalking);
        aliveSubState.RegisterState(AliveSubState.APPEARING, OnBecomeHiddingOrAppearing, OnHiddingOrAppearing);
        aliveSubState.RegisterState(AliveSubState.HIDDING, OnBecomeHiddingOrAppearing, OnHiddingOrAppearing);
        aliveSubState.RegisterState(AliveSubState.HIDDEN, OnBecomeHidden);

        hideAction = () => aliveSubState.Advance(AliveSubState.HIDDING, HIDE);
        showAction = () => aliveSubState.Advance(AliveSubState.APPEARING, SHOW);
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        walkingSpeed = UnityEngine.Random.Range(defaultWalkingSpeed, defaultWalkingSpeed + 0.5f);

        aliveSubState.ForceEnterState(AliveSubState.WALKING);

        mySpriteAnimator.Update();
    }

    private void OnBecomeWalking(object param)
    {
        Invoke(hideAction.GetMethodName(), UnityEngine.Random.Range(1.0f, 4.0f));

        currentSpeed = walkingSpeed;

        myAnimation["Walk"].speed = currentSpeed / defaultWalkingSpeed;
        myAnimation.PlayImmediately("Walk");
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

    protected override void OnAlive()
    {
        base.OnAlive();

        aliveSubState.Update();

        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }

    protected override void OnBecomeDying(object param)
    {
        base.OnBecomeDying(param);

        CancelInvoke();
    }
}
