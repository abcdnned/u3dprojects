using System;
using UnityEngine;

public class HipRunMove : HipMove
{
    // GameObject movingSphere;

    // Vector3 offset;
    private float h;
    private float speed;
    private GameObject ph;

    private float spin3speed = 10;
    private Quaternion spin3TargetRotation;



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
        // Debug.Log(" hip run ");
        normalizedTime += dt;
        controller.adjustHeight(h, controller.hic.gravityUp, speed);
        controller.hic.spin2.position = ph.transform.position;
        // controller.hic.spin2.rotation = ph.transform.rotation;
        Quaternion r = Quaternion.Slerp(controller.hic.spin2.rotation,
                                        ph.transform.rotation,
                                        1 - Mathf.Exp(-10 * Time.deltaTime));
        controller.hic.spin2.rotation = r;

        if (spin3TargetRotation != null) {
            int c = controller.hic.spin3.childCount;
            // Debug.Log(" child count " + c);
            // Transform[] childs = new Transform[1];
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
            // for (int i = 0; i < c; ++i) {
            //     if (childs[i] != null) {
            //         childs[i].SetParent(controller.hic.spin3);
            //     }
            // }
        }
        // Debug.Log(" nor " + normalizedTime);
        // if (state == 0) {
        //     state++;
        // } 
        // if (state == 1) {
        //     SyncPosition();
        // }
        // if (movingSphere == null) {
        //     return moveManager.ChangeMove(MoveNameConstants.HipIdle);
        // }
        // Update rotation based on camera.
        Vector2 m = controller.hic.inputArgument.movement;
        if (m.magnitude > 0) {
            Vector3 dir = Utils.forwardFlat(controller.cam) * m.y + Utils.right(controller.cam) * m.x;
            controller.justRotateHip(dir, 0, controller.hic.ap.hipTrackCameraSpeed);
        }
        return this;
    }

    // public void SyncPosition() {
    //     if (movingSphere != null) {
    //         humanIKController.transform.position = offset + movingSphere.transform.position;
    //     }
    // }

    internal void onLegBeats(int code, bool isRight) {
        // Debug.Log(" code " + code);
        float groundHeight = controller.hic.ap.runUpHeight;
        if (code == 0) {
            spin3TargetRotation = Quaternion.identity;
            groundHeight = controller.hic.ap.runDownHeigth;
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
        ph = PrefabCreator.CreatePrefab(controller.hic.spin2.position, "SpinHelper");
        ph.AddComponent<SphereCollider>();
        ph.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        CharacterJoint newJoint = ph.AddComponent<CharacterJoint>();
        newJoint.anchor = new Vector3(0, -controller.hic.spin2.localPosition.y, -0.01f);
        newJoint.connectedBody = controller.hic.spin1.GetComponent<Rigidbody>();
        newJoint.autoConfigureConnectedAnchor = true;
        newJoint.axis = new Vector3(1,0,0);
        newJoint.swingAxis = new Vector3(0,1,0);
        SoftJointLimitSpring sls = new SoftJointLimitSpring();
        sls.spring = 2.16f;
        newJoint.twistLimitSpring = sls;
        SoftJointLimit l = new SoftJointLimit();
        l.limit = -10;
        newJoint.lowTwistLimit = l;
        SoftJointLimit h = new SoftJointLimit();
        h.limit = -5;
        newJoint.highTwistLimit = h;
        SoftJointLimit s1 = new SoftJointLimit();
        s1.limit = 0f;
        newJoint.swing1Limit = s1;
        SoftJointLimit s2 = new SoftJointLimit();
        s2.limit = 0f;
        newJoint.swing2Limit = s2;
    }

    public override void finish() {
        if (ph != null) {
            GameObject.Destroy(ph);
        }
    }


}
