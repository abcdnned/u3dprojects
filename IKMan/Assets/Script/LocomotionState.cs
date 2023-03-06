using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LocomotionState : AnyState {

    Banner transferBanner = new Banner();

    private States moveState;
    private States stoppingState;
    private States transferState;

    private States cs;

    public LocomotionState(HumanIKController humanIKController) : base(humanIKController)
    {
        moveState = new States("moveState", Move);
        stoppingState = new States("stoppingState", Stopping);
        transferState = new States("transferState", Transfer);
        cs = moveState;
    }
    private (States, ActionStateMachine) Move(Event e)
    {
        if (e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
            humanIKController.frontRightLegStepper.handleEvent(HumanIKController.EVENT_KEEP_WALKING);
            humanIKController.frontLeftLegStepper.TryMove();
            humanIKController.frontRightLegStepper.TryMove();
            humanIKController.leftHand.handleEvent(e);
            humanIKController.rightHand.handleEvent(e);
        } else if (e.eventId.Equals(HumanIKController.EVENT_STOP_WALKING)) {
            if (humanIKController.frontLeftLegStepper.Moving) {
                humanIKController.frontLeftLegStepper.handleEvent(e.eventId, stopWalkingBanner);
            } else {
                humanIKController.frontLeftLegStepper.handleEvent(e.eventId);
            }
            if (humanIKController.frontRightLegStepper.Moving) {
                humanIKController.frontRightLegStepper.handleEvent(e.eventId, stopWalkingBanner);
            } else {
                humanIKController.frontRightLegStepper.handleEvent(e.eventId);
            }
            humanIKController.leftHand.handleEvent(e, stopWalkingBanner);
            humanIKController.rightHand.handleEvent(e, stopWalkingBanner);
            return (stoppingState, this);
        }
        return (moveState, this);
    }

    private (States, ActionStateMachine) Stopping(Event e)
    {
        if (stopWalkingBanner.Check()) {
            Debug.Log(this.GetType().Name + " stopWalingBanner checked ");
            Vector3 direction = Utils.forward(humanIKController.body.transform);
            float leftDot = Vector3.Dot(humanIKController.frontLeftLegStepper.transform.position, direction);
            float rightDot = Vector3.Dot(humanIKController.frontRightLegStepper.transform.position, direction);
            if (!humanIKController.frontLeftLegStepper.Moving && !humanIKController.frontRightLegStepper.Moving
                && (Mathf.Max(leftDot, rightDot) - Mathf.Min(leftDot, rightDot) > 0.2)) {
                Debug.Log(this.GetType().Name + " need one foot transfer ");
                if (leftDot < rightDot) {
                    humanIKController.frontLeftLegStepper.TryMove();
                    humanIKController.frontLeftLegStepper.handleEvent(HumanIKController.EVENT_STOP_WALKING, transferBanner);
                } else {
                    humanIKController.frontRightLegStepper.TryMove();
                    humanIKController.frontRightLegStepper.handleEvent(HumanIKController.EVENT_STOP_WALKING, transferBanner);
                }
                humanIKController.leftHand.handleEvent(new Event(HumanIKController.EVENT_STOP_WALKING), transferBanner);
                humanIKController.rightHand.handleEvent(new Event(HumanIKController.EVENT_STOP_WALKING), transferBanner);
                return (transferState, this);
            } else {
                return (stoppingState, new IdleStatus(humanIKController));
            }
        }
        else if (e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
            humanIKController.frontRightLegStepper.handleEvent(HumanIKController.EVENT_KEEP_WALKING);
            humanIKController.frontLeftLegStepper.TryMove();
            humanIKController.frontRightLegStepper.TryMove();
            humanIKController.leftHand.handleEvent(e);
            humanIKController.rightHand.handleEvent(e);
            stopWalkingBanner.refresh();
            return (moveState, this);
        }
        return (stoppingState, this);
    }

    private (States, ActionStateMachine) Transfer(Event e)
    {
        if (transferBanner.Check()) {
            Debug.Log(this.GetType().Name + " transferBanner checked ");
            return (transferState, new IdleStatus(humanIKController));
        }
        return (transferState, this);
    }
    public override string getName() {
        return "LocomotionState";
    }

    public override ActionStateMachine run() {
        return null;
    }
    public override ActionStateMachine handleEvent(Event e) {
        (States, ActionStateMachine) value = cs.handleEvent(e);
        if (cs != value.Item1) {
            Debug.Log(this.GetType().Name + " State change to " + value.Item1.name);
            cs = value.Item1;
        }
        return value.Item2;
        // if (transferBanner.Check()) {
        //     Debug.Log(this.GetType().Name + " transferBanner checked ");
        //     return new IdleStatus(humanIKController);
        // }
        // if (stopWalkingBanner.Check()) {
        //     Debug.Log(this.GetType().Name + " stopWalingBanner checked ");
        //     Vector3 direction = Utils.forward(humanIKController.body.transform);
        //     float leftDot = Vector3.Dot(humanIKController.frontLeftLegStepper.transform.position, direction);
        //     float rightDot = Vector3.Dot(humanIKController.frontRightLegStepper.transform.position, direction);
        //     if (!humanIKController.frontLeftLegStepper.Moving && !humanIKController.frontRightLegStepper.Moving
        //         && (Mathf.Max(leftDot, rightDot) - Mathf.Min(leftDot, rightDot) > 0.2)) {
        //         transferBanner.Reset(1);
        //         Debug.Log(this.GetType().Name + " need one foot transfer ");
        //         if (leftDot < rightDot) {
        //             humanIKController.frontLeftLegStepper.TryMove();
        //             humanIKController.frontLeftLegStepper.handleEvent(HumanIKController.EVENT_STOP_WALKING, transferBanner);
        //         } else {
        //             humanIKController.frontRightLegStepper.TryMove();
        //             humanIKController.frontRightLegStepper.handleEvent(HumanIKController.EVENT_STOP_WALKING, transferBanner);
        //         }
        //     } else {
        //         return new IdleStatus(humanIKController);
        //     }
        // }
        // if (e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
        //     humanIKController.frontRightLegStepper.handleEvent(HumanIKController.EVENT_KEEP_WALKING);
        //     humanIKController.frontLeftLegStepper.TryMove();
        //     humanIKController.frontRightLegStepper.TryMove();
        //     humanIKController.leftHand.handleEvent(e);
        //     humanIKController.rightHand.handleEvent(e);
        // } else if (e.eventId.Equals(HumanIKController.EVENT_STOP_WALKING)) {
        //     int resetValue = 3;
        //     if (humanIKController.frontLeftLegStepper.Moving && humanIKController.frontRightLegStepper.Moving) {
        //         resetValue = 4;
        //     }
        //     stopWalkingBanner.Reset(resetValue);
        //     humanIKController.frontLeftLegStepper.handleEvent(e.eventId, stopWalkingBanner);
        //     humanIKController.frontRightLegStepper.handleEvent(e.eventId, stopWalkingBanner);
        //     humanIKController.leftHand.handleEvent(e, stopWalkingBanner);
        //     humanIKController.rightHand.handleEvent(e, stopWalkingBanner);
        // }
    }


    public override void handleInput(SMInput smInput) {

    }
}