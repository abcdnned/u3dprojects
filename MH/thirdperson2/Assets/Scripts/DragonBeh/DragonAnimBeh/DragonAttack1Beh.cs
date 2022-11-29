using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class DragonAttack1Beh : DragonBeh {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks

    GameObject bite;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime;
        t = t - Mathf.Floor(t);
        if (t > 0.24 && t < 0.50 && dragonTarget.attackType != DragonTarget.ATTACK_BITE) {
            dragonTarget.attackType = DragonTarget.ATTACK_BITE;
            bite = Instantiate(dragonTarget.biteCol) as GameObject;
            ColliderBeh colliderBeh = bite.GetComponent<ColliderBeh>();
            HashSet<CharacterStats> damaged = new HashSet<CharacterStats>();
            colliderBeh.Init(50, null, dragonTarget.headTransform, 3, damaged);
        }
        if (t > 0.50 && dragonTarget.attackType != DragonTarget.ATTACK_NONE) {
            dragonTarget.attackType = DragonTarget.ATTACK_NONE;
            Destroy(bite);
        }
    }


    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

}