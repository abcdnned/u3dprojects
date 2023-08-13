﻿using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author: Sergio Abreu García | https://sergioabreu.me

/// <summary> Tells the ActiveRagdoll what it should do. Input can be external (like the
/// one from the player or from another script) and internal (kind of like sensors, such as
/// detecting if it's on floor). </summary>
public class InputModule : MonoBehaviour {
    // ---------- EXTERNAL INPUT ----------

    public delegate void onMoveDelegate(Vector2 movement);
    public onMoveDelegate OnMoveDelegates { get; set; }

    public delegate void onButtonRDelegate();
    public onButtonRDelegate OnButtonRDelegates { get; set; }
    public void OnMove(InputValue value) {
        OnMoveDelegates?.Invoke(value.Get<Vector2>());
    }

    public void OnButtonR() {
        OnButtonRDelegates?.Invoke();
    }

    public delegate void onLeftArmDelegate(float armWeight);
    public onLeftArmDelegate OnLeftArmDelegates { get; set; }
    public void OnLeftArm(InputValue value) {
        OnLeftArmDelegates?.Invoke(value.Get<float>());
    }

    public delegate void onRightArmDelegate(float armWeight);
    public onRightArmDelegate OnRightArmDelegates { get; set; }
    public void OnRightArm(InputValue value) {
        OnRightArmDelegates?.Invoke(value.Get<float>());
    }

    void Start() {
    }

    void Update() {
    }
}