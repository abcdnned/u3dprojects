using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LocomotionState : AnyState {

    public LocomotionState(HumanIKController humanIKController) : base(humanIKController)
    {
    }
    public override string getName() {
        return "LocomotionState";
    }

    public override ActionStateMachine run() {
        return null;
    }
    public override ActionStateMachine handleEvent(Event e) {
        if (e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
            humanIKController.frontRightLegStepper.handleEvent(HumanIKController.EVENT_KEEP_WALKING);
            humanIKController.frontLeftLegStepper.TryMove();
            humanIKController.frontRightLegStepper.TryMove();
        } else if (e.eventId.Equals(HumanIKController.EVENT_STOP_WALKING)) {
            humanIKController.frontLeftLegStepper.handleEvent(HumanIKController.EVENT_STOP_WALKING);
            humanIKController.frontRightLegStepper.handleEvent(HumanIKController.EVENT_STOP_WALKING);
        }
        return this;
    }


    public override void handleInput(SMInput smInput) {

    }
}