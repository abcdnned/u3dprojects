using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanIKController : MonoBehaviour
{
  public LegControllerType2 frontLeftLegStepper;
  public LegControllerType2 frontRightLegStepper;

  public HandController leftHand;

  public HandController rightHand;

  private Vector2 _movement;
  internal bool walking;

  public string status;
  public const string STATUS_MOVING = "STATUS_MOVING";
  public const string STATUS_BATTLE_IDLE = "STATUS_BATTLE_IDLE";

  private ActionStateMachine currentStatus;

  [SerializeField] WalkBalance walkBalance;

  // Only allow diagonal leg pairs to step together

  private InputModule inputModule;
    public static string EVENT_STOP_WALKING = "stopWalking";
    public static string EVENT_KEEP_WALKING = "keepWalking";

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

  public Vector3 getMovement() {
    return _movement;
  }

  private int count = 500;
  private bool f = false;
  private void MovementInput(Vector2 movement) {
      _movement = movement;
  }
  private void ButtonR() {
  }
  private void Update() {
      // if (count > 0) {
      //   count--;
      // } else {
      //   count = 500;
      //   f = !f;
      // }
      // if (f) {
      //   _movement = new Vector2(0,1);
      // } else {
      //   _movement = new Vector2(0,-1);
      // }
      // Debug.Log(this.GetType().Name + " count " + count);
    // Run continuously
      // Try moving one diagonal pair of legs
      // do
      // {
      //   frontLeftLegStepper.TryMove();
        // Wait a frame
        
        // Stay in this loop while either leg is moving.
        // If only one leg in the pair is moving, the calls to TryMove() will let
        // the other leg move if it wants to.
      //   if (!frontLeftLegStepper.Moving) break;
      //   yield return null;
      // } while (frontLeftLegStepper.Moving);
      // Do the same thing for the other diagonal pair
      // do
      // {
      //   frontRightLegStepper.TryMove();
      //   if (!frontRightLegStepper.Moving) break;
      //   yield return null;
      // } while (frontRightLegStepper.Moving);
    bool tmp = walking;
    string eva = EVENT_IDLE;
    walking = _movement.y > 0 || Mathf.Abs(_movement.x) > 0 || _movement.y < 0;
    if (tmp && !walking) {
      // frontLeftLegStepper.handleEvent(EVENT_STOP_WALKING);
      // frontRightLegStepper.handleEvent(EVENT_STOP_WALKING);
      eva = EVENT_STOP_WALKING;
    }
    if (walking)
    {
      // frontRightLegStepper.handleEvent(EVENT_KEEP_WALKING);
      // frontLeftLegStepper.TryMove();
      // frontRightLegStepper.TryMove();
      eva = EVENT_KEEP_WALKING;
    } else {
        Vector3 direction = transform.forward;
        float leftDot = Vector3.Dot(frontLeftLegStepper.transform.position, direction);
        float rightDot = Vector3.Dot(frontRightLegStepper.transform.position, direction);
        if (false && !frontLeftLegStepper.Moving && !frontRightLegStepper.Moving
            && (Mathf.Max(leftDot, rightDot) - Mathf.Min(leftDot, rightDot) > 0.2)) {
          if (leftDot < rightDot) {
            frontLeftLegStepper.TryMove();
            frontLeftLegStepper.handleEvent(EVENT_STOP_WALKING);
          } else {
            frontRightLegStepper.TryMove();
            frontRightLegStepper.handleEvent(EVENT_STOP_WALKING);
          }
        }
    }
    Event ikEvent = new Event();
    ikEvent.eventId = eva;
    ActionStateMachine oldState = currentStatus;
    currentStatus = currentStatus.handleEvent(ikEvent);
    if (oldState != currentStatus) {
      Debug.Log(this.GetType().Name + oldState.getName() + " changed to " + currentStatus.getName());
    }
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
