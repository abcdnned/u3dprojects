using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SmallLandPoseArgument : PoseArgument
{
    private float initTime;
    private int state = 0;
    internal bool recover = false;

    public SmallLandPoseArgument(HumanIKController humanIKController) : base(humanIKController) {

    }

    public override void update() {
        if (state == 0) {
            state++;
            initTime = Time.time;
        }
        if (state == 1) {
            legUpdate(leftLegController, ref llh, ref leftStopSignal);
            legUpdate(rightLegController, ref rlh, ref rightStopSignal);
            if (Time.time - initTime > hic.ap.smallLandDuration) {
                recover = true;
            }
        }
        //decrease land time
    }

    public override void run() {
        float d = hic.ap.smallLandDuration;
        hic.leftHand.TrySync(d, IKSampleNames.HAND_LAND, "hand_land");
        hic.rightHand.TrySync(d, IKSampleNames.HAND_LAND, "hand_land");
        hic.frontLeftLegStepper.TrySync(d, IKSampleNames.LEG_IDLE, "leg_land");
        hic.frontRightLegStepper.TrySync(d, IKSampleNames.LEG_IDLE, "leg_land");
        hic.walkBalance.TryChangeHeight(hic.ap.landDownHeigth, hic.ap.smallLandDuration);
    }

}