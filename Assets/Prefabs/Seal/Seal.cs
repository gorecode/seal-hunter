using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Seal : Creature2
{
    public enum Alive_SubState { Walking, Falling, Crawling };

    public AudioClip[] soundsOfFalling;

    private FSM<Alive_SubState> aliveState;

    public override MobType GetMobType()
    {
        return MobType.Seal;
    }

    public new void Awake()
    {
        base.Awake();

        aliveState = new FSM<Alive_SubState>();
        aliveState.AllowTransitionChain(Alive_SubState.Walking, Alive_SubState.Falling, Alive_SubState.Crawling);

        aliveState.RegisterState(Alive_SubState.Walking, OnBecomeWalking);
        aliveState.RegisterState(Alive_SubState.Falling, OnBecomeFalling, Action_AdvanceAfterAnimation(aliveState, Alive_SubState.Crawling));
        aliveState.RegisterState(Alive_SubState.Crawling, OnBecomeCrawling);
    }

    public Alive_SubState GetAliveState()
    {
        return aliveState.GetCurrentState();
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (!State.Alive.Equals(GetCurrentState())) return;

        if (health > 0 && health < maxHealth / 2)
        {
            aliveState.Advance(Alive_SubState.Falling);
        }
        else if (health <= 0)
        {
            switch (aliveState.GetCurrentState())
            {
                case Alive_SubState.Walking:
                    Advance(State.Dying, Random2.RandomArrayElement("DieByHeadshot", "DieFalling"));
                    break;
                case Alive_SubState.Crawling:
                case Alive_SubState.Falling:
                    Advance(State.Dying, "DieCrawling");
                    break;
            }
        }
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        moveDir = Vector3.right;

        if (ServiceLocator.current.singlePlayerGame.GetLevelIndex() > 0 && Random.value <= 0.7f)
        {
            moveStrategy = MoveStrategy.SIN;

            sinMoveHz = Random.value;
            sinMoveAmp = Random.Range(0.05f, 0.1f);

            moveDir = moveDir.ToVector2().Rotate(-moveDirMaxYVariation + Random.value * moveDirMaxYVariation * 2f);
        }
        else
        {
            moveStrategy = MoveStrategy.FORWARD;
        }

        aliveState.ForceEnterState(Alive_SubState.Walking);
    }

    protected override void OnAlive()
    {
        aliveState.Update();

        UpdateDefaultMovement();
    }

    private void OnBecomeWalking(object param)
    {
        walkingSpeed = Random.Range(defaultWalkingSpeed, defaultWalkingSpeed * 2);

        myAnimation["Walk"].speed = walkingSpeed / defaultWalkingSpeed;
        myAnimation.PlayImmediately("Walk");

        mySpriteAnimator.Update();

        currentSpeed = walkingSpeed;
    }

    private void OnBecomeFalling(object param)
    {
        moveStrategy = MoveStrategy.FORWARD;

        currentSpeed = 0;
        
        AudioCenter.PlayRandomClipAtMainCamera(soundsOfFalling);

        myAnimation.Play("FallToCrawl");
    }

    private void OnBecomeCrawling(object param)
    {
        currentSpeed = walkingSpeed / 2;

        myAnimation.Play("Crawl");
    }
}
