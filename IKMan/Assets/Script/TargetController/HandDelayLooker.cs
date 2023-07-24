using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HandDelayLooker : HandLooker
{
    public float duration = 0;
    private float initH = 0;
    private float initV = 0;

    private float initH_lv2 = 0;
    private float initV_lv2 = 0;
    public Transform parent;
    public void setDuration(float t) {
        // Debug.Log(" set Duration " + t);
        duration = t;
        initTime = Time.time;
        normalizedTime = 0;
        initH = horizonAngel;
        initV = verticalAngel;
        initH_lv2 = horizonAngel_lv2;
        initV_lv2 = verticalAngel_lv2;
    }

    public void init(float duration, float h, float v) {
        setDuration(duration);
        setAngel(h, v);
        enable = true;
        enable_lv2 = false;
    }
    public void init(float duration, float h, float v, float h2, float v2) {
        setDuration(duration);
        setAngel(h, v, h2, v2);
        enable = true;
        enable_lv2 = true;
    }
    
    internal void finishFK() {
        horizonAngel = hAd;
        verticalAngel = vAd;
        if (enable_lv2) {
            horizonAngel_lv2 = hAd_lv2;
            verticalAngel_lv2 = vAd_lv2;
        }
    }
    override protected void Transfer(float dt) {
        if (parent != null) {
            Vector3 dir = transform.position - parent.position;
            transform.rotation = Quaternion.LookRotation(dir, direction.forward);
        }
        if (duration != 0) {
            bool ran = false;
            normalizedTime = Time.time - initTime;
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
            if (!enable_lv2) return;
            if (Utils.AbsDiff(horizonAngel_lv2, hAd_lv2) > MIN_ANGEL_DIFF) {
                horizonAngel_lv2 = Mathf.Lerp(initH_lv2, hAd_lv2, poc);
                ran = true;
            }
            if (Utils.AbsDiff(verticalAngel_lv2, vAd_lv2) > MIN_ANGEL_DIFF) {
                verticalAngel_lv2 = Mathf.Lerp(initV_lv2, vAd_lv2, poc);
                ran = true;
            }
            if (!ran) {
                duration = 0;
            }
        }
    }
}
