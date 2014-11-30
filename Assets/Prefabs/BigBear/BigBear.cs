using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class BigBear : Creature {
    public float walkingSpeed = 0.5f;
    public float runningSpeed = 0.10f;
    public float currentSpeed;

    public AudioClip[] soundsOfRessurection;
    public AudioClip[] soundsOfDying;

    public float friction;

    private Action BecomeRunningAction;

    public override void OnTouch()
    {
        base.OnTouch();

        Advance(State.Dying);
    }

    public new void Awake()
    {
        base.Awake();

        RegisterState(State.Alive, OnBecomeAlive, OnLiving);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);

        BecomeRunningAction = () => BecomeRunning();
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
        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;

        AudioCenter.PlayRandomClipAtMainCamera(soundsOfRessurection);

        Invoke(BecomeRunningAction.GetMethodName(), 4.0f);
    }

    private void OnLiving()
    {
        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }

    private void BecomeRunning()
    {
        myAnimation.Play("Run");

        currentSpeed = runningSpeed;
    }

    private void OnBecomeDying(object param)
    {
        CancelInvoke();

        AudioCenter.PlayRandomClipAtMainCamera(soundsOfDying);

        friction = currentSpeed;

        myAnimation.Play("Die");

        collider2D.enabled = false;

        mySpriteRenderer.sortingLayerID = SortingLayer.BACKGROUND;

        EventBus.OnBecomeDying(myParent.gameObject);
    }

    private void OnDying()
    {
        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;

        currentSpeed = Mathf.Max(0.0f, currentSpeed - friction * Time.deltaTime);

        if (currentSpeed == 0.0f) Advance(State.Dead);
    }

    private void OnBecomeDead(object param)
    {
        mySpriteRenderer.sprite = null;

        EventBus.OnBecomeDead(myParent.gameObject);
    }
}
