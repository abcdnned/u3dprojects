using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationProperties))]
[RequireComponent(typeof(InputArgument))]
public class HumanIKController : MonoBehaviour
{
  [Header("--- BODY PART ---")]
  public LegControllerType2 frontLeftLegStepper;
  public LegControllerType2 frontRightLegStepper;

  public Rigidbody body;
  public HandController leftHand;

  public HandController rightHand;
  [SerializeField] public WalkBalance walkBalance;

  public HeadController headController;

  public WalkPointer walkPointer;

  public GameObject spin1;

  public Transform spin2;
  public Transform spin3;
  public Transform leftShoulder;
  public Transform rightShoulder;
  public Transform neck;

  public PoseManager poseManager;

  public GameObject rootGameObject;

  // private Vector2 _movement;
  internal bool walking;


  public const int ANCHOR_LEFT_LEG = 0;
  public const int ANCHOR_RIGHT_LEG = 1;
  public const int ANCHOR_LEFT_HAND = 2;
  public const int ANCHOR_RIGHT_HAND = 3;

  [Header("--- BATTLE ---")]
  public float bi_footAngel = 15f;
  public float bi_footDistance = 0.3f;
  // public float bi_backFootAngelOffset = 95f;
  // public float bi_fontLegAngelOffset = 10f;
  // public float bi_footTurnDuration = 0.5f;
  public float swingRadius = 0.6f;
  public Vector3[] battleIdleAnchorPoints = new Vector3[10];

  public float[] battleIdleAngelOffset = new float[30];

  internal CharacterJoint poleJoint;

  [Header("--- Weapon ---")]
  public Transform mainHandle;
  public Transform consonantHandle;
  public GameObject weapon;

  [Header("--- Attachment ---")]
  public GameObject attchment_rightHand;

  public GameObject weaponReady;

  [Header("--- IDLE ---")]
  public Vector3[] idleAnchorPoints = new Vector3[10];

  internal ActionStateMachine currentStatus;

  private ReadTrigger TriggerR = new ReadTrigger(false);
  private ReadTrigger TriggerLeftClick = new ReadTrigger(false);
  private ReadTrigger TriggerRightClick = new ReadTrigger(false);

  [Header("--- Character Properties ---")]
  public float swingStrength = 200;

  public float runMaxSpeed = 5;

  internal AnimationProperties ap;

  [Header("--- Input ---")]
  internal bool sprintFlag;

  // internal bool jumpFlag;

  // Only allow diagonal leg pairs to step together
  private InputModule inputModule;
  public static string EVENT_STOP_WALKING = "stopWalking";
  public static string EVENT_KEEP_WALKING = "keepWalking";
  public static string EVENT_BUTTON_R = "buttonR";
  public static string EVENT_LEFT_CLICK = "leftClick";
  public static string EVENT_RIGHT_CLICK = "rightClick";
  public static string EVENT_HARD_STOP_WALKING = "hardStopWalking";
  public static string EVENT_IDLE = "idle";
  public static string EVENT_JUMP = "jump";

  public static int RIGHT_FOOT = 0;
  public static int LEFT_FOOT = 1;

  internal InputArgument inputArgument;
  internal Vector3 gravityUp = Vector3.up;

  internal Vector3 relativeMovment = Vector3.zero;






  private void Start() {
    inputModule = GetComponent<InputModule>();
    inputArgument = GetComponent<InputArgument>();
    inputModule.OnMoveDelegates += MovementInput;
    inputModule.OnButtonRDelegates += ButtonR;
    inputModule.OnLeftArmDelegates += LeftClick;
    inputModule.OnRightArmDelegates += RightClick;
    // inputModule.OnLeftShiftDelegates += OnLeftShift;
    inputModule.OnSpaceDelegates += SpaceClick;
    ap = GetComponent<AnimationProperties>();
    initStatus();
  }

  public void initStatus() {
    currentStatus = new IdleStatus(this);
  }

  public void updateAnchorPoints() {
    idleAnchorPoints[ANCHOR_LEFT_LEG] = frontLeftLegStepper.homeTransform.position;
    idleAnchorPoints[ANCHOR_RIGHT_LEG] = frontRightLegStepper.homeTransform.position;
    idleAnchorPoints[ANCHOR_LEFT_HAND] = leftHand.handHome.position;
    idleAnchorPoints[ANCHOR_RIGHT_HAND] = rightHand.handHome.position;
    calculateBattleIdlePoints();
  }

  private void calculateBattleIdlePoints()
  {
    Vector3 forward = Utils.forwardFlat(body.transform);

    Quaternion leftRotation = Quaternion.AngleAxis(-bi_footAngel, Vector3.up);
    Quaternion rightRotation = Quaternion.AngleAxis(180f - bi_footAngel, Vector3.up);

    Vector3 leftOffset = leftRotation * forward;
    Vector3 rightOffset = rightRotation * forward;

    Vector3 position = body.transform.position;
    Vector3 pointA = position + leftOffset * bi_footDistance;
    Vector3 pointB = position + rightOffset * bi_footDistance;
    battleIdleAnchorPoints[ANCHOR_LEFT_LEG] = pointA;
    battleIdleAnchorPoints[ANCHOR_RIGHT_LEG] = pointB;
  }

    public Vector3 getMovement() {
    return inputArgument.movement;
  }

  // private int count = 500;
  // private bool f = false;
  private void MovementInput(Vector2 movement) {
      inputArgument.movement = movement;
  }
  private void ButtonR() {
    Debug.Log(this.GetType().Name + " buttonR ");
    TriggerR.set();
  }
  private void Update() {
    // Update anchor points for animation to use
    // updateAnchorPoints();
    // Movement input
    bool tmp = walking;
    string eva = null;
    // Debug.Log(" movement " + inputArgument.movement);
    walking = inputArgument.movement.magnitude > 0;
    if (inputArgument.jumpFlag) {
      eva = EVENT_JUMP;
    }
    else if (tmp && !walking) {
      eva = EVENT_STOP_WALKING;
    }
    else if (walking)
    {
      eva = EVENT_KEEP_WALKING;
    // } else {
    //     Vector3 direction = transform.forward;
    //     float leftDot = Vector3.Dot(frontLeftLegStepper.transform.position, direction);
    //     float rightDot = Vector3.Dot(frontRightLegStepper.transform.position, direction);
    }
    // Button Group A input
    string bga = null;
    if (TriggerR.read()) {
      bga = EVENT_BUTTON_R;
      // Debug.Log(this.GetType().Name + " bga set ");
    } else if (TriggerLeftClick.read()) {
      bga = EVENT_LEFT_CLICK;
    } else if (TriggerRightClick.read()) {
      bga = EVENT_RIGHT_CLICK;
    }
    Event ikEvent = new Event();
    ikEvent.bgA = bga;
    ikEvent.eventId = eva;
    ActionStateMachine oldStatus = currentStatus;
    if (inputModule.getInputController().Player.LeftShift.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
      sprintFlag = true;
    }

    // if (eva != null) {
    currentStatus = currentStatus.handleEvent(ikEvent);
    if (oldStatus.GetType() != currentStatus.GetType()) {
      oldStatus.pose?.exit();
    }
    // }

    if (currentStatus.moveController != null) {
      currentStatus.moveController.deltaMove();
    }
    walkPointer.update();

    if (currentStatus.pose != null) {
      currentStatus.pose.update();
    }

    leftHand.move?.move(Time.deltaTime);
    leftHand.updateHandLocalRotation();
    frontLeftLegStepper.move?.move(Time.deltaTime);
    rightHand.move?.move(Time.deltaTime);
    rightHand.updateHandLocalRotation();
    frontRightLegStepper.move?.move(Time.deltaTime);

    leftHand.handLookIKController.update();
    frontLeftLegStepper.handLookIKController.update();
    rightHand.handLookIKController.update();
    frontRightLegStepper.handLookIKController.update();

    // Update elbow and hand rotation
    leftHand.advanceIKController.elbow.update();
    leftHand.advanceIKController.hand.update();
    rightHand.advanceIKController.elbow.update();
    rightHand.advanceIKController.hand.update();
    frontLeftLegStepper.advanceIKController.elbow.update();
    frontLeftLegStepper.advanceIKController.hand.update();
    frontRightLegStepper.advanceIKController.elbow.update();
    frontRightLegStepper.advanceIKController.hand.update();

    walkBalance.update();

    frontLeftLegStepper.advanceIKController.update();
    frontRightLegStepper.advanceIKController.update();
    leftHand.advanceIKController.update();
    rightHand.advanceIKController.update();


  }

  public void postUpdateTowHandPosition() {
    leftHand.postUpdateTowHandPosition();
    rightHand.postUpdateTowHandPosition();
  }

  public void logHomeOffset() {
      leftHand.logHomeOffset();
      rightHand.logHomeOffset();
  }

  public void TwoFootAssign(float duration, int mainfoot,
                            float footDis, int clock,
                            float leftAngel, float rightAngel,
                            Transform pointer = null) {
      LegControllerType2 leg = mainfoot == RIGHT_FOOT ? frontRightLegStepper : frontLeftLegStepper;
      Vector3 rootPoint = mainfoot == RIGHT_FOOT ? frontLeftLegStepper.transform.position : frontRightLegStepper.transform.position;
      rootPoint = Utils.snapTo(rootPoint);
      float angle = AnchorManager.clockAngel[clock];
      // Debug.Log(" angel " + angle);
      Vector3 forward = Utils.forward(walkPointer.transform);
      // Debug.DrawLine(rootPoint, rootPoint + forward * 1, Color.yellow, 10);
      forward = Quaternion.AngleAxis(angle, Vector3.up) * forward;
      // Debug.DrawLine(rootPoint, rootPoint + forward * 1, Color.yellow, 10);
      Vector3 targetPoint = rootPoint + forward * footDis;
      if (mainfoot == RIGHT_FOOT) {
        // DrawUtils.drawBall(frontLeftLegStepper.transform.position, 5);
        frontRightLegStepper.TryPutLeg(targetPoint, rightAngel, duration, walkPointer.transform);
        frontLeftLegStepper.TryRotateLeg(leftAngel, duration);
      } else if (mainfoot == LEFT_FOOT) {
        frontLeftLegStepper.TryPutLeg(targetPoint, leftAngel, duration, walkPointer.transform);
        frontRightLegStepper.TryRotateLeg(rightAngel, duration);
      }

  }

  private void LeftClick(float value) {
    TriggerLeftClick.set();
  }

  private void RightClick(float value) {
    TriggerRightClick.set();
  }
  // private void OnLeftShift() {
  //   sprintFlag = true;
  // }
  private void SpaceClick() {
    inputArgument.jumpFlag = true;
  }
  private void LateUpdate() {
      sprintFlag =false;
      inputArgument.reset();
      relativeMovment = Vector3.zero;
  }
}
