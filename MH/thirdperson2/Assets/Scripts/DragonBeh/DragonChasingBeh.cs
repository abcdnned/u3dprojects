using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class DragonChasingBeh : DragonBeh {

    private int chasingCount = 1000;

    protected override void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        chasingCount = 1000;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandleMoveToTarget(animator);
    }

    public void HandleMoveToTarget(Animator animator) {
        CharacterStats currentTarget = Context.currentTarget;
        chasingCount--;
        if (!animator.GetBool("IsChasing") || currentTarget == null
            || chasingCount < 1) {
            animator.SetBool("IsChasing", false);
            return;
        }
        Transform transform = Context.transform;
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        float distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        bool shouldChasing = true;
        if (distanceFromTarget <= Context.chasingDistance) {
            shouldChasing = false;
        }
        if (shouldChasing) {
            Vector3 projectedVelocity = transform.forward * 5;
            projectedVelocity.y = 0;
            rigidbody.velocity = projectedVelocity;
        } else {
            Context.chasingDistance = Context.chasingDistance + 10f;
            rigidbody.velocity = Vector3.zero;
            animator.SetBool("IsChasing", false);
            instantBeh = true;
        }

        HandleRotateTowardsTarget();

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
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