using UnityEngine;

public class HandMovingMove : HandMove
{
    public HandMovingMove() : base(MoveNameConstants.HandMoving)
    {
        // additional initialization code, if any
    }
    protected override void subinit() {
        // if (handController.HandElbow != null) {
        //     handController.HandElbow.horizonAngel = 90;
        //     handController.HandElbow.verticalAngel = -80;
        // }
    }

    public override string getMoveType() {
        return AdvanceIKController.FIK;
    }
}
