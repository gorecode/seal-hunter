using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class BigBear : Creature2 {
    public enum AliveSubState { WALKING, RUNNING }

    public float friction;

    public float delayBeforeRun = 10.0f;

    private Action advanceToRunningState;
    private FSM<AliveSubState> aliveFsm;

    public AliveSubState GetAliveState()
    {
        return aliveFsm.GetCurrentState();
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (GetCurrentState() == State.Alive && initialHealth - health >= initialHealth / 8)
        {
            aliveFsm.Advance(AliveSubState.RUNNING);
        }

        if (health > 0) return;

        Advance(State.Dying, "Die");
    }

    public override MobType GetMobType()
    {
        return MobType.BigBear;
    }

    protected new void Awake()
    {
        base.Awake();

        aliveFsm = new FSM<AliveSubState>();
        aliveFsm.AllowTransitionChain(AliveSubState.WALKING, AliveSubState.RUNNING);
        aliveFsm.RegisterState(AliveSubState.WALKING, OnBecomeWalking);
        aliveFsm.RegisterState(AliveSubState.RUNNING, OnBecomeRunning);

        advanceToRunningState = () => AdvanceToRunningState();
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        walkingSpeed = UnityEngine.Random.Range(defaultWalkingSpeed, defaultWalkingSpeed * 2);
        runningSpeed = UnityEngine.Random.Range(defaultRunningSpeed, defaultRunningSpeed * 2);

        aliveFsm.ForceEnterState(AliveSubState.WALKING);

        Invoke(advanceToRunningState.GetMethodName(), delayBeforeRun);

        mySpriteAnimator.Update();
    }

    protected void OnBecomeWalking(object param)
    {
        currentSpeed = walkingSpeed;
        
        myAnimation["Walk"].speed = walkingSpeed / defaultWalkingSpeed;
        myAnimation.PlayImmediately("Walk");
    }

    protected void OnBecomeRunning(object param)
    {
        currentSpeed = runningSpeed;

        myAnimation["Run"].speed = runningSpeed / defaultRunningSpeed;
        myAnimation.PlayImmediately("Run");
    }

    protected override void OnAlive()
    {
        base.OnAlive();

        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;

        aliveFsm.Update();
    }

    private void AdvanceToRunningState()
    {
        aliveFsm.Advance(AliveSubState.RUNNING);
    }

    protected override void OnBecomeDying(object param)
    {
        base.OnBecomeDying(param);

        CancelInvoke();

        friction = currentSpeed;
    }

    protected override void OnDying()
    {
        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;

        currentSpeed = Mathf.Max(0.0f, currentSpeed - friction * Time.deltaTime);

        if (currentSpeed == 0.0f) Advance(State.Dead);
    }
}
