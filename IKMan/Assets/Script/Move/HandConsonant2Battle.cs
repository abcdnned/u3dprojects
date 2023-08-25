using System;
using UnityEngine;

public class HandConsonant2Battle : HandMove
{
    protected float duration2;

    protected GameObject greateSworad;

    public HandConsonant2Battle() : base(MoveNameConstants.HandConsonant2Battle)
    {
    }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }
    public void initBasic(float duration,
                          float duration2
                          ) {
        this.duration = duration;
        this.duration2 = duration2;
    }

    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            normalizedTime = 0;
            state++;
            handController.SyncIKSample(IKSampleNames.IDLE_SAMPLE, duration, true);
        }
        if (state == 1) {
            handController.LookToArmLook();
            if (normalizedTime > duration) {
                state++;
                handController.SyncIKSample(IKSampleNames.SWING_RIGHT_HAND_ON_LEFT, duration2, true);
                handController.handLookIKController.init(duration2,
                                                        humanIKController.consonantHandle.position,
                                                        humanIKController.body.transform,
                                                        humanIKController.consonantHandle,
                                                        true);
            }
        }
        if (state == 2) {
            handController.LookToArmLook();
            if (normalizedTime > duration + duration2) {
                state = 3;
                // return moveManager.ChangeMove(MoveNameConstants.MainHoldWeaponIdle);
            }
        }
        return this;
    }

    public override bool stateFinish() {
        return state >= 3;
    }

}
