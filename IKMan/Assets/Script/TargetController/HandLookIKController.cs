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

    internal Vector3 initForward = Vector3.zero;

    public Transform body;

    public float horizonAngel_lv2 = 0f;
    public float realTimeHorizonAngel_lv2 = 0f;

    public float targetVerticalAngel = 0f;
    public float bicep_target_angel = 0f;
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
        // DrawUtils.drawBall(elbowEnd, 5);
        // DrawUtils.drawBall(handEnd, 5);
        initForward = (handEnd - shoulder.position).normalized;
        // Debug.DrawLine(handEnd, shoulder.position, Color.yellow, 20);
        // Debug.DrawLine(elbowEnd, shoulder.position, Color.yellow, 20);
        // float dis = Vector3.Distance(handEnd, shoulder.position);
        // this.target = Utils.copy(target);
        float targetDis = Vector3.Distance(target, shoulder.position);
        // Debug.DrawLine(target, shoulder.position, Color.blue, 8);
        if (targetDis > elbow.distance + hand.distance - 0.002f) {
            targetDis = elbow.distance + hand.distance - 0.002f;
        }
        Debug.Log(" oritinDis " + Vector3.Distance(target, shoulder.position));
        Debug.Log(" targetDis " + targetDis);
        Vector3 finalTarget = shoulder.position + targetDis * (target - shoulder.position).normalized;
        // double lv1Angel = calculateTargetVerticalAngel(elbow.distance,
        //                                                hand.distance,
        //                                                Vector3.Distance(handEnd, shoulder.position));
        Debug.DrawLine(finalTarget, shoulder.position, Color.blue, 8);
        this.target = finalTarget;
        double lv2Angel = calculateAngelC(elbow.distance,
                                                                 hand.distance,
                                                                 targetDis);
        // targetVerticalAngel = (float)(lv2Angel - lv1Angel);
        targetVerticalAngel = 180 - (float)lv2Angel;
        bicep_target_angel = calculateAngelC(elbow.distance, targetDis, hand.distance);
        initNormal = Vector3.Cross(handEnd - shoulder.position, finalTarget - shoulder.position);
        // Debug.DrawLine(shoulder.position + initNormal * 5, shoulder.position, Color.black, 8);
        horizonAngel_lv2 = Vector3.Angle(handEnd - shoulder.position,
                                                finalTarget - shoulder.position);
        Debug.Log(" horizonAngel_lv2 " + horizonAngel_lv2);
        this.duration = duration;
        poc = 0;
    }

    private void Update() {
        poc += Time.deltaTime;
    }
// Vector3 currentOffset = arm.transform.InverseTransformPoint(hand.transform.position);
// Vector3 desiredOffset = arm.transform.InverseTransformPoint(box.transform.position);
// arm.transform.localRotation *= Quaternion.FromToRotation(currentOffset, desiredOffset);

    private float calculateAngelC(float sideA, float sideB, float sideC)
    {
        // Convert the sides to radians
        double angleC = Math.Acos((sideA * sideA + sideB * sideB - sideC * sideC) / (2 * sideA * sideB));

        // Convert the angle to degrees
        double degrees = angleC * 180 / Math.PI;

        return (float)degrees;
    }

    public Vector3 getHandShoulderForward(Vector3 lv1_pos) {
        return (lv1_pos - shoulder.position).normalized;
    }

    public Vector3 getHandShoulderNormal(Vector3 lv1_pos) {
        Vector3 lv1arm = lv1_pos - shoulder.position;
        // Debug.DrawLine(lv1_pos, shoulder.position, Color.red, 0.1f);
        Vector3 t = target - shoulder.position;
        // Debug.DrawLine(target, shoulder.position, Color.black, 0.1f);
        Vector3 normal1 = Vector3.Cross(t, lv1arm);
        Vector3 c1 = elbow.transform.position - shoulder.position; 
        Vector3 c2 = hand.transform.position - shoulder.position; 
        // float angle = Vector3.Angle(c1, c2);
        float angle = bicep_target_angel;
        Debug.Log(" clv1arm " + angle);
        Vector3 lv2armdir = (Quaternion.AngleAxis(angle, normal1) * t).normalized * lv1arm.magnitude;
        // Debug.DrawLine(shoulder.position + lv2armdir, shoulder.position, Color.blue, 0.1f);
        // Debug.DrawLine(target, shoulder.position + lv2armdir, Color.green, 0.1f);
        Debug.Log(" elbow " + lv2armdir.magnitude);
        Debug.Log(" target " + t.magnitude);
        Debug.Log(" hand " + Vector3.Distance(target, shoulder.position + lv2armdir));
        return Vector3.Cross(lv1arm, lv2armdir);
    }

    public Vector3 getArmForward(Vector3 lv1_pos) {
        return (lv1_pos - elbow.transform.position).normalized;
    }

    public Vector3 getArmNormal(Vector3 lv1_pos) {
        Vector3 c = lv1_pos - elbow.transform.position;
        Vector3 t = target - elbow.transform.position;
        // Vector3 arm = (lv1_pos - elbow.transform.position).normalized;
        // Vector3 bicep = (elbow.transform.position - shoulder.position).normalized;
        // return Vector3.Cross(bicep, arm);
        return Vector3.Cross(c, t);
    }

    internal float getRealTimeHorizonAngel(Vector3 lv1_pos, float normalizedTime) {
        Vector3 lv1arm = lv1_pos - shoulder.position;
        Vector3 t = target - shoulder.position;
        Vector3 normal1 = Vector3.Cross(t, lv1arm);
        Vector3 c = hand.transform.position - shoulder.position; 
        float angle = bicep_target_angel;
        Vector3 lv2armdir = Quaternion.AngleAxis(angle, normal1) * t;
        float ta = Vector3.Angle(lv1arm, lv2armdir);
        return Mathf.Lerp(0, ta, normalizedTime / duration);
    }

    internal float getRealTimeVerticalAngel(float normalizedTime, Vector3 lv1_pos)
    {
        // Vector3 arm = (lv1_pos - elbow.transform.position).normalized;
        // Vector3 bicep = (elbow.transform.position - shoulder.position).normalized;
        // Debug.Log(" targetVerticalAngel " + targetVerticalAngel);
        // return Mathf.Lerp(Vector3.Angle(bicep, arm), targetVerticalAngel, normalizedTime / duration);
        Vector3 c = lv1_pos - elbow.transform.position;
        Vector3 t = target - elbow.transform.position;
        return Mathf.Lerp(0, Vector3.Angle(c, t), normalizedTime / duration);
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
