﻿using UnityEngine;
using System.Collections;

public class FSMBehaviour<S> : BaseBehaviour {
    protected readonly FSM<S> fsm = new FSM<S>();

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

    public void Advance(S state, object param = null)
    {
        fsm.Advance(state, param);
    }

    public void Update()
    {
        fsm.Update();
    }

    public S GetCurrentState()
    {
        return fsm.GetCurrentState();
    }
}