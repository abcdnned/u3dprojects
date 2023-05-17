using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanIKController : MonoBehaviour
{
  [Header("--- BODY PART ---")]
  public LegControllerType2 frontLeftLegStepper;
  public LegControllerType2 frontRightLegStepper;

  public Rigidbody body;
  public HandController leftHand;

  public HandController rightHand;
  [SerializeField] public WalkBalance walkBalance;
  public Transform weaponHandle;
  public Transform weaponReadyHandle;

  public HeadController headController;

  public WalkPointer walkPointer;

  public PoseManager poseManager;

  private Vector2 _movement;
  internal bool walking;


  public const int ANCHOR_LEFT_LEG = 0;
  public const int ANCHOR_RIGHT_LEG = 1;
  public const int ANCHOR_LEFT_HAND = 2;
  public const int ANCHOR_RIGHT_HAND = 3;

  [Header("--- BATTLE_IDLE ---")]
  public float bi_footAngel = 15f;
  public float bi_footDistance = 0.3f;
  public float bi_backFootAngelOffset = 95f;
  public float bi_fontLegAngelOffset = 10f;
  // public float bi_footTurnDuration = 0.5f;
  public Vector3[] battleIdleAnchorPoints = new Vector3[10];

  public float[] battleIdleAngelOffset = new float[30];

  [Header("--- IDLE ---")]
  public Vector3[] idleAnchorPoints = new Vector3[10];

  internal ActionStateMachine currentStatus;

  private ReadTrigger TriggerR = new ReadTrigger(false);


  // Only allow diagonal leg pairs to step together

  private InputModule inputModule;
    public static string EVENT_STOP_WALKING = "stopWalking";
    public static string EVENT_STOP_WALKING_NOW = "stopWalkingNow";
    public static string EVENT_KEEP_WALKING = "keepWalking";
    public static string EVENT_BUTTON_R = "buttonR";

    public static string EVENT_IDLE = "idle";

    private void Start() {
    inputModule = GetComponent<InputModule>();
    inputModule.OnMoveDelegates += MovementInput;
    inputModule.OnButtonRDelegates += ButtonR;
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
    }
    Event ikEvent = new Event();
    ikEvent.bgA = bga;
    ikEvent.eventId = eva;
    ActionStateMachine oldState = currentStatus;
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
}
