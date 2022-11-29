using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot;
    WeaponHolderSlot rightHandSlot;

    DamageCollider rightHandDamageCollider;

    WeaponHolderSlot backSlot;

    PlayerInventory playerInventory;

    PlayerLocomotion playerLocomotion;

    private void Awake() {
        playerInventory = GetComponentInParent<PlayerInventory>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
            if (weaponSlot.isLeftHandSlot) {
                leftHandSlot = weaponSlot;
            } else if (weaponSlot.isRightHandSlot) {
                rightHandSlot = weaponSlot;
            } else {
                backSlot = weaponSlot;
            }
        }
    }

    public void UnloadWeapon() {
        rightHandSlot.UnloadWeaponAndDestroy();
        backSlot.UnloadWeaponAndDestroy();
    }

    public void SwitchToRightHand() {
        UnloadWeapon();
        LoadWeaponOnSlot(playerInventory.rightWeapon, 1);
        playerLocomotion.movementSpeed = 2.5f;
    }

    public void SnapToBack() {
        UnloadWeapon();
        LoadWeaponOnSlot(playerInventory.backWeapon, 2);
        playerLocomotion.movementSpeed = 5.0f;
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, int slot) {
        if (slot == 0) {
            leftHandSlot.LoadWeaponModel(weaponItem);
        } else if (slot == 1) {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();
        } else {
            backSlot.LoadWeaponModel(weaponItem);
        }
    }

    #region  Handle Weapon's Damage Collider;

    private void LoadRightWeaponDamageCollider() {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    public void OpenDamageCollider() {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider() {
        rightHandDamageCollider.DisableDamageCollider();
    }

    #endregion


}
}
