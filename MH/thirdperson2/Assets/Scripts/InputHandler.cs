using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool b_Input;

    public bool rt_Input;
    public bool rb_Input;
    public bool ry_Input;
    public bool rollFlag;

    public bool esc_Input;

    public bool comboFlag;


    public float rollInputTimer;

    public bool sprintFlag;

    private bool selectWindowFlag = true;

    PlayerContorls inputActions;

    PlayerAttack playerAttack;
    PlayerInventory playerInventory;

    PlayerManager playerManager;
    Vector2 movementInput;
    Vector2 cameraInput;

    PlayerLocomotion playerLocomotion;

    UIManager uimanager;

    private void Awake() {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAttack = GetComponent<PlayerAttack>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
        uimanager = FindObjectOfType<UIManager>();
    }
    
    public void OnEnable() {
        if (inputActions == null) {
            inputActions = new PlayerContorls();
            inputActions.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i => rt_Input = true;
            inputActions.PlayerActions.RY.performed += i => ry_Input = true;
            inputActions.PlayerActions.Esc.performed += i => esc_Input = true;
        }
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }
    
    public void TickInput(float delta) {
        MoveInput(delta);
        HandleRollInput(delta);
        HandleAttackInput(delta);
        HandleInventoryInput(delta);
    }

    private void MoveInput(float delta) {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollInput(float delta) {
        b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        if (b_Input) {
            rollInputTimer += delta;
            sprintFlag = true;
        } else {
            if(rollInputTimer > 0 && rollInputTimer < 0.25f) {
                sprintFlag = false;
                rollFlag = true;
            }
            rollInputTimer = 0;
        }
    }

    private void HandleAttackInput(float delta) {
        // if (rb_Input) {
        //     if (playerManager.canDoCombo) {
        //         // Debug.Log("inputHandle Combo");
        //         comboFlag = true;
        //         playerAttack.HandleWeaponCombo(playerInventory.rightWeapon);
        //         comboFlag = false;
        //     } else {
        //         if (playerManager.isInteracting) {
        //             return;
        //         }
        //         playerAttack.HandleLightAttack(playerInventory.rightWeapon);
        //     }
        // }

        // if (rt_Input) {
        //     if (playerManager.isInteracting) {
        //         return;
        //     }
        //     playerAttack.HandleHeavyAttack(playerInventory.rightWeapon);
        // }
        if (playerManager.canDoCombo) {
            // Debug.Log("inputHandle Combo");
            comboFlag = true;
            playerAttack.HandleAttackInput(false, rb_Input, rt_Input, ry_Input);
            comboFlag = false;
        } else {
            if (playerManager.isInteracting) {
                return;
            }
            playerAttack.HandleAttackInput(false, rb_Input, rt_Input, ry_Input);
        }
    }

    private void HandleInventoryInput(float delta) {
        if(esc_Input) {
            selectWindowFlag = !selectWindowFlag;
            if (selectWindowFlag) {
                uimanager.enableSelectWindow();
            } else {
                uimanager.disableSelectWindow();
            }
        }
    }

}
}
