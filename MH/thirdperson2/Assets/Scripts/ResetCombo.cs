using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCombo : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.SetBool("canDoCombo", false);
    }
}
