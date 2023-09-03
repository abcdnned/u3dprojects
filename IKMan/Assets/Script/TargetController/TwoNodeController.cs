using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoNodeController : TargetController
{
    public HandDelayLooker MiddleNode;
    public HandDelayLooker EndNode;
    public Transform ParentNode;
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
        target.vAd_lv2 = source.verticalAngel_lv2;
    }
}