using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanIKController : MonoBehaviour
{
  public LegControllerType2 frontLeftLegStepper;
  public LegControllerType2 frontRightLegStepper;
  private Vector2 _movement;
  internal bool walking;

  // Only allow diagonal leg pairs to step together

  private InputModule inputModule;
    public static string EVENT_STOP_WALKING = "stopWalking";

    private void Start() {
    inputModule = GetComponent<InputModule>();
    inputModule.OnMoveDelegates += MovementInput;
    
  }

  public Vector3 getMovement() {
    return _movement;
  }
  private void MovementInput(Vector2 movement) {
      _movement = movement;
  }
  private void Update() {
    // Run continuously
    bool tmp = walking;
    walking = _movement.y > 0 || Mathf.Abs(_movement.x) > 0 || _movement.y < 0;
    if (tmp && !walking) {
      frontLeftLegStepper.handleEvent(EVENT_STOP_WALKING);
      frontRightLegStepper.handleEvent(EVENT_STOP_WALKING);
    }
    if (walking)
    {
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
      frontLeftLegStepper.TryMove();
      frontRightLegStepper.TryMove();
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
  }
}
