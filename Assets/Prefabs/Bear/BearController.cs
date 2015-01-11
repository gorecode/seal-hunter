using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class BearController : Creature2
{
    public override void Damage(float damage)
    {
        base.Damage(damage);

        if (health <= 0) Advance(State.Dying, Random2.RandomArrayElement("HeadExplosion", "Eyeshot"));
    }

    public override MobType GetMobType()
    {
        return MobType.Bear;
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        currentSpeed = walkingSpeed;

        myAnimation.PlayImmediately("Walk");

        mySpriteAnimator.Update();

        StartCoroutine(Wait_Sniff_Walk_Coroutine());
    }

    protected override void OnBecomeDying(object param)
    {
        base.OnBecomeDying(param);

        StopAllCoroutines();
    }

    protected override void OnAlive()
    {
        transform.parent.position += Vector3.right * currentSpeed * Time.deltaTime;

        if (currentSpeed == 0f) health = Mathf.Min(initialHealth, health + Time.deltaTime * initialHealth * 0.25f);
    }

    IEnumerator Wait_Sniff_Walk_Coroutine()
    {
        yield return new WaitForSeconds(1 + Random.value * 4);
        float oldSpeed = currentSpeed;
        currentSpeed = 0.0f;
        myAnimation.Play("Sniff");
        yield return new WaitForSeconds(1 + Random.value * 3);
        currentSpeed = oldSpeed;
        myAnimation.Play("Walk");
        yield return null;
    }
}
