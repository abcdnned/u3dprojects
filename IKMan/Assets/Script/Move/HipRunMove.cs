using System;
using UnityEngine;

public class HipRunMove : HipMove
{
    // GameObject movingSphere;

    // Vector3 offset;
    private float h;
    private float speed;
    private Quaternion spin3TargetRotation;
    private GameObject pivot;
    private GameObject ph;
    private float pivotYOffset;

    public HipRunMove() : base(MoveNameConstants.HipRunMove)
    {
    }

    // public void init(GameObject ms, Vector3 offset) {
    //     movingSphere = ms;
    //     this.offset = offset;
    // }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }
    public override Move move(float dt) {
        normalizedTime += dt;
        controller.adjustGroundedHeight(h, controller.hic.gravityUp, speed);
        if (pivot != null) {
            updatePivot();
            // updateSpin2();
        }
        if (spin3TargetRotation != null) {
            int c = controller.hic.spin3.childCount;
            Transform neck = null;
            for (int i = 0; i < c; ++i) {
                Transform child = controller.hic.spin3.GetChild(i);
                if (child.name == controller.hic.neck.name) {
                    child.SetParent(null);
                    neck = child;
                    break;
                }
            }
            controller.hic.spin3.localRotation= Quaternion.Slerp(controller.hic.spin3.localRotation,
                                                            spin3TargetRotation,
                                                            1 - Mathf.Exp(-spin3speed * Time.deltaTime));
            if (neck != null) {
                neck.SetParent(controller.hic.spin3);
            }
        }
        rotateToCamera();
        return this;
    }

    internal void onLegBeats(int code, bool isRight) {
        // Debug.Log(" code " + code);
        float groundHeight = controller.hic.ap.runUpHeight;
        if (code == 0) {
            spin3TargetRotation = Quaternion.identity;
            groundHeight = controller.hic.ap.runDownHeight;
        } else if (code == 1) {
            float angel = isRight ? -controller.hic.ap.runSpin3Angel : controller.hic.ap.runSpin3Angel;
            spin3TargetRotation =  Quaternion.identity * Quaternion.AngleAxis(angel, controller.hic.gravityUp);
        }
        float hipMoveSpeed = controller.hipHeightDiff(groundHeight, controller.hic.gravityUp) / controller.hic.ap.runHalfDuration;
        h = groundHeight;
        speed = hipMoveSpeed;
    }

    public override void init() {
        base.init();
        (GameObject, GameObject)v = attachRunJoint();
        pivot = v.Item1;
        ph = v.Item2;
        pivotYOffset = pivot.transform.position.y - hic.spin1.transform.position.y;
    }

    public override void finish() {
        base.finish();
        // hic.spin2.transform.position = hic.spin1.transform.position + hic.spin2Offset;
    }

    private void updateSpin2() {
        Vector3 offset = ph.transform.position - pivot.transform.position;
        float x = Vector3.Dot(offset, pivot.transform.right);
        float y = Vector3.Dot(offset, pivot.transform.up);
        float z = Vector3.Dot(offset, pivot.transform.forward);
        hic.spin2.position = hic.spin1.transform.position
                             + x * hic.spin1.transform.right
                             + y * hic.spin1.transform.up
                             + z * hic.spin1.transform.forward;
        Quaternion diff = QuaternionExtensions.Diff(pivot.transform.rotation, ph.transform.rotation);
        Quaternion target = QuaternionExtensions.Add(hic.spin1.transform.rotation, diff);
        // Quaternion r = Quaternion.Slerp(hic.spin2.rotation,
        //                                 target,
        //                                 1 - Mathf.Exp(-10 * Time.deltaTime));
        // hic.spin2.rotation = r;
        hic.spin2.rotation = target;
    }

    private void updatePivot() {
        Vector3 np = Utils.copy(pivot.transform.position);
        np.y = hic.ap.runUpHeight - hic.spin1.transform.position.y + pivotYOffset;
        pivot.transform.position = np;
    }
}
