using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLooker : MonoBehaviour
{
    public Transform sun;
    public Transform direction;
    public float distance;
    public float horizonAngel;
    public float verticalAngel;
    public float speed = 10;
    public float hAd = 0;
    public float vAd = 0;
    public bool enable = true;
    public float horizonAngel_lv2 = 0;
    public float verticalAngel_lv2 = 0;
    public float hAd_lv2 = 0;
    public float vAd_lv2 = 0;
    public bool enable_lv2 = false;

    public static float MIN_ANGEL_DIFF = 0.05f;

    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        hAd = horizonAngel;
        vAd = verticalAngel;
    }

    public void setAngel(float h, float v) {
        hAd = h;
        vAd = v;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!enable) return;
        Transfer(Time.deltaTime);
        Vector3 f = Utils.forward(direction);
        Vector3 u = Utils.up(direction);
        // Debug.DrawRay(sun.position, f, Color.blue);
        // Debug.DrawRay(sun.position, u, Color.green);
        Vector3 lv1_pos = SphereRotationCalculator(f, u, horizonAngel, verticalAngel);
        if (enable_lv2) {
            // Calculate level 2
            Vector3 f2 = (lv1_pos - sun.position).normalized;
            Vector3 u2 = Vector3.Cross(f, f2);
            Vector3 lv2_pos = SphereRotationCalculator(f2, u2, horizonAngel_lv2, verticalAngel_lv2);
            Utils.deltaMove(transform, lv2_pos);
        } else {
            Utils.deltaMove(transform, lv1_pos);
        }
    }

    protected Vector3 SphereRotationCalculator(Vector3 f, Vector3 u, float horizonAngel, float verticalAngel) {
        Vector3 forward = Utils.copy(f);
        forward = Quaternion.AngleAxis(horizonAngel, u) * forward;
        Vector3 secondNormal = Vector3.Cross(forward, u);
        forward = Quaternion.AngleAxis(verticalAngel, secondNormal) * forward;
        return sun.position + forward * distance;
    }

    protected virtual void Transfer(float dt) {
        if (Utils.AbsDiff(horizonAngel, hAd) > MIN_ANGEL_DIFF) {
            horizonAngel = Mathf.Lerp(horizonAngel, hAd, 1 - Mathf.Exp(-speed * dt));
        }
        if (Utils.AbsDiff(verticalAngel, vAd) > MIN_ANGEL_DIFF) {
            verticalAngel = Mathf.Lerp(verticalAngel, vAd, 1 - Mathf.Exp(-speed * dt));
        }
        if (!enable_lv2) return;
        if (Utils.AbsDiff(horizonAngel_lv2, hAd_lv2) > MIN_ANGEL_DIFF) {
            horizonAngel_lv2 = Mathf.Lerp(horizonAngel_lv2, hAd_lv2, 1 - Mathf.Exp(-speed * dt));
        }
        if (Utils.AbsDiff(verticalAngel_lv2, vAd_lv2) > MIN_ANGEL_DIFF) {
            verticalAngel_lv2 = Mathf.Lerp(verticalAngel_lv2, vAd_lv2, 1 - Mathf.Exp(-speed * dt));
        }
    }
}
