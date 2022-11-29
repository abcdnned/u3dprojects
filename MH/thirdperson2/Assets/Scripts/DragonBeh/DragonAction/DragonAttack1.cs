using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TY {
public class DragonAttack1 : ActionCandidate {

    public DragonAttack1(Animator animator) : base (animator){
    }

    public override float[] EffectiveRange() {
        return new float[] { 8.0f, 9.5f };
    }

    public override float[] EffectiveAngle() {
        return new float[] { -15f, 15f };
    }

    public override string GetName() {
        return "DragonAttack1";
    }
    
    public override string GetCategory() {
        return "MeeleAttack";
    }

    public override void TakeAction(float dis, float angle) {
        animator.ResetTrigger("DA1");
        animator.SetTrigger("DA1");
        // Debug.Log(this.GetType().Name + " dis " + dis);
    }

}
}