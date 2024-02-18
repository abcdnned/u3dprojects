using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HumanIKController;
public class BattleIdleState : AnyState
{

    public const string NAME = "BattleIdleState";
    public const string STATE_BATTLE = "battleState";
    public const string STATE_TO_IDLE = "toIdleTransfer";
    public const string STATE_OCCUPY = "occupyState";
    private States battleIdleState;

    private States toIdleTransfer;

    private States occupyState;

    public BattleIdleState(HumanIKController humanIKController) : base(humanIKController)
    {
    }

    protected override void initState()
    {
        battleIdleState = new States(STATE_BATTLE, BattleIdle);
        toIdleTransfer = new States(STATE_TO_IDLE, ToIdleTransfer);
        occupyState = new States(STATE_OCCUPY, Occupy);
        cs = battleIdleState; ;
    }

    private (States, ActionStateMachine) BattleIdle(Event e)
    {
        if (e.bgA == EVENT_BUTTON_R)
        {
            hic.updateAnchorPoints();
            // Vector3 leftPoint = humanIKController.idleAnchorPoints[ANCHOR_LEFT_LEG];
            // Vector3 rightPoint = humanIKController.idleAnchorPoints[ANCHOR_RIGHT_LEG];
            // leftPoint = Utils.snapTo(leftPoint, Vector3.up, 0);
            // rightPoint = Utils.snapTo(rightPoint, Vector3.up, 0);
            // humanIKController.frontLeftLegStepper.TryRotateLeg(0);
            hic.frontRightLegStepper.TryPutLeg(getReturnRightPosition(), 0,
                                                             hic.frontRightLegStepper.shortStepDuration);
            // WalkBalance wb = humanIKController.walkBalance;
            // wb.TryRotate(0, wb.idleHipH);
            hic.headController.setMode(1);
            hic.rightHand.TryReturnSword(hic.weapon,
                                                       hic.attchment_rightHand);
            return (toIdleTransfer, this);
        }
        else if (e.bgA == EVENT_LEFT_CLICK)
        {
            hic.rightHand.TryLeftSwing();
            hic.walkBalance.TrySwingRotate((HandSwingMove)hic.rightHand.move);
            return (occupyState, this);
        }
        return (battleIdleState, this);
    }

    private Vector3 getReturnRightPosition()
    {
        Vector3 leftPoint = hic.idleAnchorPoints[ANCHOR_LEFT_LEG];
        Vector3 rightPoint = hic.idleAnchorPoints[ANCHOR_RIGHT_LEG];
        Vector3 currentLeft = hic.frontLeftLegStepper.transform.position;
        return currentLeft + (rightPoint - leftPoint);
    }

    private (States, ActionStateMachine) ToIdleTransfer(Event e)
    {
        if (allIdleCheck())
        {
            return (null, new IdleStatus(hic));
        }
        return (toIdleTransfer, this);
    }

    private (States, ActionStateMachine) Occupy(Event e)
    {
        if (allIdleCheck())
        {
            Debug.Log(" exit occupy ");
            return (null, this);
        }
        return (occupyState, this);
    }


    public override ActionStateMachine run()
    {
        return null;
    }


    public override string getName()
    {
        return NAME;
    }

}