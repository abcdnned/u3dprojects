using System;
using UnityEngine;

public class LegPutMove : LegMove
{
    private bool isRightFoot;

    private Vector3 targetPosition;
    private Vector3 angelOffset;
    public LegPutMove(bool isRightFoot) : base(MoveNameConstants.LegPutMove) {
        this.isRightFoot = isRightFoot;
    }

    protected override void subinit()
    {
        duration = parent.shortStepDuration;
        
    }
    public override void beReady()
    {
        Transform target = parent.transform;
        Vector3 wp1 = target.position;
        Vector3 wp3 = targetPosition;
        Vector3 wp2 = Utils.GetMiddleLiftPoint(wp1, wp3, parent.shortStepLiftDistance);
        steper = new Steper(Utils.forward(parent.body.transform),
                            Utils.right(parent.body.transform),
                            parent.shortStepDuration,
                            Steper.BEARZ,
                            parent.body.transform,
                            0,
                            parent.transform,
                            new Vector3[] {wp1, wp2, wp3});
        rotater = new Rotater(parent.body.transform, parent.transform,
                                    duration,
                                    angelOffset);
    }

    Steper steper;

    Rotater rotater;


    public override Move move(float dt) {
        // if (state == 0) {
        //     state = 1;
        //     Transform target = parent.transform;
        //     Vector3 wp1 = target.position;
        //     Vector3 wp3 = targetPosition;
        //     Vector3 wp2 = Utils.GetMiddleLiftPoint(wp1, wp3, parent.shortStepLiftDistance);
        //     // Vector3 realForward, realRight;
        //     // Utils.GetForwardAndRight(wp1, wp3, out realForward, out realRight);
        //     steper = new Steper(Utils.forward(parent.body.transform),
        //                         Utils.right(parent.body.transform),
        //                         parent.shortStepDuration,
        //                         Steper.BEARZ,
        //                         parent.body.transform,
        //                         // realForward,
        //                         // realRight,
        //                         0,
        //                         parent.transform,
        //                         new Vector3[] {wp1, wp2, wp3});
        //     rotater = new Rotater(parent.body.transform, parent.transform,
        //                                 duration,
        //                                 angelOffset);
        // }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            // Debug.Log(this.GetType().Name + " change to LegIdle ");
            return moveManager.ChangeMove(MoveNameConstants.LegIdle);
        } else {
            // Debug.Log(" leg put move ");
            steper.step(dt);
            rotater.rot(dt);
        }
        return this;
    }

    internal void setTargetPosition(Vector3 wp3, Vector3 angelOffset)
    {
        targetPosition = wp3;
        this.angelOffset = angelOffset;
    }
}