using UnityEngine;
using System.Collections;

public class FSMBehaviour<S> : SSBehaviour {
    protected readonly FSM<S> fsm = new FSM<S>();

    public S readonlyState;

    public void RegisterState(S state, OnEnter onEnter = null, OnUpdate onUpdate = null, OnExit onExit = null)
    {
        fsm.RegisterState(state, onEnter, onUpdate, onExit);
    }

    public void AllowTransitionChain(params S[] states)
    {
        fsm.AllowTransitionChain(states);
    }

    public void ForceEnterState(S s)
    {
        fsm.ForceEnterState(s);
    }

    public void AllowTransition(S from, S to)
    {
        fsm.AllowTransition(from, to);
    }

    public bool Advance(S state, object param = null)
    {
        return fsm.Advance(state, param);
    }

    public void Update()
    {
        readonlyState = GetCurrentState();

        fsm.Update();
    }

    public S GetCurrentState()
    {
        return fsm.GetCurrentState();
    }
}
