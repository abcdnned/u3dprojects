using UnityEngine;

public class HipIdleMove : HipMove
{
    public HipIdleMove() : base(MoveNameConstants.HipIdle) {
    }

    public override Move move(float dt) {
        parent.ReturnToCenter();
        parent.adjustLegDistance();
        Vector3 h = parent.getDynamicHeight(parent.leftLeg.transform.position,
                                            parent.rightLeg.transform.position,
                                            parent.expectLegDistance);
        parent.adjustHeight(h.y, Vector3.up, parent.hipBattleSpeed);
        return this;
    }
    // public override Move transfer() {
    //     if ((parent.leftLeg.move.IsLegMoving() && !parent.leftLeg.Recover)
    //         || (parent.rightLeg.move.IsLegMoving() && !parent.rightLeg.Recover)) {
    //         return moveManager.getMove(MoveNameConstants.HipDamp);
    //     } else if (
    //         (humanIKController.currentStatus.getName() == IdleStatus.NAME
    //         && humanIKController.currentStatus.cs.name == IdleStatus.STATE_TOBATTLEIDLE) ||
    //         (humanIKController.currentStatus.getName() == BattleIdleState.NAME
    //         && humanIKController.currentStatus.cs.name == BattleIdleState.STATE_BATTLE)) {
    //         return moveManager.getMove(MoveNameConstants.HipIdle2BattleIdle);
    //     } else {
    //         return this;
    //     }
    // }
    
    // private void keepBalanceWhenWalking() {
    //     Vector3 forward2 = Utils.forward(parent.target);
    //     Vector3 right2 = Utils.right(parent.target);
    //     Vector3 plane = Vector3.up;
    //     Vector3 tp = Vector3.Lerp(
    //     // transform.position = Vector3.Lerp(
    //                                     parent.target.position,
    //                                     parent.dampDist,
    //                                     1 - Mathf.Exp(-parent.finalDampSpeed * Time.deltaTime)
    //                                 );
    //     Vector3 delta = tp - parent.lastDampStart;
    //     parent.dampSum += delta.magnitude;
    //     parent.dampCount++;
    //     parent.humanIKController.logHomeOffset();
    //     parent.transform.position += forward2 * Vector3.Dot(delta, forward2)
    //                             + right2 * Vector3.Dot(delta, right2)
    //                             + Vector3.zero * Vector3.Dot(delta, plane);
    //     parent.humanIKController.postUpdateTowHandPosition();
    //     parent.lastDampStart = parent.target.position;
    //     Debug.DrawLine(parent.target.position, parent.dampDist, Color.red, Time.deltaTime);
    //     Debug.DrawLine(parent.target.position, parent.left.position, Color.blue, Time.deltaTime);
    //     Debug.DrawLine(parent.target.position, parent.right.position, Color.green, Time.deltaTime);
    // }
    
}