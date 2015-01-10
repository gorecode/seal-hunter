using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class SealChild : Creature2 {
    protected new void Awake()
    {
        base.Awake();

        defaultWalkingSpeed = 0.2f;
    }

    public override MobType GetMobType()
    {
        return MobType.SealChild;
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (health <= 0) Advance(State.Dying, Random2.RandomArrayElement("Die1", "Die2"));
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        walkingSpeed = Random.Range(defaultWalkingSpeed, defaultWalkingSpeed + 0.5f);

        currentSpeed = walkingSpeed;

        myAnimation["Walk"].speed = walkingSpeed / defaultWalkingSpeed;
        myAnimation.PlayImmediately("Walk");

        mySpriteAnimator.Update();
    }

    protected override void OnAlive()
    {
        base.OnAlive();

        myParent.position += Vector3.right * Time.deltaTime * currentSpeed;
    }
}
