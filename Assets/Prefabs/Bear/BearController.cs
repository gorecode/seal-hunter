using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class BearController : Creature
{
    private const string ANIM_WALKING = "Base.Walk";
    private const string ANIM_DEATH_FROM_EYESHOT = "Base.EyeShot";
    private const string ANIM_DEATH_FROM_BODYSHOT = "Base.HeadExplosion";

    public float walkingSpeed = 0.01f;
    public float currentSpeed;

    public AudioClip[] soundsOfDying;

    public override void OnTouch()
    {
        Advance(State.Dying, Random2.NextBool() ? ANIM_DEATH_FROM_EYESHOT : ANIM_DEATH_FROM_BODYSHOT);
    }

    public new void Awake()
    {
        base.Awake();

        RegisterState(State.Alive, OnBecomeAlive);
        RegisterState(State.Dying, OnBecomeDying);
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

        myAnimator.Play(ANIM_WALKING, 0, 0);

        transform.position = Vector3.zero;

        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;

        StartCoroutine(Wait_Sniff_Walk_Coroutine());
    }

    private void OnBecomeDying(object param)
    {
        StopAllCoroutines();

        string animatorStateName = param as string;

        AudioClip audioClip = ANIM_DEATH_FROM_EYESHOT.Equals(animatorStateName) ? soundsOfDying[0] : soundsOfDying[1];

        AudioCenter.PlayClipAtMainCamera(audioClip);

        myAnimator.Play(animatorStateName, 0, 0);

        mySpriteRenderer.sortingLayerID = SortingLayer.BACKGROUND;
        
        collider2D.enabled = false;

        EventBus.OnBecomeDying(myParent.gameObject);
    }

    private void OnBecomeDead(object param)
    {
        EventBus.OnBecomeDead(transform.parent.gameObject);
    }

    new void Update()
    {
        base.Update();

        switch (GetCurrentState())
        {
            case State.Alive:
                transform.parent.position += Vector3.right * currentSpeed * Time.deltaTime;

                myAnimator.SetFloat("Speed", currentSpeed);

                break;
            case State.Dying:
                if (myAnimator.IsFinishedPlayingAnimationWithTag("Dying")) Advance(State.Dead);
                break;

        }
    }

    IEnumerator Wait_Sniff_Walk_Coroutine()
    {
        yield return new WaitForSeconds(1 + Random.value * 4);
        float oldSpeed = currentSpeed;
        currentSpeed = 0.0f;
        yield return new WaitForSeconds(1 + Random.value * 3);
        currentSpeed = oldSpeed;
        yield return null;
    }
}
