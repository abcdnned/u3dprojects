using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class DragonAttack3Beh : DragonBeh {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks

    GameObject[] bite;

    GameObject big;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime;
        t = t - Mathf.Floor(t);
        if (t >= 0.26 && t <= 0.62 && dragonTarget.attackType != DragonTarget.ATTACK_BITE) {
            bite = new GameObject[8];
            dragonTarget.attackType = DragonTarget.ATTACK_BITE;
            HashSet<CharacterStats> damaged = new HashSet<CharacterStats>();
            for (int i = 0; i < 8; ++i) {
                bite[i] = Instantiate(dragonTarget.tailCol) as GameObject;
                ColliderBeh colliderBeh = bite[i].GetComponent<ColliderBeh>();
                colliderBeh.Init(30, null, dragonTarget.tailTransform[i], 3, damaged);
            }
            big = Instantiate(dragonTarget.tailBigCol) as GameObject;
            ColliderBeh bigColBeh = big.GetComponent<ColliderBeh>();
            bigColBeh.Init(30, null, dragonTarget.tailTransform[7], 3, damaged);
        }
        if (t > 0.62 && dragonTarget.attackType != DragonTarget.ATTACK_NONE) {
            dragonTarget.attackType = DragonTarget.ATTACK_NONE;
            for (int i = 0; i < 8; ++i) {
                Destroy(bite[i]);
            }
            Destroy(big);
        }
        if (t >= 0.26 && t <= 0.62) {
            dragonCollider.transform.Rotate(new Vector3(0, -220, 0) * Time.deltaTime);
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