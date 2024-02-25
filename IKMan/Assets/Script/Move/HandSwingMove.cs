using System;
using UnityEngine;

/*
1. how swing recovery
2. soul of character is a ball and some physics parts
3. some predefined animation is necessary, but I need to keep them simple
*/
public class HandSwingMove : HandMove
{
    internal Transform handTransform;

    internal CharacterJoint joint;

    internal CharacterJoint handJoint;

    internal Vector3 midDirection;

    internal Quaternion midDirectionOffset;

    internal delegate void acceptHipSwingRotate(Vector3 foward);
    internal acceptHipSwingRotate AcceptHipSwingRotate;

    public HandSwingMove() : base(MoveNameConstants.HandSwingMove)
    {
    }

    public void init(CharacterJoint joint,
                     CharacterJoint handJoint,
                     Transform handTransform, Vector3 midDirection) {
        base.init();
        this.duration = hic.ap.hipSwingRotateDuration;
        this.joint = joint;
        this.handJoint = handJoint;
        this.handTransform = handTransform;
        this.midDirection = midDirection;
        midDirectionOffset = Quaternion.FromToRotation(hic.walkPointer.transform.forward, midDirection);
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
            if (normalizedTime > duration && !hic.inputArgument.leftHold) {
                Debug.Log(" state ++ ");
                state++;
            } else {
                handController.handLookIKController.init(Time.deltaTime,
                                                        handTransform.position,
                                                        hic.body.transform,
                                                        handTransform);
                // (float, float) sha = swingHandAngle();
                // handController.armLookRotationH = sha.Item1;
                // handController.armLookRotationV = sha.Item2;
                // handController.LookToArmLook();
                Rigidbody rigidbody = joint.GetComponent<Rigidbody>();
                Vector3 swingTorque = new Vector3(0, -hic.swingStrength, 0);
                rigidbody.AddTorque(swingTorque, ForceMode.Acceleration);
                handJoint.GetComponent<Rigidbody>().AddTorque(swingTorque / 2, ForceMode.Acceleration);
                Vector3 dir = handJoint.transform.position - joint.transform.position;
                // Debug.DrawRay(hic.spin1.transform.position, dir, Color.gray, 0.1f);
                AcceptHipSwingRotate?.Invoke(dir);
            }
        }
        bool swingEnd = false;
        if (state == 2) {
            if (!swingEnd) {
                swingEnd = true;
                handController.handLookIKController.transferCurPosToLv1();
                Utils.JointSetLimit(joint, -45, 100, 0, 0);
                Utils.JointSetLimit(handJoint, 0, 0);
                // Debug.Log(" swing end ");
            }
            if (normalizedTime > duration + 1) {
                Utils.JointSetLimit(joint, -45, -44, 0, 0);
                state++;
            }
            Rigidbody rigidbody = joint.GetComponent<Rigidbody>();
            Vector3 swingEndTorque = new Vector3(0, hic.swingStrength / 3, 0);
            rigidbody.AddTorque(swingEndTorque, ForceMode.Acceleration);
        }
        if (state == 3) {
        }
        return this;
    }

    private void swingToLeft(CharacterJoint joint) {
        Debug.Log(" swingToLeft ");
        float hl = 100f;
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
        Vector3 result =  midDirectionOffset * hic.walkPointer.transform.forward;
        Debug.DrawRay(joint.transform.position, result, Color.red, 1);
        return result;
    }

}
