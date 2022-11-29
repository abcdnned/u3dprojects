using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class Attack1 : AttackState
{
    public override AttackState handleInput(bool boost, bool rb_input, bool rt_input) {
        if (rb_input) {
            anim.SetBool("ToAttack2", true);
            AttackState a = new Attack2();
            a.SetAnimator(anim);
            return a;
        }
        return this;
    }
}

}