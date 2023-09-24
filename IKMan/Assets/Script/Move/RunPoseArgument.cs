using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RunPoseArgument : PoseArgument
{
    // private float snapAngel = 20;

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
        hic.leftHand.TryRun(hic.ap.runHalfDuration * 2, t);
        hic.rightHand.TryRun(0, t);
        hic.frontLeftLegStepper.TryRun(0, t);
        hic.frontRightLegStepper.TryRun(2 * hic.ap.runHalfDuration, t);
        hic.walkBalance.TryRun((LegRunMove)hic.frontLeftLegStepper.move, (LegRunMove)hic.frontRightLegStepper.move);
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

}