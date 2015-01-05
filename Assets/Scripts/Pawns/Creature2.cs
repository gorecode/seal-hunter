using UnityEngine;
using System.Collections;

public class Creature2 : Creature {
    public float currentSpeed;
    public float walkingSpeed;
    public float runningSpeed;
    public float defaultWalkingSpeed;
    public float defaultRunningSpeed;

    public AudioClip[] soundsOfRessurection;
    public AudioClip[] soundsOfDying;

    protected new void Awake()
    {
        base.Awake();

        RegisterState(State.Alive, OnBecomeAlive, OnAlive);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);
		RegisterState(State.Recycled, OnBecomeRecycled);
    }

    protected void OnEnable()
    {
        ForceEnterState(State.Alive);
    }

    protected virtual void OnBecomeAlive(object param)
    {
        collider2D.enabled = true;
        
        mySpriteRenderer.sortingLayerID = SortingLayer.FOREGROUND;
        
        AudioCenter.PlayRandomClipAtMainCamera(soundsOfRessurection);
    }

    protected virtual void OnAlive()
    {
    }

    protected virtual void OnBecomeDying(object param)
    {
        EventBus.OnBecomeDying(myParent.gameObject);

        collider2D.enabled = false;
        
        mySpriteRenderer.sortingLayerID = SortingLayer.BACKGROUND;

        AudioCenter.PlayRandomClipAtMainCamera(soundsOfDying);

        myAnimation.Play((string)param);
    }

    protected virtual void OnDying()
    {
        if (!myAnimation.isPlaying)
        {
            Advance(State.Dead);
        }
    }

    protected virtual void OnBecomeDead(object param)
    {
        EventBus.OnBecomeDead(myParent.gameObject);
    }

	protected virtual void OnBecomeRecycled(object param)
	{
	}
}
