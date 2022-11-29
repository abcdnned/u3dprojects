using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class EnemyStats : CharacterStats
{
    public ParticleSystem[] hitParticlePrefab;
    ParticleSystem[] ps;
    public HealthBar healthBar;

    private void Awake() {
        animatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        maxHealth = 1000;
        currentHealth = 1000;
        ps = new ParticleSystem[hitParticlePrefab.Length];
        for (int i = 0; i < hitParticlePrefab.Length; ++i) {
            ps[i] = new ParticleSystem();
        }
        if (hitParticlePrefab != null)
        {
            for (int i = 0; i < hitParticlePrefab.Length; ++i) {
                ps[i] = Instantiate(hitParticlePrefab[i]);
                ps[i].Stop();
            }
        }
        if (healthBar != null) {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHeanlth(maxHealth);
        }
    }
    void Start() {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    private int SetMaxHealthFromHealthLevel() {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public override bool TakeDamage(int damage, Collider sourceCollider, Collider targetCollider, int force) {
        if (dead || !vulnerable) return false;
        Debug.Log(this.GetType().Name + " Hit Name " + targetCollider.gameObject.name);
        if (targetCollider.gameObject.name.Equals("Head")) {
            currentHealth = currentHealth - 3 * damage;
        } else {
            currentHealth = currentHealth - 1 * damage;
        }
        for (int i = 0; i < ps.Length; ++i) {
            ps[i].transform.position = targetCollider.transform.position;
            ps[i].time = 0;
            ps[i].Play();
        }
        if (currentHealth <= 0) {
            currentHealth = 0;
            animatorManager.anim.SetBool("Dead", true);
            dead = true;
        }
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHeanlth(currentHealth);
        return true;
    }
}

}