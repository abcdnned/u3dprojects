using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LocomotionState : AnyState {
    public const string NAME = "LocomotionState";
    public const string STATE_MOVE = "moveState";
    public const string STATE_STOPPING = "stoppingState";
    public const string STATE_TRANSFER = "transferState";
    public const string STATE_JUMP = "jumpState";
    public const string STATE_LAND = "landState";
    public const string STATE_PREPAREJUMP = "prepareJumpState";
    // Banner transferBanner = new Banner(Banner.WALKING_TO_TRANSFER);

    private States moveState;
    private States stoppingState;
    private States transferState;
    private States jumpState;
    private States landState;
    private States preJumpState;


    private Vector3 offset;

    public LocomotionState(HumanIKController humanIKController) : base(humanIKController)
    {
    }

    protected override void initState() {
        moveState = new States(STATE_MOVE, Move);
        stoppingState = new States(STATE_STOPPING, Stopping);
        transferState = new States(STATE_TRANSFER, Transfer);
        jumpState = new States(STATE_JUMP, Jump);
        landState = new States(STATE_LAND, Land);
        preJumpState = new States(STATE_PREPAREJUMP, PrepareJump);
        cs = moveState;
    }
    private (States, ActionStateMachine) Move(Event e)
    {
        if (e.eventId != null && e.eventId.Equals(HumanIKController.EVENT_JUMP)) {
            return (preJumpState, this);
        }
        else if (e.eventId != null && e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
                changePose(new RunPoseArgument(hic));
        } else if (hic.inputArgument.movement.magnitude <= 0) {
            return (null, new IdleStatus(hic));
        }
        return (moveState, this);
    }

    private (States, ActionStateMachine) Stopping(Event e)
    {
        if (legIdleChecker()) {
            // Debug.Log(this.GetType().Name + " stopWalingBanner checked ");
            Vector3 direction = Utils.forwardFlat(hic.body.transform);
            float leftDot = Vector3.Dot(hic.frontLeftLegStepper.transform.position, direction);
            float rightDot = Vector3.Dot(hic.frontRightLegStepper.transform.position, direction);
            if (!hic.frontLeftLegStepper.move.IsLegMoving() && !hic.frontRightLegStepper.move.IsLegMoving()
                && (Mathf.Max(leftDot, rightDot) - Mathf.Min(leftDot, rightDot) > 0.2)) {
                // Debug.Log(this.GetType().Name + " need one foot transfer ");
                if (leftDot < rightDot) {
                    hic.frontLeftLegStepper.TryMove();
                    hic.frontLeftLegStepper.handleEvent((HumanIKController.EVENT_STOP_WALKING));
                } else {
                    hic.frontRightLegStepper.TryMove();
                    hic.frontRightLegStepper.handleEvent((HumanIKController.EVENT_STOP_WALKING));
                }
                hic.leftHand.handleEvent((HumanIKController.EVENT_STOP_WALKING));
                hic.rightHand.handleEvent((HumanIKController.EVENT_STOP_WALKING));
                return (transferState, this);
            } else {
                return (stoppingState, new IdleStatus(hic));
            }
        }
        else if (e.eventId != null && e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
            return (moveState, this);
        }
        return (stoppingState, this);
    }

    private (States, ActionStateMachine) Transfer(Event e)
    {
        if (allIdleCheck()) {
            // Debug.Log(this.GetType().Name + " transferBanner checked ");
            return (transferState, new IdleStatus(hic));
        }
        return (transferState, this);
    }
    public override string getName() {
        return NAME;
    }

    public override ActionStateMachine run() {
        return null;
    }

    public override void handleInput(SMInput smInput) {

    }

    private void destoryMovingSphere() {
    }

    private (States, ActionStateMachine) Jump(Event e)
    {
        changePose(new AirIdlePoseArgument(hic));
        if (((AirIdlePoseArgument)pose).landed) {
            return (landState, this);
        }
        return (jumpState, this);
    }

    private (States, ActionStateMachine) Land(Event e)
    {
        changePose(new SmallLandPoseArgument(hic));
        if (((SmallLandPoseArgument)pose).recover) {
            return (moveState, this);
        }
        return (landState, this);
    }

    private (States, ActionStateMachine) PrepareJump(Event e)
    {
        changePose(new PreJumpPoseArgument(hic));
        if (((PreJumpPoseArgument)pose).prepared) {
            hic.moveController.addInputSignal(1);
            return (jumpState, this);
        }
        return (preJumpState, this);
    }
}