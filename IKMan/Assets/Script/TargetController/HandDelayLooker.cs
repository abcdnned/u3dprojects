using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HandDelayLooker : HandLooker
{
    public float duration = 0;
    private float initH = 0;
    private float initV = 0;

    private float initH_lv2 = 0;
    private float initV_lv2 = 0;
    public Transform parent;
    public void setDuration(float t) {
        // Debug.Log(" set Duration " + t);
        duration = t;
        initTime = Time.time - Time.deltaTime; // Start at current frame after init
        normalizedTime = 0;
        initH = horizonAngel;
        initV = verticalAngel;
        initH_lv2 = horizonAngel_lv2;
        initV_lv2 = verticalAngel_lv2;
    }

    public void init(float duration, float h, float v) {
        setDuration(duration);
        setAngel(h, v);
        enable = true;
        enable_lv2 = false;
        // Debug.Log(" init ");
    }
    public void init(float duration, float h, float v, float h2, float v2) {
        setDuration(duration);
        setAngel(h, v, h2, v2);
        enable = true;
        enable_lv2 = true;
    }
    
    internal void finishIKFKTransfer() {
        hAd = horizonAngel;
        vAd = verticalAngel;
    }

    override protected void Transfer(float dt) {
        if (duration != 0) {
            bool ran = false;
            normalizedTime = Time.time - initTime;
            float poc = normalizedTime / duration;
            // if (poc > 1 && (normalizedTime - Time.deltaTime) / duration < 1) {
            //     Debug.Log(" finish fk ");
            //     if (handLookIKController != null && handLookIKController.getIKSequence(this) > 0) {
            //         Debug.Log(" transfer cur pos to lv1 ");
            //         handLookIKController.transferCurPosToLv1();
            //         finishIKFKTransfer();
            //     }
            // }
            if (Utils.AbsDiff(horizonAngel, hAd) > MIN_ANGEL_DIFF
                || Utils.AbsDiff(verticalAngel, vAd) > MIN_ANGEL_DIFF) {
                Vector3 initSpherePoint = calculateInitSpherePoint();
                Vector3 targetSpherePoint = calculateTargetSpherePoint();
                // DrawUtils.drawBall(sun.transform.position + targetSpherePoint, 0.02f);
                Vector3 normal = Vector3.Cross(initSpherePoint, targetSpherePoint);
                Vector3 upward = Quaternion.AngleAxis(1, normal) * targetSpherePoint;
                Quaternion initRotation = Quaternion.LookRotation(initSpherePoint, upward);
                Quaternion targetRotation = Quaternion.LookRotation(targetSpherePoint, upward);
                Quaternion pocRotation = Quaternion.Slerp(initRotation, targetRotation, poc);
                Vector3 pocSpherePoint = calculatePocSpherePoint(pocRotation);
                (float, float)realData = calculateRealHVfromSpherePoint(pocSpherePoint);
                horizonAngel = realData.Item1;
                verticalAngel = realData.Item2;
                ran = true;
            }
            // if (Utils.AbsDiff(horizonAngel, hAd) > MIN_ANGEL_DIFF) {
            //     horizonAngel = Mathf.Lerp(initH, hAd, poc);
            //     ran = true;
            // }
            // if (Utils.AbsDiff(verticalAngel, vAd) > MIN_ANGEL_DIFF) {
            //     verticalAngel = Mathf.Lerp(initV, vAd, poc);
            //     ran = true;
            // }
            if (!enable_lv2) return;
            if (Utils.AbsDiff(horizonAngel_lv2, hAd_lv2) > MIN_ANGEL_DIFF) {
                horizonAngel_lv2 = Mathf.Lerp(initH_lv2, hAd_lv2, poc);
                ran = true;
            }
            if (Utils.AbsDiff(verticalAngel_lv2, vAd_lv2) > MIN_ANGEL_DIFF) {
                verticalAngel_lv2 = Mathf.Lerp(initV_lv2, vAd_lv2, poc);
                ran = true;
            }
            if (!ran) {
                duration = 0;
            }
        }
    }

    internal override void lookToParent() {
        if (parent != null) {
            Vector3 dir = transform.position - parent.position;
            transform.rotation = Quaternion.LookRotation(dir, direction.forward);
        }
    }

    private (float, float) calculateRealHVfromSpherePoint(Vector3 pocSpherePoint)
    {
        Vector3 hv = Vector3.ProjectOnPlane(pocSpherePoint, Vector3.up);
        float hv_sign = pocSpherePoint.x > 0 ? 1 : -1;
        float vv_sign = pocSpherePoint.y > 0 ? 1 : -1;
        float hangel = hv_sign * Vector3.Angle(hv, Vector3.forward);
        float vangel = vv_sign * Vector3.Angle(pocSpherePoint, hv);
        return (hangel, vangel);
    }

    private Vector3 calculatePocSpherePoint(Quaternion pocRotation)
    {
        Vector3 poc = pocRotation * Vector3.forward;
        // DrawUtils.drawBall(sun.transform.position + poc, 0.02f);
        return poc;
    }

    private Vector3 calculateTargetSpherePoint()
    {
        Vector3 forward = Quaternion.AngleAxis(hAd, Vector3.up) * Vector3.forward;
        Vector3 secondNormal = Vector3.Cross(forward,Vector3.up);
        Vector3 r = Quaternion.AngleAxis(vAd, secondNormal) * forward;
        return r;
    }

    private Vector3 calculateInitSpherePoint()
    {
        Vector3 forward = Quaternion.AngleAxis(initH, Vector3.up) * Vector3.forward;
        Vector3 secondNormal = Vector3.Cross(forward, Vector3.up);
        Vector3 r = Quaternion.AngleAxis(initV, secondNormal) * forward;
        return r;
    }

}
