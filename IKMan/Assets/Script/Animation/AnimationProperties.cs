using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationProperties : MonoBehaviour {

    [Header("--- Run ---")]
    public float runHalfDuration = .5f;
    public float snapTime = 0.1f;
    public float snapBlendDis = 0.1f;
    public float runUpHeight = 0.75f;
    public float runDownHeigth = 0.6f;
    public float runSpin3Angel = 30;

    [Header("--- Animation Speed ---")]
    public float transferSpeedSmall = .5f;
    public float hipTrackCameraSpeed = 5f;

    [Header("--- idle ---")]

    public float standHeight = 0.7f;

}