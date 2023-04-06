using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steper
{
    Vector3 lastPosition = Vector3.zero;
    Vector3 forward;

    Vector3 right;

    Transform body;

    Transform transform;

    float duration;

    float timeElapsed;

    public int state;

    int wpCount = 0;

    Vector3[] wp;

    public static int LEFP = 0;
    public static int BEARZ = 1;
    int lerpFunction = 0;

    int mode;


    /**
     * forward and right for Dot
     * duration: move duration
     * realForward, realRight, body: real move direction and right
     * mode: 0 use body as move direction,
     *       1 use vector3 as direction not rotate with body,
     *       2 rotate with body
     * target: the target to move
     * points move milestones
     */
    public Steper(Vector3 forward,
                  Vector3 right,
                  float duration,
                  int function,
                  Transform body,
                  int mode,
                  Transform target,
                  Vector3[] points) {
        // Init field.
        this.forward = forward;
        this.right = right;
        this.duration = duration;
        this.lerpFunction = function;
        this.body = body;
        transform = target;
        setwp(points);
        this.mode = mode;
        // Init state.
        timeElapsed = 0;
        state = 0;
    }
    public void setwp(Vector3[] points) {
        wpCount = points.Length;
        wp = new Vector3[wpCount];
        for (int i = 0; i < points.Length; ++i) {
            wp[i] = Utils.copy(points[i]);
        }
    }

    public void step(float dt) {
        if (state == 0) {
            state = 1;
            lastPosition = wp[0];
            // Debug.DrawLine(wp[0], wp[0] + forward * 2, Color.red, 10);
            // Debug.DrawLine(wp[0], wp[wpCount - 1], Color.blue, 10);
        }
        timeElapsed += dt;
        float poc = timeElapsed / duration;
        Vector3 targetPosition = calculate(poc);
        Vector3 delta = targetPosition - lastPosition;
        if (mode == 1) {
            transform.position += delta;
            Debug.Log(" ++ ");
        } else {
            Vector3 forward3 = Utils.forward(body.transform);
            Vector3 right3 = Utils.right(body.transform);
            transform.position += forward3 * Vector3.Dot(delta, forward)
                                + right3 * Vector3.Dot(delta, right)
                                + Vector3.up * Vector3.Dot(delta, Vector3.up);
        }
        lastPosition = targetPosition;
    }
    public delegate Vector3 Function(Vector3 s, Vector3 e, float v);

    public delegate Vector3 Function2(Vector3 s, Vector3 m, Vector3 e, float v);

    private Vector3 calculate(float poc) {
        if (wpCount == 3) {
            return getFunction3(lerpFunction).Invoke(wp[0], wp[1], wp[2], poc);
        }
        Function f = getFunction2(lerpFunction);
        Vector3 v = f.Invoke(wp[0], wp[1], poc);
        return v;
    }

    public static Vector3 lerp(Vector3 start, Vector3 end, float poc) {
        return Vector3.Lerp(start, end, poc);
    }

    public static Vector3 Bearz(Vector3 start, Vector3 middle, Vector3 end, float poc)
    {
        Vector3 targetPosition =
            Vector3.Lerp(
                Vector3.Lerp(start, middle, poc),
                Vector3.Lerp(middle, end, poc),
                poc
            );
        return targetPosition;
    }

    private Function getFunction2(int type) {
        if (lerpFunction == LEFP) {
            return lerp;
        }
        return lerp;
    }

    private Function2 getFunction3(int type) {
        if (lerpFunction == BEARZ) {
            return Bearz;
        }
        return Bearz;
    }

}
