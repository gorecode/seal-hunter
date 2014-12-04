using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Tortoise : Creature {
    public enum AliveSubState {
        WALKING, HIDDING, HIDDEN, APPEARING
    }

    public float walkingSpeed = 0.5f;
    public float currentSpeed;

    public AudioClip[] soundsOfRessurection;
    public AudioClip[] soundsOfDying;

    private static string[] DYING_ANIMATIONS = { "Die1", "Die2" };
    private FSM<AliveSubState> aliveSubState;

    public override void OnTouch()
    {
        base.OnTouch();
        
        Advance(State.Dying, Random2.RandomArrayElement(DYING_ANIMATIONS));
    }

    public new void Awake()
    {
        base.Awake();

        aliveSubState = new FSM<AliveSubState>();
        aliveSubState.AllowTransitionChain(AliveSubState.WALKING, AliveSubState.HIDDING, AliveSubState.HIDDEN, AliveSubState.APPEARING, AliveSubState.WALKING);

        RegisterState(State.Alive, OnBecomeAlive, OnLiving);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);
    }

    void OnEnable()
    {
        ForceEnterState(State.Alive);
    }

    private void OnBecomeAlive(object param)
    {
        collider2D.enabled = true;
        
        currentSpeed = walkingSpeed;
        
        myAnimation.Play("Walk");

        gameObject.SampleAnimation(myAnimation["Walk"].clip, 0f);

        SpriteAnimator sa = GetComponent<SpriteAnimator>();

        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;

        AudioCenter.PlayRandomClipAtMainCamera(soundsOfRessurection);
    }

    private void OnLiving()
    {
        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }

    private void OnBecomeDying(object param)
    {
        myAnimation.Play((string)param);

        collider2D.enabled = false;
        
        mySpriteRenderer.sortingLayerID = SortingLayer.BACKGROUND;
        
        EventBus.OnBecomeDying(myParent.gameObject);
    }

    private void OnDying()
    {
        if (!myAnimation.isPlaying) {
            Advance(State.Dead);
        }
    }

    private void OnBecomeDead(object param)
    {
        myAnimation.Stop();

        EventBus.OnBecomeDead(myParent.gameObject);
    }
}
