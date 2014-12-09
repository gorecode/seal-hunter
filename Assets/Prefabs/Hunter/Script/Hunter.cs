using UnityEngine;
using System.Collections;

public class Hunter : Creature2 {
    public enum AliveState {
        IDLE, WALKING
    }

    public EasyJoystick joystick;
    public EasyButton fireButton;

    private Rigidbody2D rbody;
    private FSM<AliveState> aliveState;

    public new void Awake()
    {
        base.Awake();

        rbody = myParent.rigidbody2D;

        aliveState = new FSM<AliveState>();
        aliveState.AllowTransitionChain(AliveState.IDLE, AliveState.WALKING, AliveState.IDLE);
        aliveState.RegisterState(AliveState.IDLE, OnBecomeIdle);
        aliveState.RegisterState(AliveState.WALKING, OnBecomeWalking, OnWalking);
    }

    void FixedUpdate()
    {
        if (HandleMoveInput())
            aliveState.Advance(AliveState.WALKING);
        else
            aliveState.Advance(AliveState.IDLE);

        HandleFireInput();
    }

    private void HandleFireInput()
    {
        if (fireButton.buttonState == EasyButton.ButtonState.Down || fireButton.buttonState == EasyButton.ButtonState.Press)
        {
            Gun gun = myParent.gameObject.GetComponentInChildren<Gun>();

            gun.Fire();
        }
    }

    private bool HandleMoveInput()
    {
        float f = walkingSpeed;

        bool result = false;

        Vector2 joystickValue = joystick.JoystickAxis;

        if (Input.GetKey(KeyCode.A))
        {
            rbody.AddForce(-Vector2.right * f);
            result = true;
        } else if (Input.GetKey(KeyCode.D))
        {
            rbody.AddForce(Vector2.right * f);
            result = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            rbody.AddForce(Vector2.up * f);
            result = true;
        } else if (Input.GetKey(KeyCode.S))
        {
            rbody.AddForce(-Vector2.up * f);
            result = true;
        }

        if (!result)
        {
            if (Mathf.Abs(joystickValue.x) + Mathf.Abs(joystickValue.y) > 0.0f)
            {
                rbody.AddForce(joystickValue * f);
                result = true;
            }
        }

        return result;
    }

    protected override void OnBecomeAlive(object param)
    {
        base.OnBecomeAlive(param);

        mySpriteRenderer.sortingLayerID = SortingLayer.PLAYER;

        aliveState.ForceEnterState(AliveState.IDLE);
    }

    protected override void OnAlive()
    {
        base.OnAlive();

        aliveState.Update();

        if ((Mathf.Abs(rbody.velocity.x) + Mathf.Abs(rbody.velocity.y)) >= 0.05f)
        {
            aliveState.Advance(AliveState.WALKING);
        }
        else
        {
            aliveState.Advance(AliveState.IDLE);
        }
    }

    private void OnBecomeIdle(object param)
    {
        myAnimation.Stop();

        mySpriteAnimator.sheet = 0;
        mySpriteAnimator.index = 0;
    }

    private void OnBecomeWalking(object param)
    {
        myAnimation.Play("Walk");
    }

    private void OnWalking()
    {
        myAnimation["Walk"].speed = Mathf.Max(Mathf.Abs(rbody.velocity.x), Mathf.Abs(rbody.velocity.y)) * 1.5f;
    }
}
