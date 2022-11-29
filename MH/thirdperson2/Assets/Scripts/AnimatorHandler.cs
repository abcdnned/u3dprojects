using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class AnimatorHandler : AnimatorManager
{
    int vertical;
    int horizontal;

    PlayerManager playerManager;
    InputHandler inputHandler;

    PlayerLocomotion playerLocomotion;

    Rigidbody rb;

    public bool canRotate;
    public void Initialize() {
        anim = GetComponent<Animator>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        rb = GetComponentInParent<Rigidbody>();
        playerManager = GetComponentInParent<PlayerManager>();
        vertical = Animator.StringToHash("Vertical");
        inputHandler = GetComponentInParent<InputHandler>();
        horizontal = Animator.StringToHash("Horizontal");
    }
    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting) {
        #region Vertacal
        float v = 0;
        if (verticalMovement > 0 && verticalMovement < 0.55f) {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f) {
            v = 1;
        } else if (verticalMovement < 0 && verticalMovement > -0.55f) {
            v = -0.5f;
        } else if (verticalMovement < -0.55f) {
            v = -1;
        } else {
            v = 0;
        }
        #endregion


        #region Horizontal
        float h = 0;
        if (horizontalMovement > 0 && horizontalMovement < 0.55f) {
            h = 0.5f;
        } else if (horizontalMovement > 0.55f) {
            h = 1f;
        } else if (horizontalMovement < 0 && horizontalMovement > -0.55f) {
            h = -0.5f;
        } else if (horizontalMovement < -0.55f) {
            h = -1f;
        } else {
            h = 0;
        }
        #endregion

        if (isSprinting) {
            v = 2;
            h = horizontalMovement;
        }
        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void CanRotate() {
        canRotate = true;
    }

    public void StopRotatoin() {
        canRotate = false;
    }

    // private void OnAnimatorMove() {

        // if (playerManager.isInteracting == false)
        //     return;

        // Debug.Log("OnAnimatorMove");
        // float delta = Time.deltaTime;

        // playerLocomotion.rigidbody.drag = 0; 
        // Vector3 deltaPosition = anim.deltaPosition;
        // deltaPosition.y = 0;
        // Vector3 velocity = deltaPosition / delta;
        // playerLocomotion.rigidbody.velocity = velocity;
    // }

    public void EnableCombo() {
        anim.SetBool("canDoCombo", true);
        // Debug.Log("animi canDOCOmbo " + anim.GetBool("canDoCombo"));
    }

    public void DisableCombo() {
        anim.SetBool("canDoCombo", false);
        // Debug.Log("animi canDOCOmbo " + anim.GetBool("canDoCombo"));
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