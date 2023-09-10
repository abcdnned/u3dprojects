using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseArgument
{
    protected LegControllerType2 leftLegController;
    protected LegControllerType2 rightLegController;
    protected HandController leftHandController;
    protected HandController rightHandController;

    protected HandDelayLooker leftLeg;

    protected HandDelayLooker leftFoot;

    protected HandDelayLooker rightLeg;

    protected HandDelayLooker rightFoot;
    protected HandDelayLooker leftElbow;

    protected HandDelayLooker leftHand;

    protected HandDelayLooker rightElbow;

    protected HandDelayLooker rightHand;

    protected Transform rightShoulder;
    protected Transform leftShoulder;
    protected Transform rightHip;
    protected Transform leftHip;

    protected HumanIKController hic;

    public PoseArgument(HumanIKController humanIKController) {
        hic = humanIKController;

        leftLegController = hic.frontLeftLegStepper;
        leftHandController = hic.leftHand;
        leftLeg = leftLegController.advanceIKController.elbow;
        leftFoot = leftLegController.advanceIKController.hand;
        leftElbow = leftHandController.advanceIKController.elbow;
        leftHand = leftHandController.advanceIKController.hand;
        leftHip = leftLegController.advanceIKController.shoulder;
        leftShoulder = leftHandController.advanceIKController.shoulder;

        rightLegController = hic.frontRightLegStepper;
        rightHandController = hic.rightHand;
        rightLeg = rightLegController.advanceIKController.elbow;
        rightFoot = rightLegController.advanceIKController.hand;
        rightElbow = rightHandController.advanceIKController.elbow;
        rightHand = rightHandController.advanceIKController.hand;
        rightShoulder = rightHandController.advanceIKController.shoulder;
        rightHip = rightLegController.advanceIKController.shoulder;
    }

    public virtual void update() {
    }

    public virtual void run() {

    }

}