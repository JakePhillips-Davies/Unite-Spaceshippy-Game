using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampHandler : MonoBehaviour
{
    [SerializeField] private HingeJoint joint;

    public void FlipMotorDirection() {

        JointMotor motor = joint.motor;
        motor.targetVelocity = -motor.targetVelocity;

        joint.motor = motor;

    }
}
