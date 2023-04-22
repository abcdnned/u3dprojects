using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HumanIKController;

public class IdleStatus : AnyState
{
    public const string NAME = "IdleStatus";
    public const string STATE_IDLE = "idleState";
    public const string STATE_TOBATTLEIDLE = "toBattleIdleState";
    private States idleState;
    private States toBattleIdleState;
    public IdleStatus(HumanIKController humanIKController) : base(humanIKController)
    {
    }
    public override string getName() => NAME;
    protected override void initState() {
        idleState = new States(STATE_IDLE, Idle);
        toBattleIdleState = new States(STATE_TOBATTLEIDLE, ToBattleIdle);
        cs = idleState;
    }
    private (States, ActionStateMachine) Idle(Event e) {
        if (e.bgA == EVENT_BUTTON_R) {
            humanIKController.updateAnchorPoints();
            Vector3 leftPoint = humanIKController.battleIdleAnchorPoints[ANCHOR_LEFT_LEG];
            Vector3 rightPoint = humanIKController.battleIdleAnchorPoints[ANCHOR_RIGHT_LEG];
            humanIKController.walkPointer.lookCamera();
            humanIKController.headController.setMode(0);
            Vector3 leftLegPosition = humanIKController.frontLeftLegStepper.transform.position;
            leftLegPosition = Utils.snapTo(leftLegPosition, Vector3.up, 0);
            // humanIKController.frontLeftLegStepper.TryTransferDirectly(leftLegPosition,
            //                                                           humanIKController.bi_fontLegAngelOffset);
            // PrefabCreator.SpawnDebugger(rightPoint, "DebugBall", 5, 0.5f, null);
            humanIKController.frontRightLegStepper.TryPutLeg(rightPoint, humanIKController.bi_backFootAngelOffset);
            humanIKController.rightHand.TryGetGreatSword();
            WalkBalance wb = humanIKController.walkBalance;
            wb.TryBattleIdle();
            return (toBattleIdleState, this);
        }
        else if (e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
            LocomotionState locomotionState = new LocomotionState(humanIKController);
            locomotionState.handleEvent(e);
            return (idleState, locomotionState);
        } else if (e.eventId.Equals(HumanIKController.EVENT_IDLE)) {
            humanIKController.frontLeftLegStepper.handleEvent(e.eventId);
            humanIKController.frontRightLegStepper.handleEvent(e.eventId);
            humanIKController.leftHand.handleEvent(e.eventId);
            humanIKController.rightHand.handleEvent(e.eventId);
        }
        return (idleState, this);
    }

    private (States, ActionStateMachine) ToBattleIdle(Event e) {
        if (allIdleCheck()) {
            humanIKController.headController.setMode(3);
            return (null, new BattleIdleState(humanIKController));
        }
        return (toBattleIdleState, this);
    }

}
