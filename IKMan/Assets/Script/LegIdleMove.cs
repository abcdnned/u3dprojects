using UnityEngine;

public class LegIdleMove : LegHandMove
{
    public LegIdleMove() : base(MoveNameConstants.LegIdle) {

    }
    public override string getMoveType() {
        return AdvanceIKController.FK;
    }


    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            ((TwoNodeController)targetController).SyncIKSample(IKSampleNames.LEG_IDLE, targetController.hic.ap.transferSpeedSmall, !twoNodeController().IsRightPart());
            state++;
        } 
        if (state == 1) {
            updateIKRotation();
        }
        return this;
    }
}





