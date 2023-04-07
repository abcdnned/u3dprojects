using System;
using UnityEngine;

public class HandMain2Battle : HandMove
{
    private Quaternion start;
    private Quaternion end;
    private float alpha;
    private float beta;
    private float gamma;
    private int initStepCount = 0;
    private Vector3 targetPosition;
    private int lr;
    private float pivotOffset;
    private Steper steper;
    public HandMain2Battle() : base(MoveNameConstants.HandMain2Battle)
    {
    }

    public void initTargetRotation(float a, float b, float g) {
        alpha = a;
        beta = b;
        gamma = g;
        initStepCount++;
    }
    public void initBasic(float duration, Vector3 targetPosition, int lr, float pivotOffset) {
        this.duration = duration;
        initStepCount++;
        this.targetPosition = targetPosition;
        this.pivotOffset = pivotOffset;
        this.lr = lr;
    }

    public override void beReady() {
        end = Quaternion.Euler(alpha, beta, gamma);
        Vector3 wp1 = parent.transform.position;
        Vector3 wp3 = targetPosition;
        Vector3 wp2 = getMoveCenter(wp1, wp3);
        steper = new Steper(Utils.forward(parent.body.transform),
                            Utils.right(parent.body.transform),
                            duration,
                            Steper.BEARZ,
                            parent.body.transform,
                            0,
                            parent.transform,
                            new Vector3[] {wp1, wp2, wp3});
        initStepCount++;
    }

    private Vector3 getMoveCenter(Vector3 wp1, Vector3 wp3)
    {
        Vector3 wp2 = (wp1 + wp3) / 2;
        wp2 += pivotOffset * Utils.right(parent.body.transform) * lr;
        return wp2;
    }

    public override Move move(float dt) {
        if (initStepCount < 3) {
            throw new Exception("Please init first.");
        }
        if (state == 0) {
            state = 1;
        } else {
            steper.step(dt);
        }
        // move to weapon handle and then to the weapon hold position
        return this;
    }

}
