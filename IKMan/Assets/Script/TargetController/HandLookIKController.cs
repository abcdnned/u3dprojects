using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLookIKController : MonoBehaviour
{
    public Transform target;
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
    private bool continueTrackAfterFinish = false;
    private const float BEND = 0.002f;

    private ReadTrigger stopSignal;




    public void init(float duration, Vector3 target, Transform body, Transform target_transform = null, bool continueTrack = false, ReadTrigger stopSignal = null) {
        this.body = body;
        continueTrackAfterFinish = continueTrack;
        initBodyForward = Utils.forward(body);
        // Vector3 elbowEnd = elbow.SphereLv1PositionCalculator(shoulder.position);
        // Transform helper = PrefabCreator.SpawnDebugger(elbowEnd, "ElbowEndHelper", Time.deltaTime, 1, null).transform;
        // Vector3 dir = elbowEnd - shoulder.position;
        // helper.rotation = Quaternion.LookRotation(dir, elbow.direction.forward);
        // Vector3 handEnd = hand.SphereLv1PositionCalculator(elbowEnd, false, helper);
        Vector3 handEnd = getHandEnd(false);
        // DrawUtils.drawBall(elbowEnd, 5);
        // DrawUtils.drawBall(handEnd, 5);
        initForward = (handEnd - shoulder.position).normalized;
        // Debug.DrawLine(handEnd, shoulder.position, Color.yellow, 20);
        // Debug.DrawLine(elbowEnd, shoulder.position, Color.yellow, 20);
        // float dis = Vector3.Distance(handEnd, shoulder.position);
        // this.target = Utils.copy(target);
        float targetDis = Vector3.Distance(target, shoulder.position);
        if (targetDis > elbow.distance + hand.distance - BEND) {
            targetDis = elbow.distance + hand.distance - BEND;
        }
        Vector3 finalTarget = shoulder.position + targetDis * (target - shoulder.position).normalized;
        if (target_transform != null) {
            this.target = target_transform;
        } else {
            this.target = PrefabCreator.SpawnDebugger(finalTarget, PrefabCreator.POSITION_HELPER,
                                                    duration * PrefabCreator.DEFAULT_LIVE, 1, body).transform;
        }
        // double lv2Angel = calculateAngelC(elbow.distance,
        //                                                          hand.distance,
        //                                                          targetDis);
        // targetVerticalAngel = (float)(lv2Angel - lv1Angel);
        // targetVerticalAngel = 180 - (float)lv2Angel;
        bicep_target_angel = calculateAngelC(elbow.distance, targetDis, hand.distance);
        initNormal = Vector3.Cross(handEnd - shoulder.position, finalTarget - shoulder.position);
        // Debug.DrawLine(shoulder.position + initNormal * 5, shoulder.position, Color.black, 8);
        horizonAngel_lv2 = Vector3.Angle(handEnd - shoulder.position,
                                                finalTarget - shoulder.position);
        // Debug.Log(" horizonAngel_lv2 " + horizonAngel_lv2);
        this.duration = duration;
        poc = 0;
        this.stopSignal = stopSignal;
    }

    // private void Update() {
    //     poc += Time.deltaTime;
    // }

    private Vector3 NormalizeTargetPosition(Vector3 target) {
        float targetDis = Vector3.Distance(target, shoulder.position);
        if (targetDis > elbow.distance + hand.distance - BEND) {
            targetDis = elbow.distance + hand.distance - BEND;
        }
        Vector3 finalTarget = shoulder.position + targetDis * (target - shoulder.position).normalized;
        return finalTarget;
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
        Vector3 t = NormalizeTargetPosition(target.position) - shoulder.position;
        // Debug.DrawLine(NormalizeTargetPosition(target.position), shoulder.position, Color.black, Time.deltaTime);
        Vector3 normal1 = Vector3.Cross(t, lv1arm);
        // return normal1;
        // Vector3 c1 = elbow.transform.position - shoulder.position; 
        // Vector3 c2 = hand.transform.position - shoulder.position; 
        // float angle = Vector3.Angle(c1, c2);
        if (target != null) {
            updateBicepTargetAngel();
        }
        float angle = bicep_target_angel;
        // Debug.Log(" clv1arm " + angle);
        Vector3 lv2armdir = (Quaternion.AngleAxis(angle, normal1) * t).normalized * lv1arm.magnitude;
        // Debug.DrawLine(shoulder.position + lv2armdir, shoulder.position, Color.blue, 0.1f);
        // Debug.DrawLine(target, shoulder.position + lv2armdir, Color.green, 0.1f);
        // Debug.Log(" elbow " + lv2armdir.magnitude);
        // Debug.Log(" target " + t.magnitude);
        // Debug.Log(" hand " + Vector3.Distance(target.position, shoulder.position + lv2armdir));
        return Vector3.Cross(lv1arm, lv2armdir);
    }


    public Vector3 getArmForward(Vector3 lv1_pos) {
        return (lv1_pos - elbow.transform.position).normalized;
    }

    public Vector3 getArmNormal(Vector3 lv1_pos) {
        Vector3 c = lv1_pos - elbow.transform.position;
        Vector3 t = NormalizeTargetPosition(target.position) - elbow.transform.position;
        // Vector3 arm = (lv1_pos - elbow.transform.position).normalized;
        // Vector3 bicep = (elbow.transform.position - shoulder.position).normalized;
        // return Vector3.Cross(bicep, arm);
        return Vector3.Cross(c, t);
    }

    internal float getRealTimeHorizonAngel(Vector3 lv1_pos) {
        Vector3 lv1arm = lv1_pos - shoulder.position;
        Vector3 t = NormalizeTargetPosition(target.position) - shoulder.position;
        Vector3 normal1 = Vector3.Cross(t, lv1arm);
        Vector3 c = hand.transform.position - shoulder.position; 
        if (target != null) {
            updateBicepTargetAngel();
        }
        float angle = bicep_target_angel;
        Vector3 lv2armdir = Quaternion.AngleAxis(angle, normal1) * t;
        float ta = Vector3.Angle(lv1arm, lv2armdir);
        return Mathf.Lerp(0, ta, poc / duration);
    }

    internal float getRealTimeVerticalAngel(Vector3 lv1_pos)
    {
        // Vector3 arm = (lv1_pos - elbow.transform.position).normalized;
        // Vector3 bicep = (elbow.transform.position - shoulder.position).normalized;
        // Debug.Log(" targetVerticalAngel " + targetVerticalAngel);
        // return Mathf.Lerp(Vector3.Angle(bicep, arm), targetVerticalAngel, normalizedTime / duration);
        Vector3 c = lv1_pos - elbow.transform.position;
        Vector3 t = NormalizeTargetPosition(target.position) - elbow.transform.position;
        return Mathf.Lerp(0, Vector3.Angle(c, t), poc / duration);
    }

    public int getIKSequence(HandLooker check) {
        if (duration <= 0 || poc > duration || target == null) {
            if (poc > duration && continueTrackAfterFinish && target != null) {
                if (stopSignal != null && stopSignal.peek()) {
                    return 0;
                }
                Debug.Log(" continue track ");
                // continue track
            } else {
                return 0;
            }
            // Debug.Log(" ikseq 0 ");
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

    internal void transferCurPosToLv1() {
        tCpTL(elbow, elbow.transform.position - shoulder.position, shoulder.position);
        tCpTL(hand, hand.transform.position - elbow.transform.position, elbow.transform.position);
    }

    private void tCpTL(HandLooker hdl, Vector3 dir, Vector3 parent) {
        Vector3 normal = Vector3.Cross(hdl.direction.forward, hdl.direction.right);
        Vector3 hdl_p = Vector3.ProjectOnPlane(dir, normal);
        float h = Vector3.Angle(hdl.direction.forward, hdl_p);
        Debug.DrawLine(parent, parent + hdl_p, Color.black, 0.1f);
        Vector3 normal_v = Vector3.Cross(hdl_p, hdl.direction.up);
        Vector3 hdl_v = Vector3.ProjectOnPlane(dir, normal_v);
        float v = Vector3.Angle(hdl_p, hdl_v);
        hdl.horizonAngel = h * (Vector3.Dot(normal, Vector3.Cross(hdl.direction.forward, hdl_p)) > 0 ? 1 : -1);
        hdl.verticalAngel = v * (Vector3.Dot(normal_v, Vector3.Cross(hdl_p, hdl_v)) > 0 ? 1 : -1);
        Debug.Log(" hor " + hdl.horizonAngel);
        Debug.Log(" hov " + hdl.verticalAngel);
    }

    private void updateBicepTargetAngel()
    {
        // Debug.Log(" update bicep targetAngel ");
        float targetDis = Vector3.Distance(target.position, shoulder.position);
        if (targetDis > elbow.distance + hand.distance - BEND) {
            targetDis = elbow.distance + hand.distance - BEND;
        }
        bicep_target_angel = calculateAngelC(elbow.distance, targetDis, hand.distance);
    }

    private void Awake() {
        elbow.handLookIKController = this;
        hand.handLookIKController = this;
    }

    internal void update() {
        poc += Time.deltaTime;
    }

    internal Vector3 getHandEnd(bool realTime) {
        Vector3 elbowEnd = elbow.SphereLv1PositionCalculator(shoulder.position, realTime);
        Transform helper = PrefabCreator.SpawnDebugger(elbowEnd, "ElbowEndHelper", Time.deltaTime, 1, null).transform;
        Vector3 dir = elbowEnd - shoulder.position;
        helper.rotation = Quaternion.LookRotation(dir, elbow.direction.forward);
        Vector3 handEnd = hand.SphereLv1PositionCalculator(elbowEnd, realTime, helper);
        return handEnd;
    }

}
