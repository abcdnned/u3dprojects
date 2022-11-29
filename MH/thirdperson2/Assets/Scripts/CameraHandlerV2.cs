using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace TY {

public class CameraHandlerV2 : MonoBehaviour
{

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float _threshold = 0.01f;
    public GameObject CinemachineCameraTarget;
    public float CameraSpeed = 2.0f;
    private bool IsCurrentDeviceMouse = true;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    InputHandler inputHandler;

    private void Awake() {
        inputHandler = GetComponent<InputHandler>();
    }

    private void LateUpdate() {
        // CameraRotation();
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    public void CameraRotation()
    {
        float previousYaw = _cinemachineTargetYaw;
        // if there is an input and camera position is not fixed
        if (new Vector2(inputHandler.mouseX, inputHandler.mouseY).sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            _cinemachineTargetYaw += inputHandler.mouseX * deltaTimeMultiplier * CameraSpeed;
            _cinemachineTargetPitch += inputHandler.mouseY * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }
}

}