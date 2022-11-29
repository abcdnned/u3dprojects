using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;
    public WeaponItem rightWeapon;

    public WeaponItem leftWeapon;
    public WeaponItem backWeapon;
    private void Awake() {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();

    }

    private void Start() {
        // weaponSlotManager.LoadWeaponOnSlot(leftWeapon, 0);
        // weaponSlotManager.LoadWeaponOnSlot(rightWeapon, 1);
        weaponSlotManager.LoadWeaponOnSlot(backWeapon, 2);
    }
    
}
}