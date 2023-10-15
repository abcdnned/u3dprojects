using System;
using UnityEngine;

public class LegRotateMove : LegMove
{
    private Vector3 angelOffset;

    private Transform pointer;
    public LegRotateMove() : base(MoveNameConstants.LegRotateMove) {
    }

    Rotater rotater;


    public override Move move(float dt) {
        duration = legController().shortStepDuration;
        if (state == 0) {
            state = 1;
            rotater = new Rotater(hic.walkPointer.transform,
                                  legController().transform,
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

    internal void setPointer(Transform pointer) {
        this.pointer = pointer;
    }
}