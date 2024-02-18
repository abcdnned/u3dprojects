using System;
using UnityEngine;

public class HipSwingRotateMove : HipMove
{

    private Quaternion spin1TargetRotation;
    private Vector3 origin_forward;
    private Vector3 origin_vert;
    private Vector3 start_dir;
    public const float DURATION = 2;

    public const float ANGEL = 360;
    public HipSwingRotateMove() : base(MoveNameConstants.HipSwingRotateMove)
    {
    }

    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            origin_forward = hic.spin1.transform.forward;
            origin_vert = hic.spin1.transform.up;
            state++;
        } else if (state == 1) {
            if (normalizedTime > duration) {
                state++;
            } else {
                float angel = Mathf.Lerp(0, ANGEL, normalizedTime / duration);
                spin1TargetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(angel, -origin_vert) * origin_forward);
                controller.hic.spin1.transform.rotation= Quaternion.Slerp(controller.hic.spin1.transform.rotation,
                                                                spin1TargetRotation,
                                                                1 - Mathf.Exp(-10 * Time.deltaTime));
            }
        }
        // rotateByMovingController();
        return this;
    }

    internal void rotateAngelChange(Vector3 forward) {
        // if (start_dir == null) {
        //     start_dir = forward;
        // }
        // Debug.Log(" forward " + forward);
        // float angel = Vector3.Angle(start_dir, forward);
        // Vector3 target = Quaternion.AngleAxis(angel, Vector3.Cross(start_dir, forward)) * origin_forward;
        // spin1TargetRotation = Quaternion.LookRotation(target, hic.gravityUp);
    }

}
