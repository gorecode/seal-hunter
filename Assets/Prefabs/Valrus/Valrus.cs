using UnityEngine;
using UnityEngineExt;
using System.Collections;
using System;

public class Valrus : Creature {
    private const int SHEET_RUSH_WITH_OPENED_MOUTH = 2;
    private const int SHEET_RUSH = 1;

    public float currentSpeed;
    public float walkingSpeed = 0.3f;
    public float runningSpeed = 1.5f;

    private GameObject myRoot;

    private Action runAction;
    private Action eatAction;

    private SpriteAnimator mySpriteAnimator;

    private Transform mouth;

    public override void OnTouch()
    {
        base.OnTouch();
        
        if (GetCurrentState() == State.Alive) Advance(State.Dying);
    }

    public new void Awake()
    {
        base.Awake();

        mySpriteAnimator = GetComponent<SpriteAnimator>();

        myRoot = myParent.gameObject;

        RegisterState(State.Alive, OnBecomeAlive, Alive);
        RegisterState(State.Dying, OnBecomeDying, Dying);
        RegisterState(State.Dead, OnBecomeDead);

        runAction = () => StartRun();
        eatAction = () => OpenMouth();

        mouth = transform.FindChild("Mouth");
    }

    void OnEnable()
    {
        ForceEnterState(State.Alive);
    }

    private void OnBecomeAlive(object param)
    {
        collider2D.enabled = true;
        
        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;

        CalmDown();
    }

    public void CalmDown()
    {
        if (GetCurrentState() != State.Alive) return;

        currentSpeed = walkingSpeed;
        
        animation.PlayImmediately("Walk");

        CancelInvoke();

        Invoke(runAction.GetMethodName(), 2.0f);
        
        mouth.gameObject.SetActive(false);
    }

    private void Alive()
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

        mouth.gameObject.SetActive(true);
    }

    private void OnBecomeDying(object param)
    {
        mouth.gameObject.SetActive(false);

        CancelInvoke();

        animation.Play("Die");

        collider2D.enabled = false;
        
        mySpriteRenderer.sortingLayerID = SortingLayer.BACKGROUND;

        EventBus.OnBecomeDying(myRoot);
    }

    private void Dying()
    {
        if (!animation.isPlaying) Advance(State.Dead);
    }

    private void OnBecomeDead(object param)
    {
        EventBus.OnBecomeDead(myRoot);
    }

    private bool IsMouthOpened()
    {
        return mySpriteAnimator.sheet == SHEET_RUSH_WITH_OPENED_MOUTH;
    }
}
