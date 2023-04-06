
using UnityEngine;

public class Hip2BattleIdleMove : Move
{
    WalkBalance parent;

    private Vector3 h;
    private float hipMoveSpeed;
    // private float transferSpeed;
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
            parent.adjustLegDistance();
            h = parent.getDynamicHeight(parent.leftLeg.transform.position,
                                                parent.rightLeg.transform.position,
                                                parent.battleLegDistance);
            hipMoveSpeed = parent.hipHeightDiff(h.y, Vector3.up) / duration;
            // transferSpeed = parent.getTransferSpeed(parent.battleIdleAngelOffset, duration);
        }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            return moveManager.ChangeMove(MoveNameConstants.HipBattleIdle);
        } else {
            parent.ReturnToCenter();
            parent.transferByTime(parent.battleIdleAngelOffset, normalizedTime / duration);
            // Debug.Log(" transferSpeed " + transferSpeed);
            parent.adjustHeight(h.y, Vector3.up, hipMoveSpeed);
            // Debug.Log(this.GetType().Name + " speed " + hipMoveSpeed);
        }
        return this;
    }
}