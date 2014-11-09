using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class SealLogic2 : MonoBehaviour {
    public enum State { Alive, Dying, Dead, Recycled }
    public enum AliveState { Walking, Falling, Crawling };

    public float maxSpeed;
    public float curSpeed;

    public AudioClip[] soundsOfDying;
    public AudioClip[] soundsOfFalling;

    private Animation myAnimation;
    private SpriteRenderer spriteRenderer;

    private FSM<State> fsm;
    private FSM<AliveState> fsmAlive;

    void Start()
    {
        myAnimation = GetComponent<Animation>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // main FSM.
        fsm = new FSM<State>();

        fsm.RegisterState(State.Dying, OnBecomeDying, OnDying, null);
        fsm.RegisterState(State.Dead, OnBecomeDead, null, null);
        fsm.RegisterState(State.Alive, null, OnLive, null);

        fsm.AddTransition(State.Alive, State.Dying);
        fsm.AddTransition(State.Dying, State.Dead);
        fsm.AddTransition(State.Dead, State.Recycled);

        fsm.Init(State.Alive);

        // alive substate FSM.
        fsmAlive = new FSM<AliveState>();

        fsmAlive.RegisterState(AliveState.Walking, OnBecomeWalking, null, null);
        fsmAlive.RegisterState(AliveState.Falling, OnBecomeFalling, OnFalling, null);

        fsmAlive.AddTransition(AliveState.Walking, AliveState.Falling);
        fsmAlive.AddTransition(AliveState.Falling, AliveState.Crawling);
        fsmAlive.Init(AliveState.Walking);

        curSpeed = maxSpeed;

        StartCoroutine(DieAfter2Sec());
    }

    public IEnumerator DieAfter2Sec()
    {
        yield return new WaitForSeconds(2);
        fsm.Advance(State.Dying, null);
        yield return null;
    }

    public void OnGetTouched()
    {
        if (!State.Alive.Equals(fsm.GetCurrentState())) return;

        // make random action - die or fall.
        if (Random.Range(0, 1) == 0)
        {
            fsm.Advance(State.Dying, null);
        } else
        {
            fsmAlive.Advance(AliveState.Falling, null);
        }
    }

    public void OnBecomeWalking(Object param)
    {
        myAnimation.Play("Walk");

        curSpeed = maxSpeed;
    }

    public void OnLive()
    {
        transform.position += Vector3.right * curSpeed * Time.deltaTime;

        fsmAlive.Update();
    }

    public void OnBecomeFalling(Object param)
    {
        curSpeed = 0;
        
        AudioClips.PlayRandomClipAtMainCamera(soundsOfFalling);
    }

    public void OnFalling()
    {
    }

    public void OnBecomeCrawling(Object param)
    {
        curSpeed = maxSpeed / 2;
    }

    public void OnBecomeDying(Object param)
    {
        AudioClips.PlayRandomClipAtMainCamera(soundsOfDying);

        spriteRenderer.sortingLayerID = Layers.BACKGROUND;

        myAnimation.CrossFade("DeathByBodyshot");

        Debug.Log("Seal Became Dying");
    }

    public void OnDying()
    {
        if (!myAnimation.isPlaying) fsm.Advance(State.Dead);
    }

    public void OnBecomeDead(Object param)
    {
        EventBus.EnemyDied.Publish(gameObject);

        fsm.Advance(State.Recycled);
    }

    public void Update()
    {
        fsm.Update();
    }
}
