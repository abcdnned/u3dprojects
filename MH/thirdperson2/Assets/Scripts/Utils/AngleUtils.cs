using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class AngleUtils : Object
{
    public static float AngleBetween(Transform source, Transform target) {
        Vector3 targetDirection = target.position - source.position;
        float viewableAngle = Vector3.Angle(targetDirection, source.forward);
        return viewableAngle;
    }

    public static bool AngleRange(Transform source, Transform target, float min, float max) {
        float angle = AngleBetween(source, target);
        return angle <= max && angle >= min;
    }
}

}