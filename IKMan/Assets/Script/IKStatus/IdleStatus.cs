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
            // humanIKController.updateAnchorPoints();
            // Vector3 leftPoint = humanIKController.battleIdleAnchorPoints[ANCHOR_LEFT_LEG];
            // Vector3 rightPoint = humanIKController.battleIdleAnchorPoints[ANCHOR_RIGHT_LEG];
            hic.walkPointer.lookCamera();
            hic.headController.setMode(0);
            // Vector3 leftLegPosition = humanIKController.frontLeftLegStepper.transform.position;
            // leftLegPosition = Utils.snapTo(leftLegPosition, Vector3.up, 0);
            // humanIKController.frontLeftLegStepper.TryTransferDirectly(leftLegPosition,
            //                                                           humanIKController.bi_fontLegAngelOffset);
            // PrefabCreator.SpawnDebugger(rightPoint, "DebugBall", 5, 0.5f, null);
            // humanIKController.frontRightLegStepper.TryPutLeg(rightPoint, humanIKController.bi_backFootAngelOffset,
            //                                                  humanIKController.frontRightLegStepper.shortStepDuration);
            hic.TwoFootAssign(hic.frontRightLegStepper.shortStepDuration,
                                            HumanIKController.RIGHT_FOOT,
                                            .7f,
                                            6,
                                            45f,
                                            90f);
            hic.rightHand.TryGetGreatSword(hic.weapon,
                                                         hic.attchment_rightHand);
            hic.leftHand.TryGetGreatSwordConsonant();
            WalkBalance wb = hic.walkBalance;
            wb.TryBattleIdle();
            return (toBattleIdleState, this);
        }
        else if (e.eventId != null && e.eventId.Equals(HumanIKController.EVENT_KEEP_WALKING)) {
            LocomotionState locomotionState = new LocomotionState(hic);
            // locomotionState.handleEvent(e);
            return (idleState, locomotionState);
        } else if (e.eventId != null && e.eventId.Equals(HumanIKController.EVENT_IDLE)) {
            hic.frontLeftLegStepper.handleEvent(e.eventId);
            hic.frontRightLegStepper.handleEvent(e.eventId);
            hic.leftHand.handleEvent(e.eventId);
            hic.rightHand.handleEvent(e.eventId);
        } else if (e.eventId != null && e.eventId.Equals(HumanIKController.EVENT_STOP_WALKING)) {
            changePose(new IdlePoseArgument(hic));
        }
        return (idleState, this);
    }

    private (States, ActionStateMachine) ToBattleIdle(Event e) {
        if (allIdleCheck()) {
            Debug.Log(" change to battle idle status ");
            hic.headController.setMode(3);
            return (null, new BattleIdleState(hic));
        }
        return (toBattleIdleState, this);
    }

}
