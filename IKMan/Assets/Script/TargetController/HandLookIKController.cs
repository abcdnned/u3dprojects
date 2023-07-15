using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLookIKController : MonoBehaviour
{
    public Vector3 target = Vector3.zero;
    public Transform shoulder;
    public HandLooker elbow;

    public HandLooker hand;

    public Vector3 initBodyForward;
    public Vector3 initNormal = Vector3.zero;

    public Transform body;

    public float horizonAngel_lv2 = 0f;
    public float realTimeHorizonAngel_lv2 = 0f;

    public float targetVerticalAngel = 0f;
    public float realTimeTargetVerticalAngel = 0f;

    public float duration;

    public float poc;

    public void init(float duration, Vector3 target, Transform body) {
        this.body = body;
        initBodyForward = Utils.forward(body);
        Vector3 elbowEnd = elbow.SphereLv1PositionCalculator(shoulder.position);
        Transform helper = PrefabCreator.SpawnDebugger(elbowEnd, "ElbowEndHelper", 2, 1, null).transform;
        Vector3 dir = elbowEnd - shoulder.position;
        helper.rotation = Quaternion.LookRotation(dir, elbow.direction.forward);
        Vector3 handEnd = hand.SphereLv1PositionCalculator(elbowEnd, false, helper);
        DrawUtils.drawBall(elbowEnd, 5);
        DrawUtils.drawBall(handEnd, 5);
        Debug.DrawLine(handEnd, shoulder.position, Color.yellow, 20);
        // float dis = Vector3.Distance(handEnd, shoulder.position);
        this.target = Utils.copy(target);
        float targetDis = Vector3.Distance(target, shoulder.position);
        Debug.DrawLine(target, shoulder.position, Color.blue, 8);
        if (targetDis > elbow.distance + hand.distance - 0.002f) {
            targetDis = elbow.distance + hand.distance - 0.002f;
        }
        Debug.Log(" oritinDis " + Vector3.Distance(target, shoulder.position));
        Debug.Log(" targetDis " + targetDis);
        Vector3 finalTarget = shoulder.position + targetDis * (target - shoulder.position).normalized;
        // double lv1Angel = calculateTargetVerticalAngel(elbow.distance,
        //                                                hand.distance,
        //                                                Vector3.Distance(handEnd, shoulder.position));
        double lv2Angel = calculateTargetVerticalAngel(elbow.distance,
                                                                 hand.distance,
                                                                 targetDis);
        // targetVerticalAngel = (float)(lv2Angel - lv1Angel);
        targetVerticalAngel = 180 - (float)lv2Angel;
        initNormal = Vector3.Cross(handEnd - shoulder.position, finalTarget - shoulder.position);
        horizonAngel_lv2 = Vector3.Angle(handEnd - shoulder.position,
                                                finalTarget - shoulder.position);
        this.duration = duration;
        poc = 0;
    }

    private void Update() {
        poc += Time.deltaTime;
    }

    private float calculateTargetVerticalAngel(float sideA, float sideB, float sideC)
    {
        // Convert the sides to radians
        double angleA = Math.Acos((sideA * sideA + sideB * sideB - sideC * sideC) / (2 * sideA * sideB));

        // Convert the angle to degrees
        double degrees = angleA * 180 / Math.PI;

        return (float)degrees;
    }

    public Vector3 getHandShoulderForward(Vector3 lv1_pos) {
        return (lv1_pos - shoulder.position).normalized;
    }

    public Vector3 getHandShoulderNormal() {
        Quaternion rotation = Quaternion.FromToRotation(initBodyForward, Utils.forward(body));
        return rotation * initNormal;
    }

    public Vector3 getArmForward() {
        return (elbow.transform.position - shoulder.position).normalized;
    }

    public Vector3 getArmNormal(Vector3 lv1_pos) {
        Vector3 arm = (lv1_pos - elbow.transform.position).normalized;
        Vector3 bicep = (elbow.transform.position - shoulder.position).normalized;
        return Vector3.Cross(bicep, arm);
    }

    internal float getRealTimeHorizonAngel(float normalizedTime) {
        return Mathf.Lerp(0, horizonAngel_lv2, normalizedTime / duration);
    }

    internal float getRealTimeVerticalAngel(float normalizedTime, Vector3 lv1_pos)
    {
        Vector3 arm = (lv1_pos - elbow.transform.position).normalized;
        Vector3 bicep = (elbow.transform.position - shoulder.position).normalized;
        Debug.Log(" targetVerticalAngel " + targetVerticalAngel);
        return Mathf.Lerp(Vector3.Angle(bicep, arm), targetVerticalAngel, normalizedTime / duration);
    }

    public int getIKSequence(HandLooker check) {
        if (duration <= 0 || poc > duration) {
            return 0;
        }
        if (check == elbow) {
            // Debug.Log(" ikseq 1 ");
            return 1;
        } else if (check == hand) {
            // Debug.Log(" ikseq 2 ");
            return 2;
        }
        return 0;
    } 
}
