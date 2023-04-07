using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaternionRotater
{
    Transform body;
    Transform target;
    Quaternion targetRotation;
    Quaternion initRotation;
    Quaternion lastRotation;
    float duration;
    float timeElapsed = 0;

    int state = 0;

    // Target angel based on body: (up, right, left)
    Vector3 targetAngel;
    public QuaternionRotater(Transform body, Transform target,
                   float duration,
                   Vector3 targetAngel)
    {
        this.body = body;
        this.target = target;
        this.targetAngel = targetAngel;
        this.duration = duration;
    }

    public void rot(float dt) {
        if (state == 0) {
            initRotation = target.rotation;
            targetRotation = body.rotation * Quaternion.AngleAxis(targetAngel.x, Vector3.up);
            state = 1;
            lastRotation = initRotation;
        }
        timeElapsed += dt;
        float poc = timeElapsed / duration;
        Quaternion r = Quaternion.Slerp(
            initRotation,
            targetRotation,
            poc
        );
        Quaternion diff = lastRotation.Diff(r);
        target.rotation *= diff;
        lastRotation = r;
    }
}
