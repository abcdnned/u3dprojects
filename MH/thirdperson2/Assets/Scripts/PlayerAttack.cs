using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class PlayerAttack : MonoBehaviour
{
    AnimatorHandler animatorHandler;

    InputHandler inputHandler;

    PlayerManager playerManager;

    PlayerLocomotion playerLocomotion;

    public string lastAttack;
    private void Awake() {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        inputHandler = GetComponent<InputHandler>();
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    public void HandleAttackInput(bool bost, bool rb_Input, bool rt_input, bool ry_input) {
        animatorHandler.anim.ResetTrigger("AttackX");
        animatorHandler.anim.ResetTrigger("SnapY");
        animatorHandler.anim.ResetTrigger("AttackA");
        if (rt_input) {
            animatorHandler.anim.SetTrigger("AttackX");
        } else if (rb_Input) {
            animatorHandler.anim.SetTrigger("AttackA");
        } else if (ry_input) {
            animatorHandler.anim.SetTrigger("SnapY");
        }
    }

    public void HandleWeaponCombo(WeaponItem weapon) {
        if (inputHandler.comboFlag) {
            animatorHandler.anim.SetBool("canDoCombo", false);
            // if (lastAttack == weapon.OH_Heavey_Attack_1) {
            //     animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
            //     lastAttack = weapon.OH_Light_Attack_1;
            // } else if (lastAttack == weapon.OH_Light_Attack_1) {
                // animatorHandler.PlayTargetAnimation(weapon.OH_Heavey_Attack_1, true);
            //     lastAttack = weapon.OH_Heavey_Attack_1;
            // }
        }
    }
    public void HandleLightAttack(WeaponItem weapon) {
        animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
        lastAttack = weapon.OH_Light_Attack_1;
    }
    public void HandleHeavyAttack(WeaponItem weapon) {
        animatorHandler.PlayTargetAnimation(weapon.OH_Heavey_Attack_1, true);
        lastAttack = weapon.OH_Heavey_Attack_1;
    }
}

}