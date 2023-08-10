using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StepSide
{
    Left,
    Right
}


public enum WheelPosition
{
    FrontLeft,
    FrontRight,
    RearLeft,
    RearRight
}


public class CartController : MonoBehaviour
{

    public float stepStrength;
    public float stepTime;
    public float rollingFriction, slidingFriction;
    public float casterSwivelSpeed;
    public float casterOffset;
    public Vector3 leftCasterDirection, rightCasterDirection;
    public float turningForce;

    public GameObject leftHandPos, rightHandPos;
    public GameObject frontLeftWheelPos, frontRightWheelPos, rearLeftWheelPos, rearRightWheelPos;
    public Rigidbody rb;

    StepSide currentSide = StepSide.Left;
    float stepTimer = 0;

    public ParticleSystem pushAnimVfx;

    private void Start()
    {
        leftCasterDirection = transform.forward;
        rightCasterDirection = transform.forward;
    }

    float GetWeightOnWheel(WheelPosition wheel)
    {
        // take into account wheel not being on the ground, or unequal distribution of weight on cart.
        return rb.mass / 4;
    }


    Vector3 GetWheelPosition(WheelPosition wheel)
    {
        if (wheel == WheelPosition.FrontLeft)
        {
            return frontLeftWheelPos.transform.position - leftCasterDirection * casterOffset;

        }
        if (wheel == WheelPosition.FrontRight)
        {
            return frontRightWheelPos.transform.position - rightCasterDirection * casterOffset;

        }

        if (wheel == WheelPosition.RearLeft)
            return rearLeftWheelPos.transform.position;
        if (wheel == WheelPosition.RearRight)
            return rearRightWheelPos.transform.position;

        return Vector3.zero;
    }

    void ApplyWheelFriction(WheelPosition wheel)
    {
        
        // rear wheels are always aligned in the direction of the forward vector of the cart.
        if(wheel == WheelPosition.RearLeft || wheel == WheelPosition.RearRight)
        {
            var wheelPos = GetWheelPosition(wheel);
            var normalForce = GetWeightOnWheel(wheel);
            var wheelDirection = transform.forward;
            var slidingDirection = Vector3.Cross(transform.up, wheelDirection);
            var cartMomentum = rb.GetPointVelocity(wheelPos);
            var cartMomentumDir = cartMomentum.normalized;

            var velocityInRollingDirection = Vector3.Project(cartMomentum, wheelDirection);
            var velocityInSlidingDirection = Vector3.Project(cartMomentum, slidingDirection);

            var rollingFrictionForce = -velocityInRollingDirection.normalized * rollingFriction * normalForce;
            var slidingFrictionForce = -velocityInSlidingDirection.normalized * slidingFriction * normalForce;

            rb.AddForceAtPosition(rollingFrictionForce + slidingFrictionForce, wheelPos);
            Debug.DrawRay(wheelPos, rollingFrictionForce, Color.green, 1);
            Debug.DrawRay(wheelPos, slidingFrictionForce, Color.red, 1);
        } 
        else
        {
            // front wheels are aligned with a certain direciton because they are on casters.
            var wheelDirection = wheel == WheelPosition.FrontLeft ? leftCasterDirection : rightCasterDirection;
            var wheelPos = GetWheelPosition(wheel);
            var normalForce = GetWeightOnWheel(wheel);
            var slidingDirection = Vector3.Cross(transform.up, wheelDirection);
            var cartMomentum = rb.GetPointVelocity(wheelPos);
            var cartMomentumDir = cartMomentum.normalized;

            var velocityInRollingDirection = Vector3.Project(cartMomentum, wheelDirection);
            var velocityInSlidingDirection = Vector3.Project(cartMomentum, slidingDirection);

            var rollingFrictionForce = -velocityInRollingDirection.normalized * rollingFriction * normalForce;
            var slidingFrictionForce = -velocityInSlidingDirection.normalized * slidingFriction * normalForce;

            rb.AddForceAtPosition(rollingFrictionForce + slidingFrictionForce, wheelPos);
            Debug.DrawRay(wheelPos, rollingFrictionForce, Color.green, 1);
            Debug.DrawRay(wheelPos, slidingFrictionForce, Color.red, 1);


            // rotate casters in direction of cart momentum slightly
            if (wheel == WheelPosition.FrontLeft)
            {
                leftCasterDirection = Vector3.RotateTowards(leftCasterDirection, cartMomentumDir, casterSwivelSpeed, 1000);

            } 
            else
            {
                rightCasterDirection = Vector3.RotateTowards(rightCasterDirection, cartMomentumDir, casterSwivelSpeed, 1000);
            }

        }
    }

    void DoStep(StepSide side)
    {
        /*
        var turn = Input.GetAxis("Turning");
        // can have two ways to turn, apply friction to one side, or apply less force when stepping.

        if(side == StepSide.Left && turn < 0)
        {
            turn = Mathf.Abs(turn);
        }

        if (side == StepSide.Right && turn < 0)
        {
            turn = 0;
        }

        if (side == StepSide.Right && turn > 0)
        {
            turn = Mathf.Abs(turn);
        }

        if (side == StepSide.Left && turn > 0)
        {
            turn = 0;
        }

        */


        var forcePos = side == StepSide.Left ? leftHandPos.transform.position : rightHandPos.transform.position;
        var force = transform.forward * stepStrength;
        rb.AddForceAtPosition(force, forcePos, ForceMode.Impulse);


        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = forcePos;
        pushAnimVfx.Emit(emitParams, 1);
    }



    void ApplyTurning()
    {
        var turn = Input.GetAxis("Turning");
        rb.AddTorque(Vector3.up * turn * turningForce);
    }

    void FixedUpdate()
    {
        ApplyWheelFriction(WheelPosition.FrontLeft);
        ApplyWheelFriction(WheelPosition.FrontRight);
        ApplyWheelFriction(WheelPosition.RearLeft);
        ApplyWheelFriction(WheelPosition.RearRight);
        ApplyTurning();


        if (Input.GetButton("Step"))
        {
            stepTimer -= Time.fixedDeltaTime;

            if(stepTimer < 0)
            {
                DoStep(currentSide);
                stepTimer = stepTime;
                currentSide = currentSide == StepSide.Left ? StepSide.Right : StepSide.Left;

            }
        }
    }
}
