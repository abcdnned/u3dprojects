using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    public bool isInteracting;

    public bool isInAir;
    public bool isGrounded;

    public AttackState attackState;
    
    CameraHandler cameraHandler;
    CameraHandlerV2 cameraHandlerV2;
    PlayerLocomotion playerLocomotion;
    [Header("Player Flags")]
    public bool isSprinting;
    public bool canDoCombo;

    public Vector3 InteractingDir;
    public Vector3 InteractingDirR;

    public Transform InteractingTransform;

    private PlayerIventory iventory;

    public Item[] initialItems;

    private void Awake() {
        cameraHandlerV2 = FindObjectOfType<CameraHandlerV2>();
        cameraHandler = CameraHandler.singleton;
    }

    private void FixedUpdate() {
        float delta = Time.fixedDeltaTime;
        if (cameraHandler != null) {
            cameraHandler.FollowTarget(delta);
            // cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
        // if (cameraHandlerV2 != null) {
        //     cameraHandlerV2.CameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        // }
        // playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleAirDrag();
    }

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        attackState = new LocomotionState();
        attackState.SetAnimator(anim);
        playerLocomotion = GetComponent<PlayerLocomotion>();
        iventory = GetComponent<PlayerIventory>();
        for (int i = 0; i < initialItems.Length; ++i) {
            if (i < iventory.limit) {
                Item itm = Instantiate(initialItems[i]);
                iventory.AddItem(itm, i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateAttackState();
        bool shadowII = isInteracting;
        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");
        float delta = Time.deltaTime;
        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        playerLocomotion.HandleRollingAndSprinting(delta);
        if (cameraHandler != null) {
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
        // cameraHandlerV2.CameraRotation();
    }

    void LateUpdate() {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;
        inputHandler.ry_Input = false;
        inputHandler.esc_Input = false;
        if (isInAir) {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }
    private void updateAttackState() {
        if (anim.GetAnimatorTransitionInfo(1).IsName("New State")) {
            attackState = new LocomotionState();
            attackState.SetAnimator(anim);
            Debug.Log("reset attack state");
        }
    }
    public void AttackShake() {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(1.5f, 2, 0.1f, 0.30f);
        Debug.Log("AttackShake");
    }
    public void ApplyHitCameraShake() {
        EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.Bump);
    }
}

}