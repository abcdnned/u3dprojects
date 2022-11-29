using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class PlayerStats : CharacterStats
{

    public HealthBar healthBar;


    private void Awake() {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms) {
            string boneName = t.name.ToLower();
            if (boneName.Equals("hand.r")) {
                handPosition = t;
                break;
            }
        }
    }
    void Start() {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private int SetMaxHealthFromHealthLevel() {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public override bool TakeDamage(int damage, Collider sourceCollider, Collider targetCollider, int force) {
        if (dead || !vulnerable) return false;
        currentHealth = currentHealth - damage;
        healthBar.SetCurrentHeanlth(currentHealth);

        if (currentHealth <= 0) {
            currentHealth = 0;
            dead = true;
            animatorHandler.anim.SetBool("Dead", true);
            //Handle player death;
        }
        if (force >= 3) {
            transform.rotation = Utils.LookRotationFlat(transform.position, sourceCollider.transform.position);
            animatorHandler.anim.ResetTrigger("HitFly");
            animatorHandler.anim.SetTrigger("HitFly");
        }
        return true;
    }
}

}