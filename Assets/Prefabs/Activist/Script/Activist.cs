using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class Activist : Creature2 {
    public enum AliveState {
        RUNNING, START_RUNNING_WITHOUT_SEAL, RUNNING_WITHOUT_SEAL
    }

    enum DropDownState {
        NONE, IN_PROGRESS, DONE
    }

    public GameObject sealPrefab;

    public AudioClip[] soundsOfDyingSeal1;
    public AudioClip[] soundsOfDyingSeal2;

    private FSM<AliveState> aliveState;

    // XXX: Flag to make activist temporary immortal not to kill him right after
    // he drop the seal to test it.
    private bool immortal;

    private DropDownState dropDownState;

    protected new void Awake()
    {
        base.Awake();

        aliveState = new FSM<AliveState>();
        aliveState.AllowTransitionChain(AliveState.RUNNING, AliveState.START_RUNNING_WITHOUT_SEAL, AliveState.RUNNING_WITHOUT_SEAL);
        aliveState.RegisterState(AliveState.RUNNING);
        aliveState.RegisterState(AliveState.START_RUNNING_WITHOUT_SEAL,
                                 Action_PlayAnimation("StartRunWithDeadSeal"),
                                 Action_AdvanceAfterAnimation(aliveState, AliveState.RUNNING_WITHOUT_SEAL));
        aliveState.RegisterState(AliveState.RUNNING_WITHOUT_SEAL, OnBecomeRunWithDeadSeal);
    }

    public override void OnTouch()
    {
        if (GetCurrentState() != State.Alive || immortal) return;

        switch (aliveState.GetCurrentState())
        {
            case AliveState.RUNNING:
                if (Random2.NextBool())
                {
                    if (Advance(State.Dying, Random2.RandomArrayElement("DieAndDropDown1", "DieAndDropDown2")))
                    {
                        dropDownState = DropDownState.IN_PROGRESS;
                    }
                }
                else
                {
                    if (aliveState.Advance(AliveState.START_RUNNING_WITHOUT_SEAL))
                    {
                        immortal = true;

                        AudioCenter.PlayRandomClipAtMainCamera(soundsOfDyingSeal1);
                        AudioCenter.PlayRandomClipAtMainCamera(soundsOfDyingSeal2);
                    }
                }
                break;
            default:
                Advance(State.Dying, Random2.RandomArrayElement("DieWithoutSeal1", "DieWithoutSeal2"));
                break;
        }
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        runningSpeed = UnityEngine.Random.Range(defaultRunningSpeed, defaultRunningSpeed * 2);
        currentSpeed = runningSpeed;

        aliveState.ForceEnterState(AliveState.RUNNING);

        myAnimation["Run"].speed = currentSpeed / defaultRunningSpeed;
        myAnimation.PlayImmediately("Run");

        mySpriteAnimator.Update();

        immortal = false;

        dropDownState = DropDownState.NONE;
    }

    protected void OnBecomeRunWithDeadSeal(object param)
    {
        myAnimation.Play("RunWithDeadSeal");

        immortal = false;
    }

    protected override void OnAlive()
    {
        aliveState.Update();

        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }

    protected override void OnDying()
    {
        base.OnDying();

        if (dropDownState == DropDownState.IN_PROGRESS)
        {
            int sheet = (int)mySpriteAnimator.sheet;

            if ((sheet == 3 || sheet == 4) && ((int)mySpriteAnimator.index >= 4))
            {
                Vector3 position = myParent.transform.position;
                position.x += 0.14f;

                GameObjectPool.Instance.Instantiate(sealPrefab, position, Quaternion.identity);

                dropDownState = DropDownState.DONE;
            }
        }
    }
}
