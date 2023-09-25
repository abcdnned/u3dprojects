using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePoseArgument : PoseArgument
{


    public IdlePoseArgument(HumanIKController humanIKController) : base(humanIKController) {
    }

    public override void update() {
        legUpdate(leftLegController, ref llh, leftStopSignal);
        legUpdate(rightLegController, ref rlh, rightStopSignal);
    }

    public override void run() {
        float t = Time.time;
        hic.leftHand.TryIdle();
        hic.rightHand.TryIdle();
        hic.frontLeftLegStepper.TryIdle();
        hic.frontRightLegStepper.TryIdle();
        hic.walkBalance.TryIdle(t);
    }

}
