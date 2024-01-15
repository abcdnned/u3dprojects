using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PreJumpPoseArgument : PoseArgument
{
    private float initTime;
    private int state = 0;
    internal bool prepared = false;

    public PreJumpPoseArgument(HumanIKController humanIKController) : base(humanIKController) {

    }

    public override void update() {
        if (state == 0) {
            state++;
            initTime = Time.time;
        }
        if (state == 1) {
            legUpdate(leftLegController, ref llh, ref leftStopSignal);
            legUpdate(rightLegController, ref rlh, ref rightStopSignal);
            if (Time.time - initTime > 0.08f) {
                prepared = true;
            }
        }
    }

    public override void run() {
        float d = .22f;
        hic.leftHand.TrySync(d, IKSampleNames.HAND_LAND, MoveNameConstants.HandPrepareJump);
        hic.rightHand.TrySync(d, IKSampleNames.HAND_LAND, MoveNameConstants.HandPrepareJump);
        hic.frontLeftLegStepper.TrySync(d, IKSampleNames.LEG_IDLE, MoveNameConstants.LegPrepareJump);
        hic.frontRightLegStepper.TrySync(d, IKSampleNames.LEG_IDLE, MoveNameConstants.LegPrepareJump);
        hic.walkBalance.TryChangeHeight(hic.ap.landDownHeigth, d);
    }

}