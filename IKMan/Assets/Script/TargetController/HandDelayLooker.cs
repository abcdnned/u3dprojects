using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HandDelayLooker : HandLooker
{
    public float horizonAngel_lv2 = 0;
    public float verticalAngel_lv2 = 0;
    public float hAd_lv2 = 0;
    public float vAd_lv2 = 0;
    public float normalizedTime = 0;
    public float duration = 0;
    private float initH = 0;
    private float initV = 0;
    public Transform parent;
    public void setDuration(float t) {
        // Debug.Log(" set Duration " + t);
        duration = t;
        normalizedTime = 0;
        initH = horizonAngel;
        initV = verticalAngel;
    }
    public void init(float duration, float h, float v) {
        setDuration(duration);
        setAngel(h, v);
    }
    override protected void Transfer(float dt) {
        if (parent != null) {
            Vector3 dir = transform.position - parent.position;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        if (duration != 0) {
            bool ran = false;
            normalizedTime += dt;
            float poc = normalizedTime / duration;
            if (Utils.AbsDiff(horizonAngel, hAd) > MIN_ANGEL_DIFF) {
                // Debug.Log(" poc " + poc);
                horizonAngel = Mathf.Lerp(initH, hAd, poc);
                // Debug.Log(" h " + horizonAngel);
                ran = true;
            }
            if (Utils.AbsDiff(verticalAngel, vAd) > MIN_ANGEL_DIFF) {
                // Debug.Log(" poc2 " + poc);
                verticalAngel = Mathf.Lerp(initV, vAd, poc);
                ran = true;
                // Debug.Log(" v " + verticalAngel);
            }
            if (!ran) {
                duration = 0;
            }
        }
    }
}
