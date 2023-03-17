using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipBattleIdleMove : Move
{
    WalkBalance parent;
    public HipBattleIdleMove() : base(MoveNameConstants.HipBattleIdle) {
    }
    public override void init() {
        parent = (WalkBalance)targetController;
        normalizedTime = 0;
        state = 0;
    }
    public override Move move(float dt) {
        if (state == 0) {
            state = 1;
        }
        normalizedTime += dt;
        parent.transfer(parent.battleIdleAngelOffset);
        parent.adjustHeight(parent.battleIdleHipH, Vector3.up,
                            parent.hipBattleSpeed);
        return this;
    }
}
