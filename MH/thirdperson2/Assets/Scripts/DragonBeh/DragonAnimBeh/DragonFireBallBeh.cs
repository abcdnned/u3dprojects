using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class DragonFireBallBeh : DragonBeh {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks

    ParticleSystem fireBall;

    bool fired = false;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime;
        t = t - Mathf.Floor(t);
        if (t >= 0.51 && !fired && !dragonTarget.currentTarget.dead && dragonTarget.currentTarget != null) {
            dragonTarget.attackType = DragonTarget.ATTACK_BITE;
            fireBall = Instantiate(dragonTarget.fireBall);
            MissleBeh bigColBeh = fireBall.GetComponent<MissleBeh>();
            HashSet<CharacterStats> damaged = new HashSet<CharacterStats>();
            bigColBeh.targetPosition = dragonTarget.currentTarget.transform.position;
            bigColBeh.transform.position = dragonTarget.headTransform.position;
            bigColBeh.damaged = damaged;
            bigColBeh.Shoot();
            fired = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        fired = false;
    }
}

}