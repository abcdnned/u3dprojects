using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
    public class AttackState

    {
        protected Animator anim;

        public void SetAnimator(Animator a) {
            anim = a;
        }

        public virtual AttackState handleInput(bool boost, bool rb_input, bool rt_input) {
            return this;
        }
    }
}