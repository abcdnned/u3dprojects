using System;
using UnityEngine;

public class HandSwingMove : HandMove
{
    internal Transform handTransform;

    internal CharacterJoint joint;

    internal CharacterJoint handJoint;

    internal Vector3 midDirection;

    internal Quaternion midDirectionOffset;


    public HandSwingMove() : base(MoveNameConstants.HandSwingMove)
    {
    }

    public void init(CharacterJoint joint,
                     CharacterJoint handJoint,
                     Transform handTransform, Vector3 midDirection, float duration) {
        this.duration = duration;
        this.joint = joint;
        this.handJoint = handJoint;
        this.handTransform = handTransform;
        this.midDirection = midDirection;
        Debug.DrawRay(joint.transform.position, midDirection, Color.black, 5);
        midDirectionOffset = Quaternion.FromToRotation(humanIKController.walkPointer.transform.forward, midDirection);
    }

    public override string getMoveType() {
        return AdvanceIKController.FIK;
    }
    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            state++;
            swingToLeft(joint);
            normalizedTime = 0;
        } 
        if (state == 1) {
            if (normalizedTime > duration) {
                state++;
            } else {
                handController.handLookIKController.init(Time.deltaTime,
                                                        handTransform.position,
                                                        humanIKController.body.transform,
                                                        handTransform);
                // (float, float) sha = swingHandAngle();
                // handController.armLookRotationH = sha.Item1;
                // handController.armLookRotationV = sha.Item2;
                // handController.LookToArmLook();
                Rigidbody rigidbody = joint.GetComponent<Rigidbody>();
                Vector3 swingTorque = new Vector3(0, -humanIKController.swingStrength, 0);
                rigidbody.AddTorque(swingTorque, ForceMode.Acceleration);
                handJoint.GetComponent<Rigidbody>().AddTorque(swingTorque / 2, ForceMode.Acceleration);
            }
        }
        if (state == 2) {
            handController.handLookIKController.transferCurPosToLv1();
            Debug.Log(" swing end ");
            state++;
        }
        return this;
    }

    private void swingToLeft(CharacterJoint joint) {
        Debug.Log(" swingToLeft ");
        float hl = 170f;
        Utils.JointSetLimit(joint, joint.lowTwistLimit.limit, hl);
        Utils.JointSetLimit(handJoint, -20, 20);
        // Debug.Log(" max angular velocity " + rigidbody.maxAngularVelocity);
        // rigidbody.maxAngularVelocity = humanIKController.swingStrength;
    }

    private (float, float) swingHandAngle() {
        float halfSwing = 90;
        float h1 = 45;
        float h2 = 0;
        float v1 = 0;
        float v2 = 180;
        float h = 0;
        float v = 0;
        Vector3 curDir = handTransform.position - joint.connectedBody.transform.position;
        Vector3 mid = getMidDirection();
        if (Vector3.Cross(curDir, mid).y > 0) {
            h = Mathf.Lerp(0, h1, Vector3.Angle(curDir, mid) / halfSwing);
            v = v1;
        } else {
            float hold = 45;
            h = Mathf.Lerp(h1, h2, Vector3.Angle(curDir, mid) - hold / (halfSwing - hold));
            v = Mathf.Lerp(v1, v2, Vector3.Angle(curDir, mid) - hold / (halfSwing - hold));
        }
        return (h, v);
    }

    private Vector3 getMidDirection() {
        Vector3 result =  midDirectionOffset * humanIKController.walkPointer.transform.forward;
        Debug.DrawRay(joint.transform.position, result, Color.red, 1);
        return result;
    }

}
