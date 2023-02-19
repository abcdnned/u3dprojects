using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Utils {

    public static Vector3 halfTowards(Vector3 direction1, Vector3 direction2, float maxMagDelta) {
        return customTowards(direction1, direction2, 0.5f, maxMagDelta);
    }

    public static Vector3 customTowards(Vector3 direction1, Vector3 direction2, float f, float maxMagDelta) {
        float angel = Vector3.Angle(direction1, direction2);
        Vector3 twoFootForward = Vector3.RotateTowards(direction1, direction2, Mathf.Deg2Rad * angel * f, maxMagDelta);
        return twoFootForward;
    }

    public static Vector3 forward(Transform v) {
        Vector3 forward = v.forward;
        forward.y = 0;
        forward.Normalize();
        return forward;
    }

    public static Vector3 right(Transform v) {
        Vector3 right = v.right;
        right.y = 0;
        right.Normalize();
        return right;
    }

    public static Vector3 up(Transform v) {
        Vector3 up = v.up;
        up.Normalize();
        return up;
    }

    public static Quaternion dampTrack(Transform follower, Vector3 dir, float trackSpeed) {
        Quaternion tr = Quaternion.LookRotation(dir);       
        Quaternion r = Quaternion.Slerp(
            follower.rotation,
            tr, 
            1 - Mathf.Exp(-trackSpeed * Time.deltaTime)
        );
        return r;
    }

    public static void deltaMove(Transform origin, Vector3 target) {
        Vector3 delta = target - origin.position;
        origin.position += delta;
    }
    

}