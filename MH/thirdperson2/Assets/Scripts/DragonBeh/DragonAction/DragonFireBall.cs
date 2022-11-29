using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TY {
public class DragonFireBall : ActionCandidate {

    public DragonFireBall(Animator animator) : base (animator){
    }

    public override float[] EffectiveRange() {
        return new float[] { 10.0f, 22.5f };
    }

    public override float[] EffectiveAngle() {
        return new float[] { -60f, 60f };
    }

    public override string GetName() {
        return "DragonFireBall";
    }
    
    public override string GetCategory() {
        return "RangeAttack";
    }

    public override void TakeAction(float dis, float angle) {
        animator.ResetTrigger("FireBall");
        animator.SetTrigger("FireBall");
        // Debug.Log(this.GetType().Name + " dis " + dis);
    }

}
}