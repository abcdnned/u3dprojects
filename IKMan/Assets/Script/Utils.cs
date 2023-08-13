using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using static UnityEngine.Mathf;
public class Utils {
    public static float AIR_RAY_CAST_DISTANCE = 1.5f;
    public static float DOWN_RAY_OFFSET = 10;

    public static Vector3 halfTowards(Vector3 direction1, Vector3 direction2, float maxMagDelta) {
        return customTowards(direction1, direction2, 0.5f, maxMagDelta);
    }

    public static Vector3 customTowards(Vector3 direction1, Vector3 direction2, float f, float maxMagDelta) {
        float angel = Vector3.Angle(direction1, direction2);
        Vector3 twoFootForward = Vector3.RotateTowards(direction1, direction2, Mathf.Deg2Rad * angel * f, maxMagDelta);
        return twoFootForward;
    }

    public static Vector3 forwardFlat(Transform v) {
        Vector3 forward = v.forward;
        forward.y = 0;
        forward.Normalize();
        return forward;
    }
    public static Vector3 forward(Transform v) {
        Vector3 forward = v.forward;
        forward.Normalize();
        return forward;
    }

    public static Vector3 snapTo(Vector3 start) {
        return snapTo(start, Vector3.up, 0);
    }
    public static Vector3 snapTo(Vector3 position, Vector3 normal, float h) {
        if (h < 0) {
            h = 0;
        }
        RaycastHit hit;
        Vector3 start = position + normal * DOWN_RAY_OFFSET;
        // Debug.DrawLine(start, start + ((DOWN_RAY_OFFSET + AIR_RAY_CAST_DISTANCE) * -normal), Color.red, 30);
        if (Physics.Raycast(start, -normal, out hit, DOWN_RAY_OFFSET + AIR_RAY_CAST_DISTANCE, ~0)) {
            // Debug.Log(" snapTo hitt! ");
            Vector3 desiredPos = hit.point + normal * h;
            desiredPos.y = h;
            return desiredPos;
        }
        return position;
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
    public static void deltaRotate(Transform origin, Quaternion target) {
        Quaternion diff = origin.rotation.Diff(target);
        origin.rotation *= diff;
    }

    public static Vector3 copy(Vector3 source) {
        Vector3 r = new Vector3(source.x, source.y, source.z);
        return r;
    }
    public static bool IsSecondPositionBetween(Vector3 position1, Vector3 position2, Vector3 position3, Vector3 direction)
    {
        Vector3 v1 = position2 - position1;
        Vector3 v2 = position3 - position1;
        float dot1 = Vector3.Dot(v1, direction);
        float dot2 = Vector3.Dot(v2, direction);

        // Check if the second position is between the other two positions
        if (dot1 * dot2 <= 0)
        {
            return false;
        }

        return true;
    }
    public static void GetForwardAndRight(Vector3 A, Vector3 B, out Vector3 forward, out Vector3 right) {
        forward = (B - A).normalized;
        right = Vector3.Cross(Vector3.up, forward);
    }

    public static Vector3 GetMiddleLiftPoint(Vector3 a, Vector3 b, float distance)
    {
        Vector3 middle = (a + b) / 2f;
        middle += Vector3.up * distance;
        return middle;
    }

    public static void OutputThreadInfo() {
        Thread currentThread = Thread.CurrentThread;
        Debug.Log("Current thread ID: " + currentThread.ManagedThreadId);
        Debug.Log("Current thread name: " + currentThread.Name);
    }
    
    public static float AbsDiff(float a, float b) {
        return Abs(a - b);
    }

    public static void JointSetLimit(CharacterJoint joint, float l, float h, float s1 = 0, float s2 = 0) {
        SoftJointLimit ll = new SoftJointLimit();
        ll.limit = l;
        joint.lowTwistLimit = ll;
        SoftJointLimit hl = new SoftJointLimit();
        hl.limit = h;
        joint.highTwistLimit = hl;
        SoftJointLimit s1l = new SoftJointLimit();
        s1l.limit = s1;
        joint.swing1Limit = s1l;
        SoftJointLimit s2l = new SoftJointLimit();
        s2l.limit = s2;
        joint.swing2Limit = s2l;

    }

}