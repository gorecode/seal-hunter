using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Transition<S> : System.IEquatable<Transition<S>>
{
    private S mCurrent;
    private S mNext;
    
    public Transition() { ; }
    public Transition(S current, S next)
    {
        mCurrent = current;
        mNext = next;
    }
    
    public bool Equals(Transition<S> other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return mCurrent.Equals(other.GetAState()) && mNext.Equals(other.GetBState());
    }
    
    public override int GetHashCode()
    {
        if((mCurrent == null || mNext == null))
            return 0;
        
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + mCurrent.GetHashCode();
            hash = hash * 23 + mNext.GetHashCode();
            return hash;
        }
    }
    
    public S GetAState() { return mCurrent; }
    public S GetBState() { return mNext; }
}

public delegate void OnEnter(object arg);
public delegate void OnUpdate();
public delegate void OnExit();

public class FSM<S>
{
    class StateDelegates {
        public OnEnter onEnter;
        public OnUpdate onUpdate;
        public OnExit onExit;
    }

    private bool debug;

    private S mCurrState;

    private HashSet<Transition<S>> mTransitions;
    private Dictionary<S, StateDelegates> mDelegates;

    public FSM() 
    {
        mTransitions = new HashSet<Transition<S>>();
        mDelegates = new Dictionary<S, StateDelegates>();
    }

    public void SetDebugOutput(bool debug)
    {
        this.debug = debug;
    }

    public void ForceEnterState(S s)
    {
        mCurrState = s;

        StateDelegates delegates = null;

        if (debug) Debug.Log("ForceEnterState(" + s + ")");

        if (mDelegates.TryGetValue(mCurrState, out delegates) && delegates.onEnter != null)
        {
            delegates.onEnter(null);
        }
    }

    public void Advance(S nextState, object transitionParam = null)
    {
        if (mCurrState.Equals(nextState)) return;

        Transition<S> transition = new Transition<S>(mCurrState, nextState);

        if (!mTransitions.Contains(transition))
        {
            Debug.Log("[FSM] Cannot advance to " + nextState + " state, current state is " + mCurrState);
            return;
        }

        StateDelegates oldStateDelegates = null;
        StateDelegates newStateDelegates = null;

        mDelegates.TryGetValue(mCurrState, out oldStateDelegates);
        mDelegates.TryGetValue(nextState, out newStateDelegates);

        if (oldStateDelegates != null && oldStateDelegates.onExit != null)
        {
            oldStateDelegates.onExit();
        }

        mCurrState = nextState;

        if (debug) Debug.Log("EnterState(" + nextState + ")");

        if (newStateDelegates != null && newStateDelegates.onEnter != null)
        {
            newStateDelegates.onEnter(transitionParam);
        }
    }

    public void RegisterState(S state, OnEnter onEnter = null, OnUpdate onUpdate = null, OnExit onExit = null)
    {
        StateDelegates delegates = new StateDelegates();
        
        delegates.onEnter = onEnter;
        delegates.onUpdate = onUpdate;
        delegates.onExit = onExit;

        mDelegates.Add(state, delegates);
    }

    public void AllowTransitionChain(params S[] states)
    {
        for (int i = 0; i < states.Length - 1; i++)
        {
            AllowTransition(states[i], states[i + 1]);
        }
    }

    public void AllowTransition(S init, S end)
    {
        Transition<S> tr = new Transition<S>(init, end);

        if (mTransitions.Contains(tr))
        {
            Debug.Log("[FSM] Transition: " + tr.GetAState() + " - " + tr.GetBState() + " already exists.");
            return;
        }


        mTransitions.Add(tr);
    }

    public void Update()
    {
        if (mCurrState == null) return;

        StateDelegates delegates = null;

        if (mDelegates.TryGetValue(mCurrState, out delegates) && delegates.onUpdate != null)
        {
            delegates.onUpdate();
        }
    }

    public S GetCurrentState() { return mCurrState; }
}
