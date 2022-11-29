using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TY {
public class ActionCandidate : Object {

    protected Animator animator;
    
    public ActionCandidate(Animator animator) {
        this.animator = animator;
    }
    
    public virtual float[] EffectiveRange() {
        return new float[] { 7f, 10f };
    }

    public virtual float[] EffectiveAngle() {
        return new float[] { -15f, 15f };
    }

    public virtual string GetName() {
        return "Unnamed";
    }
    
    public virtual string GetCategory() {
        return "UnknowType";
    }

    public virtual void TakeAction(float dis, float angle) {
    }
}
}