using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TY {
public class FlyAction : ActionCandidate {

    public FlyAction(Animator animator) : base (animator){
    }

    public override float[] EffectiveRange() {
        return new float[] { 0f, 22.5f };
    }

    public override float[] EffectiveAngle() {
        return new float[] { -180f, 180f };
    }

    public override string GetName() {
        return "FlyAction";
    }
    
    public override string GetCategory() {
        return "Move";
    }

    public override void TakeAction(float dis, float angle) {
        animator.ResetTrigger("FlyToAir");
        animator.SetTrigger("FlyToAir");
    }

}
}