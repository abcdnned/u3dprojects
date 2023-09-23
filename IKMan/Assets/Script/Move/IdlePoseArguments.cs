using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePoseArgument : PoseArgument
{
    // private float snapAngel = 20;


    public IdlePoseArgument(HumanIKController humanIKController) : base(humanIKController) {

    }

    public override void update() {
    }

    public override void run() {
        hic.leftHand.TryIdle();
        hic.rightHand.TryIdle();
        hic.frontLeftLegStepper.TryIdle();
        hic.frontRightLegStepper.TryIdle();
        hic.walkBalance.TryIdle();
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
                                                                            // Debug.Log(" blend snap " + (hic.ap.snapBlendDis - hitDis));
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
