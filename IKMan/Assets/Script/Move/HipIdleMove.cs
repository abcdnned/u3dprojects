using UnityEngine;

public class HipIdleMove : HipMove
{
    float initTime;
    private float h;
    private float speed;
    private GameObject ph;


    public HipIdleMove() : base(MoveNameConstants.HipIdle) {
    }

    public override Move move(float dt) {
        // controller.ReturnToCenter();
        // controller.adjustLegDistance();
        // Vector3 h = controller.getDynamicHeight(controller.leftLeg.transform.position,
        //                                     controller.rightLeg.transform.position,
        //                                     controller.expectLegDistance);
        // controller.hic.spin3.localRotation= Quaternion.Slerp(controller.hic.spin3.localRotation,
        //                                                 Quaternion.identity,
        //                                                 1 - Mathf.Exp(-spin3speed * 2 * Time.deltaTime));
        base.move(dt);
        float h = calculateRealTimeHeight();

        controller.adjustHeight(h, controller.hic.gravityUp, controller.hipBattleSpeed);

        // controller.hic.spin2.position = ph.transform.position;
        // controller.hic.spin2.rotation = ph.transform.rotation;
        Quaternion r = Quaternion.Slerp(controller.hic.spin2.rotation,
                                        ph.transform.rotation,
                                        1 - Mathf.Exp(-10 * Time.deltaTime));
        controller.hic.spin2.rotation = r;
        return this;
    }
    // public override Move transfer() {
    //     if ((parent.leftLeg.move.IsLegMoving() && !parent.leftLeg.Recover)
    //         || (parent.rightLeg.move.IsLegMoving() && !parent.rightLeg.Recover)) {
    //         return moveManager.getMove(MoveNameConstants.HipDamp);
    //     } else if (
    //         (humanIKController.currentStatus.getName() == IdleStatus.NAME
    //         && humanIKController.currentStatus.cs.name == IdleStatus.STATE_TOBATTLEIDLE) ||
    //         (humanIKController.currentStatus.getName() == BattleIdleState.NAME
    //         && humanIKController.currentStatus.cs.name == BattleIdleState.STATE_BATTLE)) {
    //         return moveManager.getMove(MoveNameConstants.HipIdle2BattleIdle);
    //     } else {
    //         return this;
    //     }
    // }
    
    // private void keepBalanceWhenWalking() {
    //     Vector3 forward2 = Utils.forward(parent.target);
    //     Vector3 right2 = Utils.right(parent.target);
    //     Vector3 plane = Vector3.up;
    //     Vector3 tp = Vector3.Lerp(
    //     // transform.position = Vector3.Lerp(
    //                                     parent.target.position,
    //                                     parent.dampDist,
    //                                     1 - Mathf.Exp(-parent.finalDampSpeed * Time.deltaTime)
    //                                 );
    //     Vector3 delta = tp - parent.lastDampStart;
    //     parent.dampSum += delta.magnitude;
    //     parent.dampCount++;
    //     parent.humanIKController.logHomeOffset();
    //     parent.transform.position += forward2 * Vector3.Dot(delta, forward2)
    //                             + right2 * Vector3.Dot(delta, right2)
    //                             + Vector3.zero * Vector3.Dot(delta, plane);
    //     parent.humanIKController.postUpdateTowHandPosition();
    //     parent.lastDampStart = parent.target.position;
    //     Debug.DrawLine(parent.target.position, parent.dampDist, Color.red, Time.deltaTime);
    //     Debug.DrawLine(parent.target.position, parent.left.position, Color.blue, Time.deltaTime);
    //     Debug.DrawLine(parent.target.position, parent.right.position, Color.green, Time.deltaTime);
    // }

    internal void initBasic(float t) {
        this.initTime = t;
    }
    
    public override void init() {
        base.init();
        ph = attachIdleJoint();
        // ph = PrefabCreator.CreatePrefab(controller.hic.spin2.position, "SpinHelper", controller.hic.spin1.transform.rotation);
        // ph.AddComponent<SphereCollider>();
        // ph.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        // CharacterJoint newJoint = ph.AddComponent<CharacterJoint>();
        // newJoint.anchor = new Vector3(0, -controller.hic.spin2.localPosition.y, -0.01f);
        // newJoint.connectedBody = controller.hic.spin1.GetComponent<Rigidbody>();
        // newJoint.autoConfigureConnectedAnchor = true;
        // newJoint.axis = new Vector3(1,0,0);
        // newJoint.swingAxis = new Vector3(0,1,0);
        // SoftJointLimitSpring sls = new SoftJointLimitSpring();
        // sls.spring = .73f;
        // newJoint.twistLimitSpring = sls;
        // SoftJointLimit l = new SoftJointLimit();
        // l.limit = -.1f;
        // newJoint.lowTwistLimit = l;
        // SoftJointLimit h = new SoftJointLimit();
        // h.limit = 0;
        // newJoint.highTwistLimit = h;
        // SoftJointLimit s1 = new SoftJointLimit();
        // s1.limit = 0f;
        // newJoint.swing1Limit = s1;
        // SoftJointLimit s2 = new SoftJointLimit();
        // s2.limit = 0f;
        // newJoint.swing2Limit = s2;
    }

    private float calculateRealTimeHeight() {
        int floor = Mathf.FloorToInt((Time.time - initTime) / controller.hic.ap.idleBreathTime);
        float fract = (Time.time - initTime) - (floor * controller.hic.ap.idleBreathTime);
        float edge = controller.hic.ap.idleBreathTime / 3;
        int mod = fract <= edge ? 1 : 0;
        // float th = mod == 1 ? controller.hic.ap.standHeight : controller.hic.ap.idleDownHeight; 
        float normal = mod == 1 ? (fract / edge) : ((fract - edge) / (edge * 2));
        float th = mod == 1 ? Mathf.Lerp(controller.hic.ap.idleDownHeight, controller.hic.ap.standHeight, normal)
                        : Mathf.Lerp(controller.hic.ap.standHeight, controller.hic.ap.idleDownHeight, normal);
        return th;
    }
}