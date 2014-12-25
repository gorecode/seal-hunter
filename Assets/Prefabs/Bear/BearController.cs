using UnityEngine;
using UnityEngineExt;
using System.Collections;

public class BearController : Creature2
{
    public override void OnTouch()
    {
        Advance(State.Dying, Random2.RandomArrayElement("HeadExplosion", "Eyeshot"));
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
