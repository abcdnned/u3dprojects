using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HumanIKController;
public class BattleIdleState : AnyState {

    public const string NAME = "BattleIdleState";
    public const string STATE_BATTLE = "battleState";
    private States battleIdleState;

    private States toIdleTransfer;

    public BattleIdleState(HumanIKController humanIKController) : base(humanIKController)
    {
    }

    protected override void initState() {
        battleIdleState = new States(STATE_BATTLE, BattleIdle);
        toIdleTransfer = new States("toIdleTransfer", ToIdleTransfer);
        cs = battleIdleState;;
    }

    private (States, ActionStateMachine) BattleIdle(Event e)
    {
        if (e.bgA == EVENT_BUTTON_R) {
            Vector3 leftPoint = humanIKController.idleAnchorPoints[ANCHOR_LEFT_LEG];
            Vector3 rightPoint = humanIKController.idleAnchorPoints[ANCHOR_RIGHT_LEG];
            leftPoint = Utils.snapTo(leftPoint, Vector3.up, 0);
            rightPoint = Utils.snapTo(rightPoint, Vector3.up, 0);
            humanIKController.frontLeftLegStepper.TryTransferDirectly(leftPoint, 0);
            humanIKController.frontRightLegStepper.TryTransferDirectly(rightPoint, 0);
            WalkBalance wb = humanIKController.walkBalance;
            wb.TryRotate(0, wb.idleHipH);
            humanIKController.headController.setMode(1);
            return (toIdleTransfer, this);
        }
        return (battleIdleState, this);
    }

    private (States, ActionStateMachine) ToIdleTransfer(Event e) {
        if (allIdleCheck()) {
            return (null, new IdleStatus(humanIKController));
        }
        return (toIdleTransfer, this);
    }


    public override ActionStateMachine run() {
        return null;
    }


    public override string getName() {
        return NAME;
    }

}