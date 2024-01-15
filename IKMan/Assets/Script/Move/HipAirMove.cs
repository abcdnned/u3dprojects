using UnityEngine;

public class HipAirMove : HipMove
{

    public HipAirMove() : base(MoveNameConstants.HipAirMove) {
    }

    public override Move move(float dt) {
        base.move(dt);
        // rotateToCamera();
        return this;
    }
    
    public override void init() {
        base.init();
    }

}