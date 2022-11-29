using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class DragonBeh : StateMachineBehaviour<DragonTarget>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    protected NavMeshAgent navMeshAgent;

    protected Rigidbody rigidbody;

    protected Animator anim;

    protected DragonCollider dragonCollider;

    protected DragonTarget dragonTarget;
    protected bool canRotate = true;

    protected static bool instantBeh = false;

    protected long frameIndex;
    protected EnemyStats enemyStats;

    protected EnemyLocomotionManager enemyLocomotionManager;

    protected override void OnInitialize(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        navMeshAgent = Context.GetComponentInChildren<NavMeshAgent>();
        rigidbody = Context.GetComponent<Rigidbody>();
        dragonCollider = Context.GetComponentInParent<DragonCollider>();
        dragonTarget = Context.GetComponentInParent<DragonTarget>();
        enemyStats = Context.GetComponentInParent<EnemyStats>();
        enemyLocomotionManager = Context.GetComponentInParent<EnemyLocomotionManager>();
        anim = animator;
    }

    protected void HandleRotateTowardsTarget() {
        if (Context.currentTarget != null) {
            Vector3 v = Context.currentTarget.transform.position - Context.transform.position;
            v.y = 0;
            v.Normalize();
            Quaternion q = Quaternion.LookRotation(v.normalized);
            Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, q, Context.rotationSpeed * Time.deltaTime);
        }
    }
    protected void HandleRotateTowardsTarget2() {
        if (Context.currentTarget != null) {
            Vector3 relativeDirection = Context.transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = rigidbody.velocity;
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(Context.currentTarget.transform.position);
            rigidbody.velocity = targetVelocity;
            Context.transform.rotation = Quaternion.Slerp(Context.transform.rotation, navMeshAgent.transform.rotation, Context.rotationSpeed / Time.deltaTime);
        }
    }

    protected override void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        frameIndex = 0;
        OnStateEnteredSub(animator, stateInfo, layerIndex);
    }

    protected virtual void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state

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