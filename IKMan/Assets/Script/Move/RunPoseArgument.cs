using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RunPoseArgument : PoseArgument
{
    // private float snapAngel = 20;

    private GameObject llh;

    private GameObject rlh;

    private ReadTrigger leftStopSignal;

    private ReadTrigger rightStopSignal;
    private LegRunMove.acceptLegRunBeat leftBeat;
    private LegRunMove.acceptLegRunBeat rightBeat;

    




    public RunPoseArgument(HumanIKController humanIKController) : base(humanIKController) {

    }

    public override void update() {
        legUpdate(leftLegController, ref llh, leftStopSignal);
        legUpdate(rightLegController, ref rlh, rightStopSignal);
        // (Vector3, Vector3) hit = getSnapPosition(leftLegController);
        // Vector3 snapPos = hit.Item1;
        // Vector3 snapNormal = hit.Item2;
        // if (snapPos.magnitude > 0) {
        //     DrawUtils.drawBall(snapPos, 0.02f);
        //     if (llh == null) {
        //         llh = PrefabCreator.SpawnDebugger(snapPos, PrefabCreator.POSITION_HELPER,
        //                                                 PrefabCreator.LONG_LIVE, 1, null);
        //         handLookIkStopSignal = new ReadTrigger(false);
        //         leftLegController.handLookIKController.init(0,
        //                                                     snapPos,
        //                                                     hic.body.transform,
        //                                                     llh.transform,
        //                                                     true,
        //                                                     handLookIkStopSignal);
        //     }
        //     Vector3 projectedForward = Vector3.ProjectOnPlane(leftFoot.transform.forward, snapNormal);
        //     ((LegRunMove)leftLegController.move).getFootRotation = () => Quaternion.LookRotation(projectedForward, snapNormal);
        //     llh.transform.position = snapPos;
        // } else {
        //     if (llh != null) {
        //         GameObject.Destroy(llh);
        //         llh = null;
        //         ((LegRunMove)leftLegController.move).getFootRotation = null;
        //         handLookIkStopSignal.set();
        //     }
        //     if (rlh != null) {
        //         GameObject.Destroy(rlh);
        //         rlh = null;
        //     }
        // }
    }

    public override void run() {
        float t = Time.time;
        // hic.leftHand.TryRun(hic.ap.runHalfDuration * 2, t);
        // hic.rightHand.TryRun(0, t);
        leftBeat = hic.frontLeftLegStepper.TryRun(0, t);
        rightBeat = hic.frontRightLegStepper.TryRun(2 * hic.ap.runHalfDuration, t);
    }

    // public bool shouldLegSnap(LegControllerType2 legController) {
    //     Vector3 v1 = -Vector3.up;
    //     Vector3 handEnd = legController.handLookIKController.getLv1HandEnd(true);
    //     Vector3 v2 = handEnd - legController.advanceIKController.shoulder.transform.position;
    //     float a = Vector3.Angle(v1, v2);
    //     LegRunMove move = (LegRunMove)legController.move;
    //     int[] indexMapping = move.getIndexMapping();
    //     int tide = indexMapping[move.previousIndex] - indexMapping[move.previousPreviousIndex];
    //     Debug.Log(" a " + a);
    //     Debug.Log(" tide " + tide);
    //     if (a <= snapAngel && tide == 1) {
    //         return true;
    //     }
    //     return false;
    // }

    public (Vector3, Vector3, float) getSnapPosition(LegControllerType2 legController) {
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

    private void legUpdate(LegControllerType2 legController, ref GameObject lh, ReadTrigger stopSignal) {
        (Vector3, Vector3, float) hit = getSnapPosition(legController);
        Vector3 snapPos = hit.Item1;
        Vector3 snapNormal = hit.Item2;
        float hitDis = hit.Item3;
        if (snapPos.magnitude > 0) {
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
            Vector3 projectedForward = Vector3.ProjectOnPlane(hic.walkPointer.transform.forward, snapNormal);
            Quaternion baseOnSnap = Quaternion.LookRotation(projectedForward, snapNormal);
            ((LegRunMove)legController.move).getFootRotation = () => {  if (hitDis <= 0) {
                                                                            return baseOnSnap;
                                                                        } else {
                                                                            Quaternion baseOnLeg = ((LegRunMove)legController.move).getBaseOnArmRotation();
                                                                            Quaternion r = Quaternion.Slerp(baseOnLeg,
                                                                                                            baseOnSnap,
                                                                                                            (hic.ap.snapBlendDis - hitDis)
                                                                                                               / hic.ap.snapBlendDis);
                                                                            Debug.Log(" blend snap " + (hic.ap.snapBlendDis - hitDis));
                                                                            return r;
                                                                        }};
            lh.transform.position = snapPos;
        } else {
            if (lh != null) {
                GameObject.Destroy(lh);
                lh = null;
                ((LegRunMove)legController.move).getFootRotation = null;
                stopSignal?.set();
            }
        }
    }
}