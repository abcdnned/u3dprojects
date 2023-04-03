
using UnityEngine;

public class HipHeightChangeMove : HipMove
{
    private float hipMoveSpeed;

    public float groundHeight;
    public float targetRotation;
    public HipHeightChangeMove() : base(MoveNameConstants.HipHeightChangeMove) {

    }
    public override Move move(float dt) {
        if (state == 0) {
            state = 1;
            hipMoveSpeed = parent.hipHeightDiff(groundHeight, Vector3.up) / duration;
        }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            return moveManager.ChangeMove(MoveNameConstants.HipIdle);
        } else {
            parent.transfer(targetRotation);
            parent.adjustHeight(groundHeight, Vector3.up, hipMoveSpeed);
        }
        parent.ReturnToCenter();
        return this;
    }
}