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
    protected EularRotater rotater;
    protected EularRotater rotater2;
    protected float duration2;
    protected Transform handHint;
    // private float hintTargetDegree;
    public HandMain2Battle() : base(MoveNameConstants.HandMain2Battle)
    {
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
        Vector3 wp1 = parent.transform.position;
        Vector3 wp3 = targetPosition.position;
        Vector3 wp2 = getMoveCenter(wp1, wp3);
        steper = new Steper(Utils.forwardFlat(parent.body.transform),
                            Utils.right(parent.body.transform),
                            duration,
                            Steper.BEARZ,
                            parent.body.transform,
                            0,
                            parent.transform,
                            new Vector3[] {wp1, wp2, wp3});
        // Init rotater
        // end = Quaternion.Euler(alpha, beta, gamma);
        rotater = new EularRotater(parent.body.transform, parent.transform, duration, new Vector3(alpha, beta, gamma));
        // rotater.setTargetRotation(end);
        initStepCount++;
    }

    protected void initStep2() {
        // Init steper
        Vector3 wp1 = parent.transform.position;
        Vector3 wp3 = targetPosition2.position;
        Vector3 wp2 = getMoveCenter(wp1, wp3);
        steper2 = new Steper(Utils.forwardFlat(parent.body.transform),
                            Utils.right(parent.body.transform),
                            duration2,
                            Steper.BEARZ,
                            parent.body.transform,
                            0,
                            parent.transform,
                            new Vector3[] {wp1, wp2, wp3});
        // Init rotater
        // end = Quaternion.Euler(alpha2, beta2, gamma2);
        rotater2 = new EularRotater(parent.body.transform, parent.transform,
                               duration2, new Vector3(alpha2, beta2, gamma2));
        // rotater2.setTargetRotation(end);
        initStepCount++;
    }

    private Vector3 getMoveCenter(Vector3 wp1, Vector3 wp3)
    {
        Vector3 wp2 = (wp1 + wp3) / 2;
        wp2 += pivotOffset * Utils.right(parent.body.transform) * lr;
        return wp2;
    }

    public override Move move(float dt) {
        if (initStepCount < 5) {
            throw new Exception("Please init first.");
        }
        if (state == 0) {
            state = 1;
        } else if (state == 1) {
            normalizedTime += dt;
            if (normalizedTime > duration) {
                state = 2;
                initStep2();
                parent.handHint.hAd = parent.m2b_elbow;
            } else {
                steper.step(dt);
                // parent.transform.rotation = parent.arm.rotation;
                rotater.rot(dt);
            }
        } else if (state == 2) {
            normalizedTime += dt;

            if (normalizedTime > duration + duration2) {
                state = 3;
                return moveManager.ChangeMove(MoveNameConstants.MainHoldWeaponIdle);
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
