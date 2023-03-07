using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnyState : ActionStateMachine
{
    protected Banner stopWalkingBanner;
    protected Banner keepWalkingBanner;
    protected Banner idleBanner;
    public AnyState(HumanIKController humanIKController) : base(humanIKController)
    {
        stopWalkingBanner = new Banner(Banner.STOP_WALKING);
    }
    public bool allIdleCheck()
    {
        return humanIKController.leftHand.move.name.Contains("idle")
               && humanIKController.rightHand.move.name.Contains("idle")
               && humanIKController.frontLeftLegStepper.move.name.Contains("idle")
               && humanIKController.frontRightLegStepper.move.name.Contains("idle");
    }

    // check if only the two legs are idle
    public bool legIdleChecker()
    {
        return humanIKController.frontLeftLegStepper.move.name.Contains("idle")
               && humanIKController.frontRightLegStepper.move.name.Contains("idle");
    }
}