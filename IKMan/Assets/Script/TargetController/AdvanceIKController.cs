using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceIKController : MonoBehaviour
{
    public Transform shoulder;
    public HandDelayLooker elbow;
    public HandDelayLooker hand;
    public HandLooker hint;

    public HandController handController;

    public Transform fk;

    public string state;
    
    public const string FK = "FK";

    public const string IK = "IK";

    public const string HALF_IK = "HALF_IK";

    // FK for hint, IK for position
    public const string FIK = "FK_IK";
    public const string FIK_ADVANCE = "FK_IK_ADVANCE";

    // public float FK_SPEED = 0.2f;
    public float FK_SPEED = 200f;

    [SerializeField]
    float HintDis = 0.3f;
    

    private void LateUpdate()
    {
        if (state == HALF_IK) {
            updateArmDirection();
            if (handController == null) {
                updateHintByFK();
            } else {
                handController.updateHintByFK();
            }
        } else if (state == IK) {
        } else if (state == FK) {
            if (handController == null) {
                updateHintByFK();
            } else {
                handController.updateHintByFK();
            }
            trackIKtoFK();
        } else if (state == FIK) {
            if (handController == null) {
                updateHintByFK();
            } else {
                handController.updateHintByFK();
            }
        }
    }
    
    public void changeState(string state) {
        this.state = state;
        if (state == HALF_IK) {
            // elbow.enabled = true;
            // hand.enabled = false;
            hint.enable = false;
            hint.enable_lv2 = false;
        } else if (state == IK) {
            // elbow.enabled = false;
            // hand.enabled = false;
            hint.enable = true;
            hint.enable_lv2 = true;
        } else if (state == FK) {
            // elbow.enabled = true;
            // hand.enabled = true;
            hint.enable = false;
            hint.enable_lv2 = false;
        } else if (state == FIK) {
            // elbow.enabled = true;
            // hand.enabled = true;
            hint.enable = false;
            hint.enable_lv2 = false;
        }
    }

    private void trackIKtoFK() {
        Vector3 target = hand.transform.position;
        Utils.deltaMove(fk, target);
        // DrawUtils.drawBall(target, Time.deltaTime);
        // fk.position = target;
        // Debug.Log(" trackIKtoFK ");
    }

    private void updateArmDirection() {
        Vector3 forward = getArmDirection();
        Vector3 target = elbow.transform.position + forward * hand.distance;
        Utils.deltaMove(hand.transform, target);
    }

    public Vector3 getArmDirection() {
        Vector3 r = fk.position - elbow.transform.position;
        return r.normalized;
    }

    public void updateHintByFK() {
        Vector3 elbow = this.elbow.transform.position;
        Vector3 hand = this.hand.transform.position;
        Vector3 shoulder = this.shoulder.position;
        Vector3 v1 = elbow - shoulder;
        Vector3 v2 = elbow - hand;
        Vector3 normal = Vector3.Cross(v2, v1);
        float angel = Vector3.Angle(v1, v2);
        Quaternion rotation = Quaternion.AngleAxis(angel / 2, normal);
        Vector3 forward = rotation * v2;
        Debug.DrawLine(shoulder, elbow, Color.red);
        Debug.DrawLine(hand, elbow, Color.blue);
        // Debug.DrawRay(elbow, v1, Color.red);
        // Debug.DrawRay(elbow, v2, Color.blue);
        Debug.DrawRay(elbow, forward, Color.green);
        Vector3 offset = HintDis * forward.normalized;
        hint.transform.position = elbow + offset;
    }
}
