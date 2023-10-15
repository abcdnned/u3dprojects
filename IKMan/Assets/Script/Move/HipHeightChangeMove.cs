using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HipHeightChangeMove : HipMove
{
    private float targetHeight;
    private float spin2Rotation;
    private float initHeight;
    private float initSpin2LocalRotationX;
    private bool valid;
    public HipHeightChangeMove() : base(MoveNameConstants.HipHeightChangeMove) {

    }

    public void initBasics(float h, float duration) {
        targetHeight = h;
        this.duration = duration;
        (valid, initHeight) = controller.getGroundedHeight(hic.gravityUp);
        initSpin2LocalRotationX = hic.spin2.localEulerAngles.x;
    }

    public override Move move(float dt) {
        normalizedTime += dt;
        if (!valid) throw new Exception("Invalid grounded height");
        if (state == 0) {
            state = 1;
        }
        if (state == 1) {
            float poc = normalizedTime / duration;
            float ty = Mathf.Lerp(initHeight, targetHeight, poc);
            controller.adjustGroundedHeight(ty, hic.gravityUp, 0, true);
        }
        return this;
    }
    // TODO change this class to a DelayValueObject for WalkBalance
}