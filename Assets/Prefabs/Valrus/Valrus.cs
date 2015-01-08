using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class Valrus : Creature2 {
    private const int SHEET_RUSH_WITH_OPENED_MOUTH = 2;
    private const int SHEET_RUSH = 1;

    private Action runAction;
    private Action eatAction;

    public new void Awake()
    {
        base.Awake();

        runAction = () => StartRun();
        eatAction = () => OpenMouth();
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (health <= 0) Advance(State.Dying);
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        CalmDown();

        mySpriteAnimator.Update();
    }

    public void CalmDown()
    {
        if (GetCurrentState() != State.Alive) return;

        currentSpeed = walkingSpeed;

        animation.PlayImmediately("Walk");

        mySpriteAnimator.Update();

        CancelInvoke();

        Invoke(runAction.GetMethodName(), 2.0f);
    }

    protected override void OnAlive()
    {
        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }

    private void StartRun()
    {
        currentSpeed = runningSpeed;

        animation.Stop();

        mySpriteAnimator.index = 0;
        mySpriteAnimator.sheet = SHEET_RUSH;

        Invoke(eatAction.GetMethodName(), 0.5f);
    }

    private void OpenMouth()
    {
        mySpriteAnimator.index = 0;
        mySpriteAnimator.sheet = SHEET_RUSH_WITH_OPENED_MOUTH;
    }

    protected override void OnBecomeDying(object param)
    {
        base.OnBecomeDying(param);

        CancelInvoke();

        animation.Play("Die");
    }

    private bool IsMouthOpened()
    {
        return mySpriteAnimator.sheet == SHEET_RUSH_WITH_OPENED_MOUTH;
    }
}
