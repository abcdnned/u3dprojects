using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoNodeController : TargetController
{
    public HandDelayLooker MiddleNode;
    public HandDelayLooker EndNode;
    public Transform ParentNode;
    public HandLookIKController handLookIKController;
    [SerializeField]internal float isRightPart = 1;

    protected override void assemblyComponent() {
        handLookIKController = ParentNode.gameObject.GetComponent<HandLookIKController>();
    }
    internal void SyncIKSample(string sampleName, float duration, bool horizon_mirror = false) {
        // Debug.Log(" isRightHand " + isRightHand);
        String elbow = IKSampleNames.ELBOW + "_" + sampleName;
        String hand = IKSampleNames.HAND + "_" + sampleName;
        HandDelayLooker elbowLooker = hic.poseManager.handDelayLookerMap[elbow];
        HandDelayLooker handLooker = hic.poseManager.handDelayLookerMap[hand];
        if (MiddleNode != null && EndNode != null) {
            MiddleNode.setDuration(duration);
            EndNode.setDuration(duration);
            SyncTwoHandLooker(elbowLooker, MiddleNode, horizon_mirror);
            SyncTwoHandLooker(handLooker, EndNode, horizon_mirror);
            // HandElbow.init(duration, elbowLooker.hAd, elbowLooker.vAd,
            //                          elbowLooker.hAd_lv2, elbowLooker.vAd_lv2);
            // HandFK.init(duration, handLooker.hAd, handLooker.vAd,
            //                       handLooker.hAd_lv2, handLooker.vAd_lv2);
        }
    }
    protected void SyncTwoHandLooker(HandLooker source, HandLooker target, bool horizon_mirror) {
        if (source == null || target == null) return;
        target.enable_lv2 = source.enable_lv2;
        target.hAd = horizon_mirror ? -source.horizonAngel : source.horizonAngel;
        target.vAd = source.verticalAngel;
        target.hAd_lv2 = source.horizonAngel_lv2;
        target.vAd_lv2 = horizon_mirror ? -source.verticalAngel_lv2 : source.verticalAngel_lv2;
    }

    internal Quaternion LookToArmLook(float angel = 90, bool sideEffect = true) {
        Vector3 v1 = getArmDirection();
        Vector3 v2 = getBicepDirection();
        Quaternion rotate = Quaternion.AngleAxis(angel, Vector3.Cross(v2, v1));
        v1 = rotate * v1;
        Quaternion look = Quaternion.LookRotation(v1,
                                                  -getArmDirection());
        if (sideEffect) {
            transform.rotation = look;
        }
        return look;
        // Quaternion r = Quaternion.Slerp(transform.rotation,
        //                                 look,
        //                                 1 - Mathf.Exp(-handLookSpeed * Time.deltaTime));
        // Quaternion hr = Quaternion.AngleAxis(armLookRotationH, transform.right);
        // transform.rotation = hr * transform.rotation;
        // Quaternion vr = Quaternion.AngleAxis(armLookRotationV, transform.up);
        // transform.rotation = vr * transform.rotation;
    }

    public Vector3 getArmDirection() {
        Vector3 r = EndNode.transform.position - MiddleNode.transform.position;
        return r.normalized;
    }

    public Vector3 getBicepDirection() {
        Vector3 r = MiddleNode.transform.position - ParentNode.transform.position;
        return r.normalized;
    }

    public virtual bool IsRightPart() {
        return isRightPart == 1;
    }

}