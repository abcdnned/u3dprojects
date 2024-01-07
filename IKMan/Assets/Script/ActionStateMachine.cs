using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author: Sergio Abreu García | https://sergioabreu.me

/// <summary> Tells the ActiveRagdoll what it should do. Input can be external (like the
/// one from the player or from another script) and internal (kind of like sensors, such as
/// detecting if it's on floor). </summary>
public class ActionStateMachine {
    // ---------- EXTERNAL INPUT ----------
    protected ActionStateMachine previousState;
    protected HumanIKController hic;

    internal PoseArgument pose;
    internal States cs;


    public ActionStateMachine(HumanIKController humanIKController) {
        this.hic = humanIKController;
    }

    public void setPreviousState(ActionStateMachine previousState) {
        this.previousState = previousState;
    }

    public virtual string getName() {
        return "ActionStateMachine";
    }

    public virtual ActionStateMachine run() {
        return null;
    }

    public virtual ActionStateMachine handleEvent(Event e) {
        return this;
    }

    public virtual void handleInput(SMInput smInput) {

    }

    public bool allIdleCheck()
    {
        return hic.leftHand.move.isIdle()
               && hic.rightHand.move.isIdle()
               && hic.frontLeftLegStepper.move.isIdle()
               && hic.frontRightLegStepper.move.isIdle();
    }

    public bool legMovingCheck() {
        if (hic.frontLeftLegStepper.move == null || hic.frontRightLegStepper.move == null)
            return false;
        return hic.frontLeftLegStepper.move.name.Contains("moving")
               || hic.frontRightLegStepper.move.name.Contains("moving");
    }

    // check if only the two legs are idle
    public bool legIdleChecker()
    {
        if (hic.frontLeftLegStepper.move == null || hic.frontRightLegStepper.move == null)
            return false;
        return hic.frontLeftLegStepper.move.isIdle()
               && hic.frontRightLegStepper.move.isIdle();
    }

    public PoseArgument changePose(PoseArgument argument, bool refresh = false) {
        if (pose != null && (pose.GetType() == argument.GetType()) && !refresh)  {
            // Keep
        } else {
            if (pose != null) {
                pose.exit();
            }
            pose = argument;
            pose.run();
        }
        return pose;
    }


}