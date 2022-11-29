using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class ThrowBeh : HeroBeh {

    bool triggered = false;

    GameObject missle;

    protected override void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        triggered = false;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime;
        t = t - Mathf.Floor(t);
        if (t > 0.57 && !triggered) {
            triggered = true;
            missle = Instantiate(stats.handy.missle);
            MissleBeh missleBeh = missle.GetComponent<MissleBeh>();
            HashSet<CharacterStats> damaged = new HashSet<CharacterStats>();
            missle.transform.position = stats.handPosition.position;
            missleBeh.targetPosition = Context.transform.position + Context.transform.forward * 2f;
            missleBeh.damaged = damaged;
            missleBeh.sphereTrigger = false;
            missleBeh.setTrigger();
            Vector3 dir = Context.transform.forward;
            dir.y = 0;
            missleBeh.Shoot(8f, dir.normalized);
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