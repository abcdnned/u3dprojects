using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class PlayerLocomotion : MonoBehaviour
{
    public GameObject CameraTarget;
    Transform cameraObject;
    InputHandler inputHandler;
    public Vector3 moveDirection;
    // Start is called before the first frame update
    [HideInInspector]
    public Transform myTransform;

    [HideInInspector]
    public AnimatorHandler animatorHandler;

    public new Rigidbody rigidbody;
    PlayerManager playerManager;
    public GameObject normalCamera;
    [Header("Ground & Air Detectioin Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 1.5f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1.7f;
    [SerializeField]
    float groundDirectionRayDistance = 0.2f;
    LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Movement Stats")]
    [SerializeField]
    public float movementSpeed = 5;
    [SerializeField]
    float sprintSpeed = 6.5f;
    [SerializeField]
    float rotationSpeed = 20;
    [SerializeField]
    float fallingSpeed = 45;
    [SerializeField]
    // float walkingSpeed = 5;
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        rigidbody = GetComponent<Rigidbody>();
        playerManager = GetComponentInParent<PlayerManager>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        if (CameraTarget != null) {
            cameraObject = CameraTarget.transform;
        } else {
            cameraObject = Camera.main.transform;
        }
        myTransform = transform;
        animatorHandler.Initialize();
        playerManager.isGrounded = true;
        ignoreForGroundCheck = 1 << 9 | 1 << 10;
    }

    #region Movement
    Vector3 normalVector;
    Vector3 targetPosition;
    // Update is called once per frame
    private void HandleRotation(float delta) {

        if (playerStats.dead) return;
        Vector3 targetDir = Vector3.zero;
        float moveOverride = inputHandler.moveAmount;
        targetDir = cameraObject.forward * inputHandler.vertical;
        targetDir += cameraObject.right * inputHandler.horizontal;

        targetDir.Normalize();
        targetDir.y = 0;
        if (targetDir == Vector3.zero) {
            targetDir = myTransform.forward;
        }
        float rs = rotationSpeed;
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
        myTransform.rotation = targetRotation;
    }

    public void HandleMovement(float delta) {
        if (inputHandler.rollFlag || playerStats.dead) return;
        if (playerManager.isInteracting) {
            // decreaseVelocity(0.2f);
            return;
        }

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = movementSpeed;
        if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5 && animatorHandler.anim.GetBool("canSprint")) {
            speed = sprintSpeed;
            playerManager.isSprinting = true;
            moveDirection *= speed;
        } else {
            if (inputHandler.moveAmount < 0.5) {
                moveDirection *= movementSpeed;
                playerManager.isSprinting = false;
            } else {
                moveDirection *= speed;
                playerManager.isSprinting = false;
            }
        }

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        // if (rigidbody.velocity.magnitude <= projectedVelocity.magnitude) {
        //     rigidbody.AddForce(projectedVelocity, ForceMode.VelocityChange);
        //     Vector3 a = Vector3.ClampMagnitude(projectedVelocity, speed - rigidbody.velocity.magnitude);
        //     rigidbody.AddForce(a, ForceMode.VelocityChange);
        // }
        rigidbody.velocity = projectedVelocity;

        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
        if (animatorHandler.canRotate) {
            HandleRotation(delta);
        }
    }

    public void AddA(Vector3 v, float a) {
        // Debug.Log("v.magnitude " + v.magnitude);
        // clearVelocity();
        Vector3 b = v.normalized * a;
        // Debug.Log("b.magnitude " + b.magnitude);
        rigidbody.AddForce(b, ForceMode.VelocityChange);
        // rigidbody.velocity = b;
        // Debug.Log("Add force");
    }

    public void HandleRollingAndSprinting(float delta) {
        if (animatorHandler.anim.GetBool("isInteracting") && !animatorHandler.anim.GetBool("canDoCombo")) return;
        if (inputHandler.rollFlag) {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            animatorHandler.anim.ResetTrigger("Doge");
            if (inputHandler.moveAmount > 0) {
                float force = 10.0f;
                if (animatorHandler.anim.GetBool("isInteracting") && !animatorHandler.anim.GetCurrentAnimatorStateInfo(1).IsName("Armature|Attack2")) {
                    float angle = SetDogeAngle(moveDirection);
                    if (angle >= -45f && angle <= 45f) {
                        Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                        myTransform.rotation = rollRotation;
                    } else if (angle < 0) {
                        moveDirection = new Vector3(-playerManager.InteractingDirR.x, 0, -playerManager.InteractingDirR.z);
                        force = 7f;
                    } else {
                        moveDirection = playerManager.InteractingDirR;
                        force = 7f;
                    }
                } else {
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                moveDirection.y = 0;
                clearVelocity();
                AddA(moveDirection, force);
                // Debug.Log(rigidbody.velocity);
                animatorHandler.anim.SetTrigger("Doge");
            }
        }
    }

    private float SetDogeAngle(Vector3 rollDirection) {
        // Debug.Log("facing " + playerManager.InteractingDir);
        // Debug.Log("rollDir " + rollDirection);
        // Debug.Log("angle " + Vector3.SignedAngle(playerManager.InteractingDir, rollDirection, Vector3.up));
        float angle = Vector3.SignedAngle(playerManager.InteractingDir, rollDirection, Vector3.up);
        animatorHandler.anim.SetFloat("DogeAngle", angle);
        return angle;
    }

    public void HandleFalling(float delta, Vector3 moveDirection) {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if(Physics.Raycast(origin, myTransform.forward, out hit, 0.4f)) {
            moveDirection = Vector3.zero;
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;

        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.0f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck)) {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPosition.y = tp.y;
            // decreaseVelocity(0.5f);

            if (playerManager.isInAir) {
                if (inAirTimer > 0.5f) {
                    Debug.Log("You were in the air for " + inAirTimer);
                    // animatorHandler.PlayerTargetAnimation("Land", true);
                    inAirTimer = 0;
                }
                else {
                    // animatorHandler.PlayerTargetAnimation("Locomotion", false);
                    inAirTimer = 0;
                }
                playerManager.isInAir = false;
                clearVelocity();
                myTransform.position = new Vector3(myTransform.position.x, 0, myTransform.position.z);
            }
        } else {
            rigidbody.AddForce(-Vector3.up * fallingSpeed);
            if (playerManager.isGrounded) {
                playerManager.isGrounded = false;
            }
            if (playerManager.isInAir == false) {
                if (playerManager.isInteracting == false) {
                    // animatorHandler.PlayerTargetAnimation("Falling", true);
                }

                // Vector3 vel = rigidbody.velocity;
                // vel.Normalize();
                // rigidbody.velocity = vel * (movementSpeed / 2);
                playerManager.isInAir = true;
                // Debug.Log("InAir true");
            }
            // if (playerManager.isGrounded) {
            //     if (playerManager.isInteracting || inputHandler.moveAmount > 0) {
            //         Debug.Log("lerp");
            //         myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
            //     }
            // }
        }
    }

    public void HandleAirDrag() {
        float drag = -0.1f;
        Vector3 force = drag * rigidbody.velocity.normalized * rigidbody.velocity.sqrMagnitude;
        rigidbody.AddForce(force);
    }

    #endregion

    public void clearVelocity() {
        rigidbody.velocity = new Vector3(0, 0, 0);
        // Debug.Log("clear");
    }

    public void decreaseVelocity(float v) {
        if (rigidbody.velocity.magnitude > 0) {
            Vector3 vv = Vector3.ClampMagnitude(rigidbody.velocity, Mathf.Max(0, rigidbody.velocity.magnitude - v));
            rigidbody.velocity = vv;
        }
    }
}

}