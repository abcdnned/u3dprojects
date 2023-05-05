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
        Vector3 f = Utils.forward(direction);
        Vector3 u = Utils.up(direction);
        // Debug.DrawRay(sun.position, f, Color.blue);
        // Debug.DrawRay(sun.position, u, Color.green);
        Vector3 forward = Utils.copy(f);
        forward = Quaternion.AngleAxis(horizonAngel, u) * forward;
        Vector3 secondNormal = Vector3.Cross(forward, u);
        forward = Quaternion.AngleAxis(verticalAngel, secondNormal) * forward;
        transform.position = sun.position + forward * distance;
        // sun.rotation = Quaternion.LookRotation(transform.position - sun.position);
        Transfer(Time.deltaTime);
    }

    protected virtual void Transfer(float dt) {
        if (Utils.AbsDiff(horizonAngel, hAd) > MIN_ANGEL_DIFF) {
            horizonAngel = Mathf.Lerp(horizonAngel, hAd, 1 - Mathf.Exp(-speed * dt));
        }
        if (Utils.AbsDiff(verticalAngel, vAd) > MIN_ANGEL_DIFF) {
            verticalAngel = Mathf.Lerp(verticalAngel, vAd, 1 - Mathf.Exp(-speed * dt));
        }
    }
}
