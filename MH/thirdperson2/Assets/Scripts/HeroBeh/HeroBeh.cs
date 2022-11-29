using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {
public class HeroBeh : StateMachineBehaviour<PlayerManager>
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    protected CharacterStats stats;

    protected override void OnInitialize(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        stats = Context.GetComponent<CharacterStats>();
    }


    protected override void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        OnStateEnteredSub(animator, stateInfo, layerIndex);
    }

    protected virtual void OnStateEnteredSub(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }
}

}