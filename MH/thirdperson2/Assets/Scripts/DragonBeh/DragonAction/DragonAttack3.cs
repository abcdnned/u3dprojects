using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TY {
public class DragonAttack3 : ActionCandidate {

    public DragonAttack3(Animator animator) : base (animator){
    }

    public override float[] EffectiveRange() {
        return new float[] { 7.0f, 9.5f };
    }

    public override float[] EffectiveAngle() {
        return new float[] { -60f, 60f };
    }

    public override string GetName() {
        return "DragonAttack3";
    }
    
    public override string GetCategory() {
        return "MeeleAttack";
    }

    public override void TakeAction(float dis, float angle) {
        animator.ResetTrigger("DA3");
        animator.SetTrigger("DA3");
        // Debug.Log(this.GetType().Name + " dis " + dis);
    }

}
}