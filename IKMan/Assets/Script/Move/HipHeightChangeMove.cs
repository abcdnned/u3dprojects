
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
            hipMoveSpeed = controller.hipHeightDiff(groundHeight, Vector3.up) / duration;
        }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            return moveManager.ChangeMove(MoveNameConstants.HipIdle);
        } else {
            controller.transfer(targetRotation);
            controller.adjustHeight(groundHeight, Vector3.up, hipMoveSpeed);
        }
        controller.ReturnToCenter();
        return this;
    }
}