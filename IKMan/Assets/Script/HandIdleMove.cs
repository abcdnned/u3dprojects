using UnityEngine;

public class HandIdleMove : LegHandMove
{
    public HandIdleMove() : base(MoveNameConstants.HandIdle) {}

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }

    
    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            ((TwoNodeController)targetController).SyncIKSample(IKSampleNames.IDLE_SAMPLE, targetController.hic.ap.transferSpeedFast, !twoNodeController().IsRightPart());
            state++;
        } 
        if (state == 1) {
            handController().LookToArmLook();
        }
        return this;
    }



    
}
