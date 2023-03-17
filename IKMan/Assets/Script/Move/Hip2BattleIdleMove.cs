
using UnityEngine;

public class Hip2BattleIdleMove : Move
{
    WalkBalance parent;

    private float hipMoveSpeed;
    public Hip2BattleIdleMove() : base(MoveNameConstants.HipIdle2BattleIdle) {

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
            hipMoveSpeed = parent.hipHeightDiff(parent.battleIdleHipH, Vector3.up) / duration;
        }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            return moveManager.ChangeMove(MoveNameConstants.HipBattleIdle);
        } else {
            parent.transfer(parent.battleIdleAngelOffset);
            parent.adjustHeight(parent.battleIdleHipH, Vector3.up, hipMoveSpeed);
            // Debug.Log(this.GetType().Name + " speed " + hipMoveSpeed);
        }
        return this;
    }
}