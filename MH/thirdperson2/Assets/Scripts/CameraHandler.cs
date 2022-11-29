using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class CameraHandler : MonoBehaviour
{
    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransfrom;
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    private LayerMask ignoreLayers;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public static CameraHandler singleton;
    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 10f;

    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -55;
    public float maximumPivot = 55;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    private void Awake() {
        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1<<8|1<<9|1<<10);
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget(float delta) {
        Vector3 targetPosition = Vector3.SmoothDamp
        (myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targetPosition;
        HandleCameraCollisions(delta);
        // cameraTransform.position = targetPosition;
        // Debug.Log("targetTransform.position " + targetTransform.position);
        // Debug.Log("cameraTransform.position " + cameraTransform.position);
        // Debug.Log("follow " + targetPosition);
    }
    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput) {
        // Debug.Log("mouseY " + Input.GetAxis("Mouse Y"));
        // lookAngle += (0.3f * lookSpeed) / delta;
        // Debug.Log("moudeXInput " + mouseXInput);
        // mouseXInput = mouseXInput == 0 ? 0 : mouseXInput > 0 ? 0.2f : -0.2f;
        // lookAngle += (mouseXInput * lookSpeed) / delta;
        // pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        lookAngle += (mouseXInput * lookSpeed);
        pivotAngle -= (mouseYInput * pivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        // myTransform.rotation = targretRotation;

        rotation.x = pivotAngle;
        Quaternion targretRotation = Quaternion.Euler(rotation);


        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targretRotation, delta * 10);
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targretRotation = Quaternion.Euler(rotation);
        // cameraPivotTransfrom.localRotation = targretRotation;
        cameraPivotTransfrom.localRotation = Quaternion.Slerp(cameraPivotTransfrom.localRotation, targretRotation, delta * 10);
    }

    private void HandleCameraCollisions(float delta) {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransfrom.position;
        direction.Normalize();
        if (Physics.SphereCast
        (cameraPivotTransfrom.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition)
        , ignoreLayers)) {
            float dis = Vector3.Distance(cameraPivotTransfrom.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset) {
            targetPosition = -minimumCollisionOffset;
        }
        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta/0.2f);
        cameraTransform.localPosition = cameraTransformPosition;
    }

}


}
