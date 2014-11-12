using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Pinguin : Creature, ITouchable
{
    enum Alive_SubState { Walking, Sliding }

    private const int DEATH_HEADSHOT = 2;
    private const int DEATH_BODYSHOT = 1;

    private static readonly int WALKING_STATE_HASH = Animator.StringToHash("Base Layer.Walking");

    public float slidingSpeed;
    public float walkingSpeed = 0.3f;
    public float currentSpeed;
    public float friction = 0.02f;
    public float delayBeforeSliding = 1;

    public AudioClip[] soundsOfDying;
    public AudioClip[] soundsOfSpawning;

    private Vector3 direction = Vector3.right;

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

    public void OnEnable()
    {
        ForceEnterState(State.Alive);
    }

    private void OnBecomeAlive(object param)
    {
        myAnimator.speed = 1;
        myAnimator.SetInteger("DeathAnimationId", 0);
        myAnimator.SetBool("Sliding", false);

        direction = Vector3.right;

        collider2D.enabled = true;

        AudioClips.PlayRandomClipAtMainCamera(soundsOfSpawning);

        aliveState.ForceEnterState(Alive_SubState.Walking);
    }

    private void OnContinueLiving()
    {
        myParent.position += direction * currentSpeed * Time.deltaTime;

        aliveState.Update();
    }

    private void OnBecomeWalking(object param)
    {
        myAnimator.Play(WALKING_STATE_HASH, 0, 0);

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
        slidingSpeed = currentSpeed = walkingSpeed * 2;

        direction = Quaternion.Euler(0, 0, Random.Range(-30, 30)) * direction;
        direction.Normalize();

        myAnimator.SetBool("Sliding", true);
    }

    private void OnBecomeDying(object param)
    {
        StopAllCoroutines();

        collider2D.enabled = false;

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
                myParent.position += direction * currentSpeed * Time.deltaTime;

                currentSpeed = Mathf.Max(0.0f, currentSpeed - friction * Time.deltaTime);

                myAnimator.speed = 1.0f - (slidingSpeed - currentSpeed) / slidingSpeed;

                if (currentSpeed == 0.0f) Advance(State.Dead);

                break;
        }
    }

    private void OnBecomeDead(object param)
    {
        EventBus.OnDeath(transform.parent.gameObject);
    }
}
