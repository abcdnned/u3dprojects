using System;
using UnityEngine;

public class HandSwingMove : HandMove
{
    internal Transform handTransform;

    internal CharacterJoint joint;

    public HandSwingMove() : base(MoveNameConstants.HandSwingMove)
    {
    }

    public void init(CharacterJoint joint, Transform handTransform) {
        this.joint = joint;
        this.handTransform = handTransform;
    }

    public override string getMoveType() {
        return AdvanceIKController.FIK;
    }
    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            state = 1;
            swingToLeft(joint);
        }
        return this;
    }

    private void swingToLeft(CharacterJoint joint) {
        Utils.JointSetLimit(joint, 42, 45);
    }

}
