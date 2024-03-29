using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
    internal float initTime;
    private Vector3 lv2Normal = Vector3.zero;
    public HandLookIKController handLookIKController;
    public float normalizedTime = 0;
    public static float MIN_ANGEL_DIFF = 0.05f;
    public Vector3 offset;
    internal Vector3 cur_lv1_pos;






    // Start is called before the first frame update
    void Start()
    {
        hAd = horizonAngel;
        vAd = verticalAngel;

    }
    public void setAngel(float h, float v) {
        hAd = h;
        vAd = v;
        // Debug.Log(" vAd " + vAd);
        hAd_lv2 = 0;
        vAd_lv2 = 0;
        // enable_lv2 = false;
    }

    public void setAngel(float h, float v, float h2, float v2) {
        hAd = h;
        vAd = v;
        hAd_lv2 = h2;
        vAd_lv2 = v2;
    }

    // Update is called once per frame
    // protected void Update()
    internal void update()
    {
        if (!enable) return;
        Transfer(Time.deltaTime);
        Vector3 f = Utils.forward(direction);
        Vector3 u = Utils.up(direction);
        // Debug.DrawRay(sun.position, f, Color.blue);
        // Debug.DrawRay(sun.position, u, Color.green);
        Vector3 lv1_pos = SphereRotationCalculator(f, u, horizonAngel, verticalAngel);
        cur_lv1_pos = lv1_pos;
        if (enable_lv2) {
            // Calculate level 2
            if (handLookIKController != null && handLookIKController.getIKSequence(this) == 1) {
                // Debug.Log(" seq1 ");
                // Utils.deltaMove(transform, lv1_pos);
                Vector3 f2 = handLookIKController.getHandShoulderForward(lv1_pos);
                Vector3 u2 = handLookIKController.getHandShoulderNormal(lv1_pos);
                Vector3 lv2_pos = SphereRotationCalculator(f2, u2,
                                                           handLookIKController.getRealTimeHorizonAngel(lv1_pos),
                                                           0);
                // Utils.deltaMove(transform, lv2_pos);
                Utils.deltaMove(transform, lv2_pos);
            } else if (handLookIKController != null && handLookIKController.getIKSequence(this) == 2) {
                // Debug.Log(" seq2 ");
                Vector3 f2 = handLookIKController.getArmForward(lv1_pos);
                Vector3 u2 = handLookIKController.getArmNormal(lv1_pos);
                Vector3 lv2_pos = SphereRotationCalculator(f2, u2,
                                                           handLookIKController.getRealTimeVerticalAngel(lv1_pos),
                                                           0);
                Utils.deltaMove(transform, lv2_pos);
                // Debug.DrawLine(lv2_pos, handLookIKController.shoulder.transform.position, Color.green, normalizedTime * 2);
                // Debug.Log(" actuel dis " + Vector3.Distance(lv2_pos, handLookIKController.shoulder.position));
            } else {
                Vector3 f2 = (lv1_pos - sun.position).normalized;
                Vector3 u2 = Vector3.Cross(f, f2);
                Vector3 lv2_pos = SphereRotationCalculator(f2, u2, horizonAngel_lv2, verticalAngel_lv2);
                Utils.deltaMove(transform, lv2_pos);
            }
        } else {
            Utils.deltaMove(transform, lv1_pos);
        }
        lookToParent();
    }

    public Vector3 SphereLv1PositionCalculator(Vector3 sunpos, bool realTime = false, Transform dir = null) {
        Vector3 f = Utils.forward(dir ?? direction);
        Vector3 u = Utils.up(dir ?? direction);
        if (!realTime) {
            // Debug.Log(" hv " + hAd + " " + vAd);
            return SphereRotationCalculator(f, u, hAd, vAd, sunpos);
        }
        return SphereRotationCalculator(f, u, horizonAngel, verticalAngel, sunpos);
    }

    protected Vector3 SphereRotationCalculator(Vector3 f, Vector3 u, float horizonAngel, float verticalAngel, Vector3? sunpos = null) {
        Vector3 forward = Utils.copy(f);
        forward = Quaternion.AngleAxis(horizonAngel, u) * forward;
        Vector3 secondNormal = Vector3.Cross(forward, u);
        forward = Quaternion.AngleAxis(verticalAngel, secondNormal) * forward;
        return (sunpos ?? sun.position) + forward * distance;
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

    internal virtual void lookToParent() {
    }

    private void Update() {
        if (Application.isPlaying)
        {
        }
        else
        {
            hAd = horizonAngel;
            vAd = verticalAngel;
            hAd_lv2 = horizonAngel_lv2;
            vAd_lv2 = verticalAngel_lv2;
            update();
        } 
    }
}
