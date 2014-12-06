using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class SealChild : Creature2 {
    protected new void Awake()
    {
        base.Awake();

        defaultWalkingSpeed = 0.2f;
    }

    public override void OnTouch()
    {
        base.OnTouch();
        
        if (GetCurrentState() == State.Alive) Advance(State.Dying, Random2.RandomArrayElement("Die1", "Die2"));
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        walkingSpeed = Random.Range(defaultWalkingSpeed, defaultWalkingSpeed + 0.5f);

        currentSpeed = walkingSpeed;

        myAnimation["Walk"].speed = walkingSpeed / defaultWalkingSpeed;
        myAnimation.PlayImmediately("Walk");
    }

    protected override void OnAlive()
    {
        base.OnAlive();

        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }
}
