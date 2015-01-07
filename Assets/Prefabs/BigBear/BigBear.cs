using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class BigBear : Creature2 {
    public float friction;

    private Action BecomeRunningAction;

    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (health > 0) return;

        Advance(State.Dying, "Die");
    }

    protected new void Awake()
    {
        base.Awake();

        BecomeRunningAction = () => BecomeRunning();
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        walkingSpeed = UnityEngine.Random.Range(defaultWalkingSpeed, defaultWalkingSpeed * 2);
        runningSpeed = UnityEngine.Random.Range(defaultRunningSpeed, defaultRunningSpeed * 2);

        currentSpeed = walkingSpeed;

        myAnimation["Walk"].speed = walkingSpeed / defaultWalkingSpeed;
        myAnimation.PlayImmediately("Walk");

        Invoke(BecomeRunningAction.GetMethodName(), 4.0f);

        mySpriteAnimator.Update();
    }

    protected override void OnAlive()
    {
        base.OnAlive();

        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }

    private void BecomeRunning()
    {
        myAnimation["Run"].speed = runningSpeed / defaultRunningSpeed;
        myAnimation.PlayImmediately("Run");

        currentSpeed = runningSpeed;
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
