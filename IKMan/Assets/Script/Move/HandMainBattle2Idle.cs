using System;
using UnityEngine;

public class HandMainBattle2Idle : HandMain2Battle
{
    public HandMainBattle2Idle() : base()
    {
        name = MoveNameConstants.HandMainBattle2Idle;
    }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }

    public override Move move(float dt) {
        if (state == 0) {
            state = 1;
            // handController.handHint.hAd = handController.m2b_idle_elbow;
            handController.HandElbow.init(duration,
                                  90,
                                  0);
            handController.HandFK.init(duration,
                                  0,
                                  90);
            handController.handHint.enable = false;
        } else if (state == 1) {
            normalizedTime += dt;
            if (normalizedTime > duration) {
                state = 2;
                handController.HandElbow.init(duration2,
                                      90,
                                      -80);
                handController.HandFK.init(duration2,
                                      -10,
                                      -10);
            }
        } else if (state == 2) {
            normalizedTime += dt;
            if (normalizedTime > duration + duration2) {
                state = 3;
                return moveManager.ChangeMove(MoveNameConstants.HandBattleIdle);
            }

        }
        // move to weapon handle and then to the weapon hold position
        return this;
    }

}
