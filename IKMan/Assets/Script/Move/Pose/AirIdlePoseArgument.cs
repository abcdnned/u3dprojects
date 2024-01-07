using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AirIdlePoseArgument : PoseArgument
{
    internal bool landed = false;
    private float initTime;
    private float enableTime = 0.1f;
    private int state = 0;

    public AirIdlePoseArgument(HumanIKController humanIKController) : base(humanIKController) {

    }

    public override void update() {
        if (state == 0) {
            state++;
            initTime = Time.time;
        }
        if (state == 1) {
            if (land()) {
                landed = true;
                state++;
            }
        }
    }

    public override void run() {
        float t = Time.time;
        hic.leftHand.TryAir(hic.ap.runHalfDuration, t, hic.ap.transferSpeedLong);
        hic.rightHand.TryAir(0, t, hic.ap.transferSpeedLong);
        hic.frontLeftLegStepper.TryAir(0, t, hic.ap.transferSpeedLong);
        hic.frontRightLegStepper.TryAir(hic.ap.transferSpeedLong, t, hic.ap.transferSpeedLong);
        hic.walkBalance.TryAir();
    }

    private bool land() {
        float t = Time.time - initTime;
        if (t <= enableTime) {
            return false;
        } else {
            // (Vector3, Vector3) hit = hitStandPosition();
            // Vector3 snapPos = hit.Item1;
            // int hitLayer = 1 << 9;
            // bool hitGround = isAirRayHit(hic.spin1.transform, hic.gravityUp, hitLayer, hic.ap.standHeight + hic.ap.airRayCast);
            // hitGround |= isAirRayHit(leftLegController.transform, hic.gravityUp, hitLayer);
            bool fall = Vector3.Dot(hic.moveController.getVelocity(), hic.gravityUp) <= 0;
            bool hitGround = hic.moveController.onGround() > 0;
            if (hitGround && fall) {
                return true;
            }
        }
        return false;
    }


}