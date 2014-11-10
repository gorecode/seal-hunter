using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Pinguin : Creature, ITouchable
{
    enum Alive_SubState { Walking, Sliding }

    private const int DEATH_HEADSHOT = 2;
    private const int DEATH_BODYSHOT = 1;

    public float walkingSpeed = 0.3f;
    public float currentSpeed;
    public float friction = 0.02f;
    public float delayBeforeSliding = 1;

    public AudioClip[] soundsOfDying;
    public AudioClip[] soundsOfSpawning;

    private FSM<Alive_SubState> aliveState;

    public void OnTouch()
    {
        if (!State.Alive.Equals(GetCurrentState())) return;

        switch (aliveState.GetCurrentState())
        {
            case Alive_SubState.Walking:
                Advance(State.Dying, Random2.NextBool() ? DEATH_BODYSHOT : DEATH_HEADSHOT);
                break;
            case Alive_SubState.Sliding:
                Advance(State.Dying, Random.Range(1, 3));
                break;
        }
    }

    public new void Awake()
    {
        base.Awake();

        RegisterState(State.Alive, OnBecomeAlive, OnContinueLiving);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);

        aliveState = new FSM<Alive_SubState>();
        aliveState.AllowTransitionChain(Alive_SubState.Walking, Alive_SubState.Sliding);
        aliveState.RegisterState(Alive_SubState.Walking, OnBecomeWalking);
        aliveState.RegisterState(Alive_SubState.Sliding, OnBecomeSliding);
    }

    public new void Start()
    {
        base.Start();

        SetDefaultState(State.Alive);
    }

    private void OnBecomeAlive(object param)
    {
        AudioClips.PlayRandomClipAtMainCamera(soundsOfSpawning);

        aliveState.SetDefaultState(Alive_SubState.Walking);
    }

    private void OnContinueLiving()
    {
        myParent.position += Vector3.right * currentSpeed * Time.deltaTime;

        aliveState.Update();
    }

    private void OnBecomeWalking(object param)
    {
        currentSpeed = walkingSpeed;

        StartCoroutine(StartSlidingAfterDelay(delayBeforeSliding));
    }

    private IEnumerator StartSlidingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (State.Alive.Equals(GetCurrentState())) aliveState.Advance(Alive_SubState.Sliding);

        yield return null;
    }

    private void OnBecomeSliding(object param)
    {
        currentSpeed = walkingSpeed * 2;

        myAnimator.SetBool("Sliding", true);
    }

    private void OnBecomeDying(object param)
    {
        this.RemovePhysics();

        System.Int32 deathAnimationId = (System.Int32)param;

        AudioClips.PlayRandomClipAtMainCamera(soundsOfDying);

        myAnimator.SetInteger("DeathAnimationId", deathAnimationId);
    }

    private void OnDying()
    {
        switch (aliveState.GetCurrentState())
        {
            case Alive_SubState.Walking:
                if (myAnimator.IsFinishedPlayingAnimationWithTag("Dying")) Advance(State.Dead);
                break;
            case Alive_SubState.Sliding:
                myParent.position += Vector3.right * currentSpeed * Time.deltaTime;

                currentSpeed = Mathf.Max(0.0f, currentSpeed - friction * Time.deltaTime);

                if (currentSpeed == 0.0f) Advance(State.Dead);
                Debug.Log("s="+currentSpeed);
                break;
        }
    }

    private void OnBecomeDead(object param)
    {
        EventBus.EnemyDied.Publish(gameObject);
    }
}
