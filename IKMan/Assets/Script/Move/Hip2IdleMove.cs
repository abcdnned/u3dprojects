
using UnityEngine;

public class Hip2IdleMove : Move
{
    WalkBalance parent;

    private float hipMoveSpeed;
    public Hip2IdleMove() : base(MoveNameConstants.HipBattleIdle2Idle) {

    }
    public override void init() {
        parent = (WalkBalance)targetController;
        normalizedTime = 0;
        state = 0;
        duration = parent.battleIdleTransferDuration;
    }
    public override Move move(float dt) {
        if (state == 0) {
            state = 1;
            hipMoveSpeed = parent.hipHeightDiff(parent.idleHipH, Vector3.up) / duration;
        }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            return moveManager.ChangeMove(MoveNameConstants.HipIdle);
        } else {
            parent.transfer(0);
            parent.adjustGroundedHeight(parent.idleHipH, Vector3.up, hipMoveSpeed);
        }
        return this;
    }
}