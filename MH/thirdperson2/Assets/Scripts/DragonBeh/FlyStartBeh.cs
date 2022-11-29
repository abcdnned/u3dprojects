using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class FlyStartBeh : DragonBeh {
    bool fired = false;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime;
        t = t - Mathf.Floor(t);
        if (t >= 0.33 && !fired) {
            fired = true;
            enemyLocomotionManager.fly = true;
            enemyLocomotionManager.antiGravity = true;
        }
    }
    protected override void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        fired = false;
    }


}

}