using System;
using UnityEngine;

public class HandMainBattle2Idle : HandMain2Battle
{
    public HandMainBattle2Idle() : base()
    {
        name = MoveNameConstants.HandMainBattle2Idle;
    }


    public override Move move(float dt) {
        if (initStepCount < 4) {
            throw new Exception("Please init first.");
        }
        if (state == 0) {
            state = 1;
            parent.HandLook.setDuration(duration);
            parent.HandLook.setAngel(parent.m2b_hangel,
                                        parent.m2b_vangel);
            parent.handHint.hAd = parent.m2b_idle_elbow;
            parent.HandElbow.init(duration,
                                  90,
                                  0);
            parent.HandFK.init(duration,
                                  0,
                                  90);
        } else if (state == 1) {
            normalizedTime += dt;
            parent.LookToHandLook(Utils.right(parent.body.transform));
            if (normalizedTime > duration) {
                state = 2;
                parent.HandLook.setDuration(duration2);
                parent.HandLook.setAngel(0, 0);
                parent.HandElbow.init(duration2,
                                      0,
                                      -90);
                parent.HandFK.init(duration2,
                                      0,
                                      0);
                initStep2();
            } else {
                steper.step(dt);
                // parent.transform.rotation = parent.arm.rotation;
                // rotater.rot(dt);
            }
        } else if (state == 2) {
            parent.LookToHandLook(Vector3.up);
            normalizedTime += dt;

            if (normalizedTime > duration + duration2) {
                state = 3;
                return moveManager.ChangeMove(MoveNameConstants.HandIdle);
            } else {
                steper2.step(dt);
                // rotater2.rot(dt);
                // statellite.rot(dt);
            }

        }
        // move to weapon handle and then to the weapon hold position
        return this;
    }

}
