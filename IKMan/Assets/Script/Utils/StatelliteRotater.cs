using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatelliteRotater
{
    Transform main;
    Transform statellite;
    Quaternion targetRotation;
    Quaternion initRotation;
    Quaternion lastRotation;
    float duration;
    float timeElapsed = 0;
    int state = 0;

    // Target angel based on body: (up, right, left)
    float targetAngel;
    Quaternion negativeRotation;
    public StatelliteRotater(Transform main,
                             Transform statellite,
                             float duration,
                             float targetAngel)
    {
        this.main = main;
        this.statellite = statellite;
        this.targetAngel = targetAngel;
        this.duration = duration;
        targetRotation = Quaternion.AngleAxis(targetAngel, Utils.up(main));
    }

    public void rot(float dt) {
        if (state == 0) {
            initRotation = statellite.rotation;
            state = 1;
            lastRotation = initRotation;
        }
        timeElapsed += dt;
        if (state == 1) {
            // Rotate face
            float poc = timeElapsed / duration;
            Quaternion r = Quaternion.Slerp(
                initRotation,
                targetRotation,
                poc
            );
            Quaternion diff = lastRotation.Diff(r);
            statellite.rotation *= diff;
            lastRotation = r;
            state = timeElapsed > duration ? 2 : 1;
            // Rotate around orbit
        }
    }
}
