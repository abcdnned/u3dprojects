using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class LandingBeh : DragonBeh {

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dragonTarget.transform.position.y < 0.2) {
            animator.ResetTrigger("LandOn");
            animator.SetTrigger("LandOn");
        }
    }
    protected override void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        enemyLocomotionManager.fly = false;
        enemyLocomotionManager.antiGravity = false;
    }


}

}