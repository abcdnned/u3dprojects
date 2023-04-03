using System;
using UnityEngine;

public class LegRotateMove : LegMove
{
    private Vector3 angelOffset;
    public LegRotateMove() : base(MoveNameConstants.LegRotateMove) {
    }

    protected override void subinit()
    {
        duration = parent.shortStepDuration;
    }

    Rotater rotater;


    public override Move move(float dt) {
        if (state == 0) {
            state = 1;
            rotater = new Rotater(parent.body.transform, parent.transform,
                                        duration,
                                        angelOffset);
        }
        normalizedTime += dt;
        if (normalizedTime > duration) {
            state = 2;
            // Debug.Log(this.GetType().Name + " change to LegIdle ");
            return moveManager.ChangeMove(MoveNameConstants.LegIdle);
        } else {
            rotater.rot(dt);
        }
        return this;
    }

    internal void setTargetPosition(Vector3 angelOffset)
    {
        this.angelOffset = angelOffset;
    }
}