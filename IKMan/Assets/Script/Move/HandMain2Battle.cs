using System;
using UnityEngine;

public class HandMain2Battle : HandMove
{
    private Quaternion end;
    protected float alpha;
    protected float beta;
    protected float gamma;
    protected float alpha2;
    protected float beta2;
    protected float gamma2;
    protected int initStepCount = 0;
    protected Transform targetPosition;
    protected Transform targetPosition2;
    protected int lr;
    protected float pivotOffset;
    protected Steper steper;
    protected Steper steper2;
    // protected EularRotater rotater;
    // protected EularRotater rotater2;
    protected float duration2;
    protected Transform handHint;
    // private float hintTargetDegree;
    public HandMain2Battle() : base(MoveNameConstants.HandMain2Battle)
    {
    }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }

    public void initTargetRotation(float a, float b, float g) {
        alpha = a;
        beta = b;
        gamma = g;
        initStepCount++;
    }
    public void initTargetRotation2(float a, float b, float g) {
        alpha2 = a;
        beta2 = b;
        gamma2 = g;
        initStepCount++;
    }
    public void initBasic(float duration,
                          float duration2,
                          Transform targetPosition,
                          Transform targetPosition2,
                          int lr,
                          float pivotOffset) {
        this.duration = duration;
        initStepCount++;
        this.targetPosition = targetPosition;
        this.targetPosition2 = targetPosition2;
        this.pivotOffset = pivotOffset;
        this.lr = lr;
        this.duration2 = duration2;
    }

    public void initHint(Transform handHint) {
        this.handHint = handHint;
        // this.hintTargetDegree = targetDegree;
        initStepCount++;
    }

    public override void beReady() {
        // Init steper
        Vector3 wp1 = handController.transform.position;
        Vector3 wp3 = targetPosition.position;
        Vector3 wp2 = getMoveCenter(wp1, wp3);
        steper = new Steper(Utils.forwardFlat(handController.body.transform),
                            Utils.right(handController.body.transform),
                            duration,
                            Steper.BEARZ,
                            handController.body.transform,
                            0,
                            handController.transform,
                            new Vector3[] {wp1, wp2, wp3});
        // Init rotater
        // end = Quaternion.Euler(alpha, beta, gamma);
        // rotater = new EularRotater(parent.body.transform, parent.transform, duration, new Vector3(alpha, beta, gamma));
        // rotater.setTargetRotation(end);
        initStepCount++;
    }

    protected void initStep2() {
        // Init steper
        Vector3 wp1 = handController.transform.position;
        Vector3 wp3 = targetPosition2.position;
        Vector3 wp2 = getMoveCenter(wp1, wp3);
        steper2 = new Steper(Utils.forwardFlat(handController.body.transform),
                            Utils.right(handController.body.transform),
                            duration2,
                            Steper.BEARZ,
                            handController.body.transform,
                            0,
                            handController.transform,
                            new Vector3[] {wp1, wp2, wp3});
        // Init rotater
        // end = Quaternion.Euler(alpha2, beta2, gamma2);
        // rotater2 = new EularRotater(parent.body.transform, parent.transform,
        //                        duration2, new Vector3(alpha2, beta2, gamma2));
        // rotater2.setTargetRotation(end);
        initStepCount++;
    }

    private Vector3 getMoveCenter(Vector3 wp1, Vector3 wp3)
    {
        Vector3 wp2 = (wp1 + wp3) / 2;
        wp2 += pivotOffset * Utils.right(handController.body.transform) * lr;
        return wp2;
    }

    // public override Move move(float dt) {
    //     if (initStepCount < 5) {
    //         throw new Exception("Please init first.");
    //     }
    //     if (state == 0) {
    //         state = 1;
    //         parent.HandLook.init(duration,
    //                              parent.m2b_hangel,
    //                              parent.m2b_vangel);
    //     } else if (state == 1) {
    //         normalizedTime += dt;
    //         parent.LookToHandLook(-Utils.right(parent.body.transform));
    //         if (normalizedTime > duration) {
    //             state = 2;
    //             initStep2();
    //             parent.handHint.hAd = parent.m2b_elbow;
    //             parent.HandLook.setDuration(duration2);
    //             parent.HandLook.setAngel(parent.m2b_battle_h,
    //                                      parent.m2b_battle_v);
    //         } else {
    //             steper.step(dt);
    //         }
    //     } else if (state == 2) {
    //         normalizedTime += dt;
    //         parent.LookToHandLook(Utils.forward(parent.body.transform) * -1);
    //         if (normalizedTime > duration + duration2) {
    //             state = 3;
    //             return moveManager.ChangeMove(MoveNameConstants.MainHoldWeaponIdle);
    //         } else {
    //             steper2.step(dt);
    //         }
    //     }
    //     return this;
    // }

    public override Move move(float dt) {
        if (initStepCount < 5) {
            throw new Exception("Please init first.");
        }
        if (state == 0) {
            state = 1;
            handController.HandLook.init(duration,
                                 handController.m2b_hangel,
                                 handController.m2b_vangel);
            handController.HandElbow.init(duration,
                                  90,
                                  0);
            handController.HandFK.init(duration,
                                  0,
                                  90);
            handController.handHint.enable = false;
        } else if (state == 1) {
            normalizedTime += dt;
            handController.LookToHandLook(-handController.getArmDirection());
            // handController.updateHintByFK();
            if (normalizedTime > duration) {
                state = 2;
                initStep2();
                handController.handHint.hAd = handController.m2b_elbow;
                handController.HandLook.init(duration2,
                                     handController.m2b_battle_h,
                                     handController.m2b_battle_v);
                handController.HandElbow.init(duration2,
                                      0,
                                      -60);
                handController.HandFK.init(duration2,
                                      0,
                                      30);
            } else {
                // steper.step(dt);
                // handController.transform.position = handController.HandFK.transform.position;
            }
        } else if (state == 2) {
            normalizedTime += dt;
            handController.LookToHandLook(-handController.getArmDirection());
            // handController.updateHintByFK();
            if (normalizedTime > duration + duration2) {
                state = 3;
                return moveManager.ChangeMove(MoveNameConstants.MainHoldWeaponIdle);
            } else {
                // steper2.step(dt);
                // handController.transform.position = handController.HandFK.transform.position;
            }
        }
        return this;
    }
}
