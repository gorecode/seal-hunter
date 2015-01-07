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

public class FSMState {
    public event OnEnter onEnter;
    public event OnUpdate onUpdate;
    public event OnExit onExit;
    
    public void InvokeOnEnter(object param)
    {
        if (onEnter != null) onEnter.Invoke(param);
    }
    
    public void InvokeOnUpdate()
    {
        if (onUpdate != null) onUpdate.Invoke();
    }
    
    public void InvokeOnExit()
    {
        if (onExit != null) onExit.Invoke();
    }
}

public class FSM<S>
{
    private bool debug;

    private S mCurrState;

    private HashSet<Transition<S>> mTransitions;
    private Dictionary<S, FSMState> mDelegates;

    public FSM() 
    {
        mTransitions = new HashSet<Transition<S>>();
        mDelegates = new Dictionary<S, FSMState>();
    }

    public void SetDebugOutput(bool debug)
    {
        this.debug = debug;
    }

    public void ForceEnterState(S s)
    {
        mCurrState = s;

        FSMState delegates = null;

        if (debug) Debug.Log("ForceEnterState(" + s + ")");

        if (mDelegates.TryGetValue(mCurrState, out delegates))
        {
            delegates.InvokeOnEnter(null);
        }
    }

    public bool Advance(S nextState, object transitionParam = null)
    {
        if (mCurrState.Equals(nextState)) return false;

        Transition<S> transition = new Transition<S>(mCurrState, nextState);

        if (!mTransitions.Contains(transition))
        {
            if (debug) Debug.Log("[FSM] Cannot advance to " + nextState + " state, current state is " + mCurrState);
            return false;
        }

        FSMState oldStateDelegates = null;
        FSMState newStateDelegates = null;

        mDelegates.TryGetValue(mCurrState, out oldStateDelegates);
        mDelegates.TryGetValue(nextState, out newStateDelegates);

        if (oldStateDelegates != null)
        {
            oldStateDelegates.InvokeOnExit();
        }

        mCurrState = nextState;

        if (debug) Debug.Log("EnterState(" + nextState + ")");

        if (newStateDelegates != null)
        {
            newStateDelegates.InvokeOnEnter(transitionParam);
        }

        return true;
    }

    public FSMState RegisterState(S state, OnEnter onEnter = null, OnUpdate onUpdate = null, OnExit onExit = null)
    {
        FSMState delegates = new FSMState();

        if (onEnter != null)
            delegates.onEnter += onEnter;
        if (onUpdate != null)
            delegates.onUpdate += onUpdate;
        if (onExit != null)
            delegates.onExit += onExit;

        mDelegates.Add(state, delegates);

        return delegates;
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

        FSMState delegates = null;

        if (mDelegates.TryGetValue(mCurrState, out delegates))
        {
            delegates.InvokeOnUpdate();
        }
    }

    public S GetCurrentState() { return mCurrState; }
}
