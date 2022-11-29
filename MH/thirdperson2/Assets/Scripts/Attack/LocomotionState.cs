using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class LocomotionState : AttackState
{

    public override AttackState handleInput(bool boost, bool rb_input, bool rt_input) {
        if (rt_input) {
            Debug.Log("rt_input");
            anim.SetBool("ToAttack1", true);
            AttackState a = new Attack1();
            a.SetAnimator(anim);
            return a;
        } else if (rb_input) {
            Debug.Log("rb_input");
            anim.SetBool("ToAttack2", true);
            AttackState a = new Attack2();
            a.SetAnimator(anim);
            return a;
        }
        return this;
    }
}

}