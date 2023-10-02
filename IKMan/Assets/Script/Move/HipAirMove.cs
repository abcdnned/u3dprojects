using UnityEngine;

public class HipAirMove : HipMove
{
    // float initTime;
    // private GameObject ph;


    public HipAirMove() : base(MoveNameConstants.HipAirMove) {
    }

    public override Move move(float dt) {
        base.move(dt);
        // Quaternion r = Quaternion.Slerp(controller.hic.spin2.rotation,
        //                                 ph.transform.rotation,
        //                                 1 - Mathf.Exp(-10 * Time.deltaTime));
        // controller.hic.spin2.rotation = r;
        rotateToCamera();
        return this;
    }

    // internal void initBasic(float t) {
    //     this.initTime = t;
    // }
    
    public override void init() {
        base.init();
        // ph = attachIdleJoint();
    }


}