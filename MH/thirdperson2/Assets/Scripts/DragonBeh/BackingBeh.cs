using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class BackingBeh : DragonBeh {
    protected override void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        HandleBackToTarget(animator);
    }

    public void HandleBackToTarget(Animator animator) {
        if (!animator.GetBool("IsBacking")) return;
        CharacterStats currentTarget = Context.currentTarget;
        Transform transform = Context.transform;
        float distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        bool shouldBacking = true;
        if (distanceFromTarget >= Context.backingDistance) {
            shouldBacking = false;
        }
        if (shouldBacking) {
            Vector3 projectedVelocity = transform.forward * -2.5f;
            projectedVelocity.y = 0;
            rigidbody.velocity = projectedVelocity;
        } else {
            Context.backingDistance = 0;
            animator.SetBool("IsBacking", false);
            rigidbody.velocity = Vector3.zero;
        }

        HandleRotateTowardsTarget();

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
    }

}

}