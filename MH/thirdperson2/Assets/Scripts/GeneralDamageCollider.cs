using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class GeneralDamageCollider : MonoBehaviour
{
    Collider damageCollider;

    public int currentWeaponDamage = 25;

    private void Awake() {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider() {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider() {
        damageCollider.enabled = false;
    }

    void OnCollisionEnter(Collision collision) {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        Debug.Log("OnCollisionEnter");
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag == "Hittable") {
            if (collision.gameObject.layer == 9) {
                AttackShake();
                Debug.Log("Hit Terrian");
            } else {
                Debug.Log("Hit!!!");
                ApplyHitCameraShake();
                DisableDamageCollider();
                EnemyStats enemyStats = collision.GetComponentInParent<EnemyStats>();
                if (enemyStats != null) {
                    enemyStats.TakeDamage(currentWeaponDamage, null, collision, 1);
                }
            }
        }
    }

    private void AttackShake() {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(1.5f, 2, 0.1f, 0.30f);
        Debug.Log("AttackShake");
    }
    private void ApplyHitCameraShake() {
        EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.Bump);
    }
}

}