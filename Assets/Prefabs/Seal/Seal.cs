using UnityEngine;
using UnityEngineExt;
using System.Collections;
using Assets.Scripts;

public class Seal : FSM<Seal.State>, ITouchable
{
    public const int ANIM_DYING_BY_HEADSHOT = 2;
    public const int ANIM_DYING_BY_BODYSHOT = 1;
    public const int ANIM_DYING_WHILE_CRAWLING = 1;

    public enum State { Alive, Dying, Dead, Recycled }
    public enum Alive_SubState { Walking, Falling, Crawling };

    public float maximumSpeed;
    public float currentSpeed;

    public AudioClip[] soundsOfDying;
    public AudioClip[] soundsOfFalling;

    private SpriteRenderer mySpriteRenderer;
    private Animator myAnimator;
    private Transform myParent;

    private FSM<Alive_SubState> aliveState;

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myParent = transform.parent;

        // General mob lifecycle.
        RegisterState(State.Alive, onUpdate: OnLiving);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);

        AllowTransitionChain(State.Alive, State.Dying, State.Dead, State.Recycled);

        SetDefaultState(State.Alive);

        // Seal living lifecycle.
        aliveState = new FSM<Alive_SubState>();

        aliveState.RegisterState(Alive_SubState.Walking, OnBecomeWalking);
        aliveState.RegisterState(Alive_SubState.Falling, OnBecomeFalling);
        aliveState.RegisterState(Alive_SubState.Crawling, OnBecomeCrawling);

        aliveState.AllowTransitionChain(Alive_SubState.Walking, Alive_SubState.Falling, Alive_SubState.Crawling);

        aliveState.SetDefaultState(Alive_SubState.Walking);

        currentSpeed = maximumSpeed;
    }

    public void OnTouch()
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
                    int animatorParamValue = Random2.NextBool() ? ANIM_DYING_BY_BODYSHOT : ANIM_DYING_BY_HEADSHOT;
                    
                    Advance(State.Dying, animatorParamValue);
                }
                break;
            case Alive_SubState.Crawling:
                Advance(State.Dying, ANIM_DYING_WHILE_CRAWLING);
                break;
        }
    }

    /// <summary>
    /// Called from the StartCrawl animation clip.
    /// </summary>
    public void BecomeCrawling()
    {
        aliveState.Advance(Alive_SubState.Crawling);
    }

    private void OnBecomeWalking(object param)
    {
        currentSpeed = maximumSpeed;
    }

    private void OnLiving()
    {
        myParent.position += Vector3.right * currentSpeed * Time.deltaTime;

        aliveState.Update();
    }

    private void OnBecomeFalling(object param)
    {
        currentSpeed = 0;
        
        AudioClips.PlayRandomClipAtMainCamera(soundsOfFalling);

        myAnimator.SetBool("Crawling", true);
    }

    private void OnBecomeCrawling(object param)
    {
        currentSpeed = maximumSpeed / 2;
    }

    private void OnBecomeDying(object param)
    {
        this.RemovePhysics();

        System.Int32 animatorParameter = (System.Int32)param;

        AudioClips.PlayRandomClipAtMainCamera(soundsOfDying);

        mySpriteRenderer.sortingLayerID = Layers.BACKGROUND;

        myAnimator.SetInteger("Dying", animatorParameter);
    }

    private void OnDying()
    {
        AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);

        if (info.IsTag("dying") && info.normalizedTime >= 1.0f) Advance(State.Dead);
    }

    private void OnBecomeDead(object param)
    {
        EventBus.EnemyDied.Publish(gameObject);

        Advance(State.Recycled);
    }
}
