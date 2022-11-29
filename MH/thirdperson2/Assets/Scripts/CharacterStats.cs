using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {

public class CharacterStats : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;
    internal Item handy;

    internal Transform handPosition;

    internal GameObject inHand;

    internal bool vulnerable = true;
    protected AnimatorHandler animatorHandler;

    protected EnemyAnimatorManager animatorManager;
    private void Awake() {
    }

    internal bool dead = false;
    public virtual bool TakeDamage(int damage, Collider sourceCollider, Collider targetCollider, int force) {
        return true;
    }

    public bool CanBeHit() {
        return !dead && vulnerable;
    }
}

}