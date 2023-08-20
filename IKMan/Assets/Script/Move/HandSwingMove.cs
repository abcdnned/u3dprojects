using System;
using UnityEngine;

public class HandSwingMove : HandMove
{
    internal Transform handTransform;

    internal CharacterJoint joint;

    internal Vector3 midDirection;

    internal Quaternion midDirectionOffset;


    public HandSwingMove() : base(MoveNameConstants.HandSwingMove)
    {
    }

    public void init(CharacterJoint joint, Transform handTransform, Vector3 midDirection, float duration) {
        this.duration = duration;
        this.joint = joint;
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
        } 
        if (state == 1) {
            if (normalizedTime > duration) {
                state++;
            } else {
                handController.handLookIKController.init(Time.deltaTime,
                                                        handTransform.position,
                                                        humanIKController.body.transform);
                (float, float) sha = swingHandAngle();
                handController.armLookRotationH = sha.Item1;
                handController.armLookRotationV = sha.Item2;
                // handController.LookToArmLook();
            }
        }
        if (state == 2) {
            Debug.Log(" swing end ");

        }
        return this;
    }

    private void swingToLeft(CharacterJoint joint) {
        Debug.Log(" swingToLeft ");
        Utils.JointSetLimit(joint, 87, joint.highTwistLimit.limit);
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
