using System;
using UnityEngine;

public class HipRunMove : HipMove
{
    GameObject movingSphere;

    Vector3 offset;

    public HipRunMove() : base(MoveNameConstants.HipRunMove)
    {
    }

    public void init(GameObject ms, Vector3 offset) {
        movingSphere = ms;
        this.offset = offset;
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
            SyncPosition();
        }
        if (movingSphere == null) {
            return moveManager.ChangeMove(MoveNameConstants.HipIdle);
        }
        return this;
    }

    public void SyncPosition() {
        if (movingSphere != null) {
            humanIKController.transform.position = offset + movingSphere.transform.position;
        }
    }

}
