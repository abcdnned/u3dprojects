using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseArgument
{
    protected LegControllerType2 leftLegController;
    protected LegControllerType2 rightLegController;
    protected HandController leftHandController;
    protected HandController rightHandController;

    protected HandDelayLooker leftLeg;

    protected HandDelayLooker leftFoot;

    protected HandDelayLooker rightLeg;

    protected HandDelayLooker rightFoot;
    protected HandDelayLooker leftElbow;

    protected HandDelayLooker leftHand;

    protected HandDelayLooker rightElbow;

    protected HandDelayLooker rightHand;

    protected Transform rightShoulder;
    protected Transform leftShoulder;
    protected Transform rightHip;
    protected Transform leftHip;

    protected HumanIKController hic;
    protected GameObject llh;

    protected GameObject rlh;

    protected ReadTrigger leftStopSignal;

    protected ReadTrigger rightStopSignal;
    protected WalkBalance hip;




    public PoseArgument(HumanIKController humanIKController) {
        hic = humanIKController;

        leftLegController = hic.frontLeftLegStepper;
        leftHandController = hic.leftHand;
        leftLeg = leftLegController.advanceIKController.elbow;
        leftFoot = leftLegController.advanceIKController.hand;
        leftElbow = leftHandController.advanceIKController.elbow;
        leftHand = leftHandController.advanceIKController.hand;
        leftHip = leftLegController.advanceIKController.shoulder;
        leftShoulder = leftHandController.advanceIKController.shoulder;

        rightLegController = hic.frontRightLegStepper;
        rightHandController = hic.rightHand;
        rightLeg = rightLegController.advanceIKController.elbow;
        rightFoot = rightLegController.advanceIKController.hand;
        rightElbow = rightHandController.advanceIKController.elbow;
        rightHand = rightHandController.advanceIKController.hand;
        rightShoulder = rightHandController.advanceIKController.shoulder;
        rightHip = rightLegController.advanceIKController.shoulder;

        hip = hic.walkBalance;
    }

    public virtual void update() {
    }

    public virtual void run() {
        float t = Time.time;
        hic.leftHand.TryIdle();
        hic.rightHand.TryIdle();
        hic.frontLeftLegStepper.TryIdle();
        hic.frontRightLegStepper.TryIdle();
        hic.walkBalance.TryIdle(t);
    }

    protected (Vector3, Vector3, float) getSnapPosition(LegControllerType2 legController) {
        Vector3 handEnd = legController.handLookIKController.getLv1HandEnd(true);
        Vector3 dir = handEnd - legController.advanceIKController.shoulder.transform.position;
        RaycastHit hit;
        Vector3 start = legController.advanceIKController.shoulder.transform.position;
        // float maxDis = legController.handLookIKController.getMaxFootDis();
        float maxDis = dir.magnitude + hic.ap.snapBlendDis;
        if (Physics.Raycast(start, dir, out hit, maxDis, 1 << 9)) {
            // Debug.DrawLine(hit.point, legController.advanceIKController.shoulder.transform.position, Color.blue, 0.1f);
            return (hit.point, hit.normal, (hit.point - legController.advanceIKController.shoulder.transform.position).magnitude - dir.magnitude);
        }
        return (Vector3.zero, Vector3.zero, 0);
        
    }

    protected void legUpdate(LegControllerType2 legController, ref GameObject lh, ref ReadTrigger stopSignal) {
        (Vector3, Vector3, float) hit = getSnapPosition(legController);
        Vector3 snapPos = hit.Item1;
        Vector3 snapNormal = hit.Item2;
        float hitDis = hit.Item3;
        if (snapPos.magnitude > 0 && hitDis <= 0) {
            DrawUtils.drawBall(snapPos, 0.02f);
            if (lh == null) {
                lh = PrefabCreator.SpawnDebugger(snapPos, PrefabCreator.POSITION_HELPER,
                                                        PrefabCreator.LONG_LIVE, 1, null);
                stopSignal = new ReadTrigger(false);
                legController.handLookIKController.init(0,
                                                            snapPos,
                                                            hic.body.transform,
                                                            lh.transform,
                                                            true,
                                                            stopSignal);
            }
            lh.transform.position = snapPos;
        } else {
            if (lh != null) {
                GameObject.Destroy(lh);
                lh = null;
                ((LegHandMove)legController.move).getFootRotation = null;
                stopSignal?.set();
            }
        }
        if (snapPos.magnitude > 0) {
            Vector3 projectedForward = Vector3.ProjectOnPlane(hic.walkPointer.transform.forward, snapNormal);
            Quaternion baseOnSnap = Quaternion.LookRotation(projectedForward, snapNormal);
            ((LegHandMove)legController.move).getFootRotation = () => {  if (hitDis <= 0) {
                                                                            return baseOnSnap;
                                                                        } else {
                                                                            Quaternion baseOnLeg = ((LegHandMove)legController.move).getBaseOnArmRotation();
                                                                            Quaternion r = Quaternion.Slerp(baseOnLeg,
                                                                                                            baseOnSnap,
                                                                                                            (hic.ap.snapBlendDis - hitDis)
                                                                                                               / hic.ap.snapBlendDis);
                                                                            // Debug.Log(" blend snap " + (hic.ap.snapBlendDis - hitDis) + " blendDis " + hic.ap.snapBlendDis);
                                                                            return r;
                                                                        }};
        }
    }

    internal void exit() {
        GameObject.Destroy(llh);
        GameObject.Destroy(rlh);
        llh = null;
        rlh = null;
        ((LegHandMove)leftLegController.move).getFootRotation = null;
        ((LegHandMove)rightLegController.move).getFootRotation = null;
        leftStopSignal?.set();
        rightStopSignal?.set();
    }

    protected (Vector3, Vector3) hitStandPosition() {
        RaycastHit hit;
        if (Physics.Raycast(hip.transform.position, -hic.gravityUp, out hit, hic.ap.standHeight, 1 << 9)) {
            Debug.DrawLine(hip.transform.position, hit.point, Color.red, 3);
            return (hit.point, hit.normal);
        }
        return (Vector3.zero, Vector3.zero);
        
    }
}