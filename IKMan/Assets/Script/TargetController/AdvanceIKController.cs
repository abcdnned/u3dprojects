using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceIKController : MonoBehaviour
{
    public Transform shoulder;
    public HandDelayLooker elbow;
    public HandDelayLooker hand;

    public Transform fk;

    public string state;
    
    public const string FK = "FK";

    public const string IK = "IK";

    public const string HALF_IK = "HALF_IK";

    void Update()
    {
        if (state == HALF_IK) {
            updateArmDirection();
        }
    }
    
    public void changeState(string state) {
        this.state = state;
        if (state == HALF_IK) {
            elbow.enabled = true;
            hand.enabled = false;
        } else {
            elbow.enabled = true;
            hand.enabled = true;
        }
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
}
