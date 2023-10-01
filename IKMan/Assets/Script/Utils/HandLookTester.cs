using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent (typeof (HandDelayLooker))]
public class HandLookTester : MonoBehaviour {
    public float h1;
    public float v1;
    public float h2;
    public float v2;
    public float interval = 5;

    public float duration = 2f;
    private float initTime;
    private int previousIndex;
    private HandDelayLooker handDelayLooker;

    private void Start() {
        handDelayLooker = GetComponent<HandDelayLooker>();
        initTime = Time.time;
    }

    private void Update() {
        float t = Time.time - initTime;
        int index = Mathf.CeilToInt(t / interval);
        if (previousIndex != index) {
            previousIndex = index;
            int mod = index % 2;
            if (mod == 0) {
                handDelayLooker.hAd = h1;
                handDelayLooker.vAd = v1;
                handDelayLooker.setDuration(duration);
            } else {
                handDelayLooker.hAd = h2;
                handDelayLooker.vAd = v2;
                handDelayLooker.setDuration(duration);
            }
        }
        handDelayLooker.update();
    }

}