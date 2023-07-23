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
        if (initStepCount < 4) {
            throw new Exception("Please init first.");
        }
        if (state == 0) {
            state = 1;
            // handController.HandLook.setDuration(duration);
            // handController.HandLook.setAngel(handController.m2b_hangel,
            //                             handController.m2b_vangel);
            handController.handHint.hAd = handController.m2b_idle_elbow;
            handController.HandElbow.init(duration,
                                  90,
                                  0);
            handController.HandFK.init(duration,
                                  0,
                                  90);
            handController.handHint.enable = false;
        } else if (state == 1) {
            normalizedTime += dt;
            // handController.LookToHandLook(-handController.getArmDirection());
            // handController.updateHintByFK();
            if (normalizedTime > duration) {
                state = 2;
                // handController.HandLook.setDuration(duration2);
                // handController.HandLook.setAngel(0, 0);
                handController.HandElbow.init(duration2,
                                      90,
                                      -80);
                handController.HandFK.init(duration2,
                                      -10,
                                      -10);
                initStep2();
            } else {
                // steper.step(dt);
                // handController.transform.position = handController.HandFK.transform.position;
                // parent.transform.rotation = parent.arm.rotation;
                // rotater.rot(dt);
            }
        } else if (state == 2) {
            // handController.LookToHandLook(-handController.getArmDirection());
            // handController.updateHintByFK();
            normalizedTime += dt;

            if (normalizedTime > duration + duration2) {
                state = 3;
                return moveManager.ChangeMove(MoveNameConstants.HandBattleIdle);
            } else {
                // steper2.step(dt);
                // handController.transform.position = handController.HandFK.transform.position;
                // rotater2.rot(dt);
                // statellite.rot(dt);
            }

        }
        // move to weapon handle and then to the weapon hold position
        return this;
    }

}
