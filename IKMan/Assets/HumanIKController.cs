using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationProperties))]
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

  public PoseManager poseManager;

  private Vector2 _movement;
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

  internal AnimationProperties animationProperties;

  [Header("--- Input ---")]
  internal bool sprintFlag;

  internal bool jumpFlag;



  // Only allow diagonal leg pairs to step together

  private InputModule inputModule;
    public static string EVENT_STOP_WALKING = "stopWalking";
    public static string EVENT_KEEP_WALKING = "keepWalking";
    public static string EVENT_BUTTON_R = "buttonR";
    public static string EVENT_LEFT_CLICK = "leftClick";
    public static string EVENT_RIGHT_CLICK = "rightClick";
    public static string EVENT_HARD_STOP_WALKING = "hardStopWalking";

    public static string EVENT_IDLE = "idle";

    public static int RIGHT_FOOT = 0;
    public static int LEFT_FOOT = 1;

    private void Start() {
    inputModule = GetComponent<InputModule>();
    inputModule.OnMoveDelegates += MovementInput;
    inputModule.OnButtonRDelegates += ButtonR;
    inputModule.OnLeftArmDelegates += LeftClick;
    inputModule.OnRightArmDelegates += RightClick;
    // inputModule.OnLeftShiftDelegates += OnLeftShift;
    inputModule.OnSpaceDelegates += SpaceClick;
    animationProperties = GetComponent<AnimationProperties>();
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
    return _movement;
  }

  // private int count = 500;
  // private bool f = false;
  private void MovementInput(Vector2 movement) {
      _movement = movement;
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
    string eva = EVENT_IDLE;
    walking = _movement.y > 0 || Mathf.Abs(_movement.x) > 0 || _movement.y < 0;
    if (tmp && !walking) {
      eva = EVENT_STOP_WALKING;
    }
    if (walking)
    {
      eva = EVENT_KEEP_WALKING;
    } else {
        Vector3 direction = transform.forward;
        float leftDot = Vector3.Dot(frontLeftLegStepper.transform.position, direction);
        float rightDot = Vector3.Dot(frontRightLegStepper.transform.position, direction);
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
    ActionStateMachine oldState = currentStatus;
    if (inputModule.getInputController().Player.LeftShift.phase == UnityEngine.InputSystem.InputActionPhase.Performed) {
      sprintFlag = true;
    }
    currentStatus = currentStatus.handleEvent(ikEvent);
    // if (oldState != currentStatus) {
    //   Debug.Log(oldState.getName() + "status changed to " + currentStatus.getName());
    // }
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
    jumpFlag = true;
  }
  private void LateUpdate() {
      sprintFlag =false;
      jumpFlag = false;
  }
}
