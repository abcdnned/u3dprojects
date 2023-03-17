using UnityEngine;

public class LegPutMove : LegMove
{
    public LegPutMove() : base(MoveNameConstants.LegPutMove) {}

    protected override void subinit()
    {
        
    }

    Steper steper;

    public override Move move(float dt) {
        if (state == 0) {
            state = 1;
            Transform target = parent.transform;
            Vector3 wp1 = target.position;
            Vector3 wp3 = parent.humanIKController.battleIdleAnchorPoints[HumanIKController.ANCHOR_LEFT_LEG];
            Vector3 wp2 = Utils.GetMiddleLiftPoint(wp1, wp3, parent.shortStepLiftDistance);
            Vector3 realForward, realRight;
            Utils.GetForwardAndRight(wp1, wp3, out realForward, out realRight);
            steper = new Steper(Utils.forward(parent.body.transform),
                                Utils.right(parent.body.transform),
                                parent.shortStepDuration,
                                Steper.BEARZ,
                                null,
                                realForward,
                                realRight,
                                1,
                                parent.humanIKController.frontLeftLegStepper.transform,
                                new Vector3[] {wp1, wp2, wp3});
        }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            return moveManager.ChangeMove(MoveNameConstants.LegIdle);
        } else {
            steper.step(dt);
        }
        return this;
    }
}