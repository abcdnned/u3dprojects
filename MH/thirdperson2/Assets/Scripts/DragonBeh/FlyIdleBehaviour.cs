using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class FlyIdleBehaviour : DragonBeh
{

    protected ActionHolder actionHolder = new ActionHolder();

    private int latestAction;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyStats.dead) return;
        // if (canRotate) {
            HandleRotateTowardsTarget();
        // }
        if (instantBeh) {
            instantBeh = false;
            // Debug.Log(this.GetType().Name + " instanBeh ");
        } else if ((Time.frameCount - latestAction) < 500) {
            return;
        }
        latestAction = Time.frameCount;
        HandleDetection();
        if (Context.currentTarget != null) {
            float dis = Vector3.Distance(Context.currentTarget.transform.position, Context.transform.position);
            Debug.Log(this.GetType().Name + " dis " + dis);
            if (dis > 20) {
                // Debug.Log(this.GetType().Name + " chasing ");
                animator.SetBool("IsChasing", true);
                Context.chasingDistance = 20;
                Context.backingDistance = 10;
            } else if (dis < 10) {
                Debug.Log(this.GetType().Name + " <10 ");
                animator.SetBool("IsBacking", true);
                Context.chasingDistance = 20;
                Context.backingDistance = 10;
            } else {
                float angle = AngleUtils.AngleBetween(Context.transform, Context.currentTarget.transform);
                bool needAdjustPosition;
                ActionCandidate ac = actionHolder.PickAction(dis, angle, out needAdjustPosition);
                if (!needAdjustPosition) {
                    ac.TakeAction(dis, angle);
                } else {
                    float[] er = ac.EffectiveRange();
                    Context.backingDistance = er[0];
                    Context.chasingDistance = er[1];
                    instantBeh = true;
                }
            }
        }
    }

    private void HandleDetection() {
        Collider[] colliders = Physics.OverlapSphere(Transform.position, Context.detectionRadius, Context.detectionLayer);

        if (Context.currentTarget != null) {
            if (Vector3.Distance(Context.currentTarget.transform.position, Context.transform.position) > 2 * Context.detectionRadius) {
                Context.currentTarget = null;
                // Debug.Log(this.GetType().Name + " Target canceled");
            }
            else if (Context.currentTarget.dead) {
                Context.currentTarget = null;
            }
            return;
        }
        for (int i = 0; i < colliders.Length; i++) {
            PlayerStats characterStats = colliders[i].transform.GetComponentInParent<PlayerStats>();
            if (characterStats != null && characterStats.dead == false) {
                Context.currentTarget = characterStats;
                Debug.Log("Target detected");
            }
        }
    }
    protected override void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        enemyLocomotionManager.fly = true;
        enemyLocomotionManager.antiGravity = true;
        // animator.ResetTrigger("DA1");
        // DragonAttack1 a1 = new DragonAttack1(animator);
        // DragonAttack3 a3 = new DragonAttack3(animator);
        LandingAction la = new LandingAction(animator);
        actionHolder.AddAction(la);
        // actionHolder.AddAction(a3);
        // actionHolder.AddAction(df);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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