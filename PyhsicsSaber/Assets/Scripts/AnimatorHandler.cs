using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class AnimatorHandler : MonoBehaviour
{
    // int walkSpeed;
    public Animator anim;
    private void Awake() {
        anim = GetComponent<Animator>();
    }
    public void UpdateWalkSpeed(float walkSpeed) {
        float v = 0;
        if (walkSpeed > 0) {
            v = 1f;
        } else {
            v = 0;
        }
        anim.SetFloat("WalkSpeed", v);
    }
    public void PlayTargetAnimation(string targetAnim) {
        anim.CrossFade(targetAnim, 0.2f);
    }
}
}