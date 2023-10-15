
using UnityEngine;

public class HipMove : Move
{
    protected float spin3speed = 10;
    protected WalkBalance controller;

    protected GameObject[] helpers;
    protected int helperCount = 0;



    public HipMove(string name) : base(name) {

    }
    public override void init() {
        controller = (WalkBalance)targetController;
        normalizedTime = 0;
        state = 0;
        duration = controller.battleIdleTransferDuration;
        helpers = new GameObject[10];
    }

    protected GameObject attachIdleJoint() {
        GameObject ph = PrefabCreator.CreatePrefab(controller.hic.spin2.position, "SpinHelper", controller.hic.spin1.transform.rotation);
        ph.AddComponent<SphereCollider>();
        ph.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        CharacterJoint newJoint = ph.AddComponent<CharacterJoint>();
        newJoint.anchor = new Vector3(0, -controller.hic.spin2.localPosition.y, -0.01f);
        newJoint.connectedBody = controller.hic.spin1.GetComponent<Rigidbody>();
        newJoint.autoConfigureConnectedAnchor = true;
        newJoint.axis = new Vector3(1,0,0);
        newJoint.swingAxis = new Vector3(0,1,0);
        SoftJointLimitSpring sls = new SoftJointLimitSpring();
        sls.spring = .73f;
        newJoint.twistLimitSpring = sls;
        SoftJointLimit l = new SoftJointLimit();
        l.limit = -.1f;
        newJoint.lowTwistLimit = l;
        SoftJointLimit h = new SoftJointLimit();
        h.limit = 0;
        newJoint.highTwistLimit = h;
        SoftJointLimit s1 = new SoftJointLimit();
        s1.limit = 0f;
        newJoint.swing1Limit = s1;
        SoftJointLimit s2 = new SoftJointLimit();
        s2.limit = 0f;
        newJoint.swing2Limit = s2;
        helpers[helperCount] = ph;
        helperCount++;
        return ph;
    }

    public override void finish() {
        for (int i = 0; i < helperCount; ++i) {
            if (helpers[i] != null) {
                GameObject.Destroy(helpers[i]);
                helpers[i] = null;
            }
        }
        helperCount = 0;
    }

    public override Move move(float dt) {
        controller.hic.spin3.localRotation= Quaternion.Slerp(controller.hic.spin3.localRotation,
                                                        Quaternion.identity,
                                                        1 - Mathf.Exp(-spin3speed * 2 * Time.deltaTime));
        return this;
    }

    protected void rotateToCamera() {
        Vector2 m = controller.hic.inputArgument.movement;
        if (m.magnitude > 0) {
            Vector3 dir = Utils.forwardFlat(controller.cam) * m.y + Utils.right(controller.cam) * m.x;
            controller.justRotateHip(dir, 0, controller.hic.ap.hipTrackCameraSpeed);
        }
    }
}