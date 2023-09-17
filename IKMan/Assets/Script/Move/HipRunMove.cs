using System;
using UnityEngine;

public class HipRunMove : HipMove
{
    // GameObject movingSphere;

    // Vector3 offset;
    private float h;
    private float speed;
    private GameObject ph;



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
        controller.adjustHeight(h, controller.hic.gravityUp, speed);
        controller.hic.spin2.position = ph.transform.position;
        // controller.hic.spin2.rotation = ph.transform.rotation;
        Quaternion r = Quaternion.Slerp(controller.hic.spin2.rotation,
                                        ph.transform.rotation,
                                        1 - Mathf.Exp(-10 * Time.deltaTime));
        controller.hic.spin2.rotation = r;
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
        return this;
    }

    // public void SyncPosition() {
    //     if (movingSphere != null) {
    //         humanIKController.transform.position = offset + movingSphere.transform.position;
    //     }
    // }

    internal void onLegBeats(int code) {
        // Debug.Log(" code " + code);
        float groundHeight = controller.hic.ap.runUpHeight;
        if (code == 0) {
            groundHeight = controller.hic.ap.runDownHeigth;
        }
        float hipMoveSpeed = controller.hipHeightDiff(groundHeight, controller.hic.gravityUp) / controller.hic.ap.runHalfDuration;
        h = groundHeight;
        speed = hipMoveSpeed;
    }

    public override void init() {
        base.init();
        ph = PrefabCreator.CreatePrefab(controller.hic.spin2.position, "SpinHelper");
        ph.AddComponent<SphereCollider>();
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
