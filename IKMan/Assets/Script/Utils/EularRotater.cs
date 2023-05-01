using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EularRotater
{
    Transform body;
    Transform target;
    Vector3 targetEular;
    Vector3 initEular;
    Quaternion initQuaternion;
    Quaternion targetQuaternion;
    float duration;
    float timeElapsed = 0;

    int state = 0;

    // Target angel based on body: (up, right, left)
    Vector3 targetAngel;
    public EularRotater(Transform body, Transform target,
                   float duration,
                   Vector3 targetEular)
    {
        this.body = body;
        this.target = target;
        this.targetEular = targetEular;
        this.duration = duration;
    }

    public void rot(float dt) {
        // use hand look as a rotation hint, hand will always look to the hint.
        // then I can move hint to control to hand's rotation.
        if (state == 0) {
            initEular = target.localEulerAngles;
            initQuaternion = target.rotation;
            targetQuaternion = Quaternion.Euler(targetEular.x,
                                                targetEular.y,
                                                targetEular.z);
            targetQuaternion = body.rotation * targetQuaternion;
            state = 1;
        }
        timeElapsed += dt;
        float poc = timeElapsed / duration;
        Quaternion r = Quaternion.Slerp(initQuaternion,
                                        targetQuaternion,
                                        poc);
        // Vector3 r = Vector3.Lerp(
        //     initEular,
        //     targetEular,
        //     poc
        // );
        target.localRotation = r;
    }
}
