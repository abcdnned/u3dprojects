using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class PlayerLocomotion : MonoBehaviour
{
    public GameObject CameraTarget;
    Transform cameraObject;
    InputHandler inputHandler;
    private AnimatorHandler animatorHandler;
    public Vector3 moveDirection;
    // Start is called before the first frame update
    [HideInInspector]
    public Transform myTransform;

    public new Rigidbody rigidbody;
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

    void Start()
    {
        animatorHandler = GetComponent<AnimatorHandler>();
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        if (CameraTarget != null) {
            cameraObject = CameraTarget.transform;
        } else {
            cameraObject = Camera.main.transform;
        }
        myTransform = transform;
        ignoreForGroundCheck = 1 << 9 | 1 << 10;
    }

    Vector3 normalVector;
    Vector3 targetPosition;
    // Update is called once per frame
    private void HandleRotation(float delta) {

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
        animatorHandler.UpdateWalkSpeed(inputHandler.moveAmount);
    }
    void Update()
    {
        inputHandler.GetInput(Time.deltaTime);
        HandleMovement(Time.deltaTime);
    }
}

}