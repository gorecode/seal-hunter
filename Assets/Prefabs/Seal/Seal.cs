using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Seal : Creature2
{
    public enum Alive_SubState { Walking, Falling, Crawling };

    public AudioClip[] soundsOfFalling;

    private FSM<Alive_SubState> aliveState;

    public new void Awake()
    {
        base.Awake();

        aliveState = new FSM<Alive_SubState>();
        aliveState.AllowTransitionChain(Alive_SubState.Walking, Alive_SubState.Falling, Alive_SubState.Crawling);

        aliveState.RegisterState(Alive_SubState.Walking, OnBecomeWalking);
        aliveState.RegisterState(Alive_SubState.Falling, OnBecomeFalling, delegateAdvanceAfterAnimation(aliveState, Alive_SubState.Crawling));
        aliveState.RegisterState(Alive_SubState.Crawling, OnBecomeCrawling);
    }

    public override void OnTouch()
    {
        if (!State.Alive.Equals(GetCurrentState())) return;

        switch (aliveState.GetCurrentState())
        {
            case Alive_SubState.Walking:
                if (Random2.NextBool())
                {
                    aliveState.Advance(Alive_SubState.Falling);
                } else
                {
                    Advance(State.Dying, Random2.RandomArrayElement("DieByHeadshot", "DieFalling"));
                }
                break;
            case Alive_SubState.Crawling:
            case Alive_SubState.Falling:
                Advance(State.Dying, "DieCrawling");
                break;
        }
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        aliveState.ForceEnterState(Alive_SubState.Walking);
    }

    protected override void OnAlive()
    {
        myParent.position += Vector3.right * currentSpeed * Time.deltaTime;
        
        aliveState.Update();
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
