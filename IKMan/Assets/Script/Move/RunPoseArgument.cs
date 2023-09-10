using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RunPoseArgument : PoseArgument
{
    private float snapAngel = 20;

    private float maxDis = 10;

    private GameObject llh;

    private GameObject rlh;

    private ReadTrigger handLookIkStopSignal;

    




    public RunPoseArgument(HumanIKController humanIKController) : base(humanIKController) {

    }

    public override void update() {
        if (shouldLegSnap(leftLegController)) {
            // Debug.DrawLine(leftHip.position, leftFoot.transform.position, Color.red, 0.02f);
            Vector3 snapPos = getSnapPosition(leftLegController);
            if (snapPos.magnitude > 0) {
                DrawUtils.drawBall(snapPos, 0.02f);
                if (llh == null) {
                    Debug.Log(" rwrew ");
                    llh = PrefabCreator.SpawnDebugger(snapPos, PrefabCreator.POSITION_HELPER,
                                                            PrefabCreator.LONG_LIVE, 1, null);
                    handLookIkStopSignal = new ReadTrigger(false);
                    leftLegController.handLookIKController.init(0,
                                                                snapPos,
                                                                hic.body.transform,
                                                                llh.transform,
                                                                true,
                                                                handLookIkStopSignal);
                }
                llh.transform.position = snapPos;
            }
        } else {
            if (llh != null) {
                GameObject.Destroy(llh);
                llh = null;
                handLookIkStopSignal.set();
                // leftLegController.handLookIKController.transferCurPosToLv1();
                Debug.Log(" llh destroy ");
            }
            if (rlh != null) {
                GameObject.Destroy(rlh);
                rlh = null;
            }
        }
        // if (shouldLegSnap(rightLegController)) {
        //     Debug.DrawLine(rightHip.position, rightFoot.transform.position, Color.red, 0.02f);
        // }
    }

    public override void run() {
        float t = Time.time;
        // hic.leftHand.TryRun(hic.ap.runHalfDuration * 2, t);
        // hic.rightHand.TryRun(0, t);
        hic.frontLeftLegStepper.TryRun(0, t);
        // hic.frontRightLegStepper.TryRun(2 * hic.ap.runHalfDuration, t);
    }

    public bool shouldLegSnap(LegControllerType2 legController) {
        Vector3 v1 = -Vector3.up;
        Vector3 handEnd = legController.handLookIKController.getHandEnd(true);
        Vector3 v2 = handEnd - legController.advanceIKController.shoulder.transform.position;
        float a = Vector3.Angle(v1, v2);
        LegRunMove move = (LegRunMove)legController.move;
        int[] indexMapping = move.getIndexMapping();
        int tide = indexMapping[move.previousIndex] - indexMapping[move.previousPreviousIndex];
        Debug.Log(" a " + a);
        Debug.Log(" tide " + tide);
        if (a <= snapAngel && tide == 1) {
            return true;
        }
        return false;
    }

    public Vector3 getSnapPosition(LegControllerType2 legController) {
        Vector3 handEnd = legController.handLookIKController.getHandEnd(true);
        Vector3 dir = handEnd - legController.advanceIKController.shoulder.transform.position;
        RaycastHit hit;
        Vector3 start = legController.advanceIKController.shoulder.transform.position;
        if (Physics.Raycast(start, dir, out hit, maxDis, 1 << 9)) {
            // Debug.DrawLine(hit.point, legController.advanceIKController.shoulder.transform.position, Color.blue, 0.1f);
            return hit.point;
        }
        return Vector3.zero;
        
    }
}