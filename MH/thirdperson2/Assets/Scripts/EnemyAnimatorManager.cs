using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class EnemyAnimatorManager : AnimatorManager
{
    EnemyLocomotionManager enemyLocomotionManager;
    Rigidbody rb;

    private void Awake() {
        anim = GetComponent<Animator>();
        enemyLocomotionManager = GetComponentInParent<EnemyLocomotionManager>();
        rb = GetComponentInParent<Rigidbody>();
    }

    public void SetWanderStat(bool v) {
        anim.SetBool("IsWandering", v);
    }

    private void OnAnimatorMove() {
        // float delta = Time.deltaTime;
        // enemyLocomotionManager.enemyRigidBody.drag = 0;
        // Vector3 deltaPosition = anim.deltaPosition;
        // deltaPosition.y = 0;
        // Vector3 velocity = deltaPosition / delta;
        // enemyLocomotionManager.enemyRigidBody.velocity = velocity;
    }
    public void AnimForwardMove(float v) {
        Vector3 direction = transform.forward * v;
        rb.AddForce(direction, ForceMode.VelocityChange);
    }

    public void AnimStop() {
        Vector3 v = new Vector3(-rb.velocity.x, -rb.velocity.y, -rb.velocity.z);
        rb.AddForce(v, ForceMode.VelocityChange);
    }

}

}