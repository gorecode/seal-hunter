using UnityEngine;
using System.Collections;

public class Creature2 : Creature {
    public float maxHealth;
    public float maxHealthIncrementByLevel;

    public float currentSpeed;
    public float walkingSpeed;
    public float runningSpeed;
    public float defaultWalkingSpeed;
    public float defaultRunningSpeed;

    public AudioClip[] soundsOfRessurection;
    public AudioClip[] soundsOfDying;

    protected float health;

    protected Level currentLevel;

    public void Kill()
    {
        Damage(health);
    }

    public virtual void Damage(float damage)
    {
        health -= damage;
    }

    protected new void Awake()
    {
        base.Awake();

        RegisterState(State.Alive, OnBecomeAlive, OnAlive);
        RegisterState(State.Dying, OnBecomeDying, OnDying);
        RegisterState(State.Dead, OnBecomeDead);
		RegisterState(State.Recycled, OnBecomeRecycled);
    }

    protected void Start()
    {
        currentLevel = PrefabLocator.INSTANCE.gameObject.GetComponent<Level>();
    }

    protected virtual void OnBecomeAlive(object param)
    {
        health = maxHealth + 1 * maxHealthIncrementByLevel;

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

        if (param != null) myAnimation.Play((string)param);
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
