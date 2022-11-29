using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class GuardIdleBehaviour : DragonBeh
{

    protected ActionHolder actionHolder = new ActionHolder();
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyStats.dead) return;
        // if (canRotate) {
            HandleRotateTowardsTarget();
        // }
        if (instantBeh) {
            instantBeh = false;
            // Debug.Log(this.GetType().Name + " instanBeh ");
        } else if (Mathf.CeilToInt(Time.frameCount) % 100 != 0) {
            return;
        }
        HandleDetection();
        if (Context.currentTarget != null) {
            float dis = Vector3.Distance(Context.currentTarget.transform.position, Context.transform.position);
            if (dis > Context.chasingDistance) {
                // Debug.Log(this.GetType().Name + " chasing ");
                animator.SetBool("IsChasing", true);
            } else if (dis < Context.backingDistance) {
                // Debug.Log(this.GetType().Name + " backing ");
                animator.SetBool("IsBacking", true);
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
                    // Debug.Log(this.GetType().Name + " need adjust position ");
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
                Vector3 targetDirection = characterStats.transform.position - Context.transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, Context.transform.forward);
                if (viewableAngle > Context.minimumDectionAngle && viewableAngle < Context.maximumDetectionAngle) {
                    Context.currentTarget = characterStats;
                    Debug.Log("Target detected");
                }
            }
        }
    }
    protected override void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.ResetTrigger("DA1");
        DragonAttack1 a1 = new DragonAttack1(animator);
        DragonAttack3 a3 = new DragonAttack3(animator);
        DragonFireBall df = new DragonFireBall(animator);
        FlyAction fa = new FlyAction(animator);
        // actionHolder.AddAction(a1);
        // actionHolder.AddAction(a3);
        actionHolder.AddAction(fa);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        canRotate = false;
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