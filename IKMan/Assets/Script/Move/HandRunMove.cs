using System;
using UnityEngine;

public class HandRunMove : HandMove
{
    GameObject movingSphere;

    public HandRunMove() : base(MoveNameConstants.HandRunMove)
    {
    }

    public void init(GameObject ms) {
        movingSphere = ms;
    }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }
    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            state++;
        } 
        if (state == 1) {
        }
        return this;
    }

}
