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
            parent.handHint.hAd = parent.m2b_idle_elbow;
        } else if (state == 1) {
            normalizedTime += dt;
            if (normalizedTime > duration) {
                state = 2;
                initStep2();
            } else {
                steper.step(dt);
                // parent.transform.rotation = parent.arm.rotation;
                rotater.rot(dt);
            }
        } else if (state == 2) {
            normalizedTime += dt;

            if (normalizedTime > duration + duration2) {
                state = 3;
                return moveManager.ChangeMove(MoveNameConstants.HandIdle);
            } else {
                steper2.step(dt);
                rotater2.rot(dt);
                // statellite.rot(dt);
            }

        }
        // move to weapon handle and then to the weapon hold position
        return this;
    }

}
