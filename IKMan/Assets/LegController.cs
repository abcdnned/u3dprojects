using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    // The position and rotation we want to stay in range of
    [SerializeField] Transform homeTransform;
    [SerializeField] Transform pair;
    [SerializeField] Transform toe;
    // Stay within this distance of home
    [SerializeField] float halfStepDistance = 0.2f;
    // How long a step takes to complete
    [SerializeField] float moveDuration = 0.2f;
    // [SerializeField] float stepOvershootFraction = 1f;
    // [SerializeField] float velocityStepDistance = 0.8f;
    [SerializeField] float liftDistance = 0.2f;

    [SerializeField] float walkFeetAngel = 60f;

    [SerializeField] Rigidbody owner;
    [SerializeField] bool enable;

     public bool Moving;
    // Is the leg moving?
    IEnumerator MoveToHome(float pairProjectDis)
    {
        Moving = true;

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;

        Quaternion endRot = homeTransform.rotation;

        // Directional vector from the foot to the home position
        // Vector3 towardHome = (homeTransform.position - transform.position);
        // Total distnace to overshoot by   
        // float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        // Vector3 overshootVector = towardHome * overshootDistance;
        // Vector3 overshootVector = towardHome;
        // Since we don't ground the point in this simplified implementation,
        // we restrict the overshoot vector to be level with the ground
        // by projecting it on the world XZ plane.
        Vector3 direction = owner.gameObject.transform.forward;
        // Vector3 overshootVector = (homeTransform.position - transform.position) * wantStepAtDistance * 20;
        Vector3 overshootVector = direction * (pairProjectDis + halfStepDistance);
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);


        // float legAngle = calculateLegAngle(Vector3.up);
        // bool firstStepHalf = false;
        // if (legAngle > 10 || legAngle < -10) {
        //     firstStepHalf = true;
        // }

        float timeElapsed = 0;
        // float duration = Mathf.Min((wantStepAtDistance * 3) / (owner.velocity.magnitude * 2), moveDuration);
        float duration = moveDuration;
        // Apply the overshoot
        // Vector3 endPoint = homeTransform.position + overshootVector;
        Vector3 endPoint = transform.position + overshootVector;
        // Vector3 endPoint = homeTransform.position;

        // We want to pass through the center point
        Vector3 centerPoint = (startPoint + endPoint) / 2;
        // But also lift off, so we move it up by half the step distance (arbitrarily)
        centerPoint += homeTransform.up * liftDistance;
        do
        {

            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / duration;
            // normalizedTime = EasingFunction.EaseInOutCubic(0, 1, normalizedTime);

            // Quadratic bezier curve
            transform.position =
            Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime
            );
            Vector3 curAngel = transform.localEulerAngles;
            float walkAngel = IKComponentRotation.mappingFootRotation(normalizedTime, walkFeetAngel);
            transform.localEulerAngles = new Vector3(walkAngel, curAngel.y, curAngel.z);
            
            if (timeElapsed >= duration) {
                break;
            } 
            // transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);
            yield return null;
        }
        while (timeElapsed < duration);

        Moving = false;
    }

    private float calculateLegAngle(Vector3 plan)
    {
        Vector3 legBetween = Vector3.ProjectOnPlane(pair.transform.position - transform.position, plan);
        Vector3 forward = Vector3.ProjectOnPlane(owner.gameObject.transform.forward, plan);
        float AngleRad = Mathf.Atan2(legBetween.y - forward.y, legBetween.x - forward.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        return AngleDeg;
    }

    public void TryMove()
    {
        if (!enable) return;
        // If we are already moving, don't start another move
        if (Moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        // If we are too far off in position or rotation
        Vector3 direction = owner.gameObject.transform.forward;
        float pairDirProject = Vector3.Dot(pair.transform.position, direction);
        float dirProject = Vector3.Dot(transform.position, direction);
        if (pairDirProject >= dirProject)
        {
            float pairProjectDis = pairDirProject - dirProject;
            // Start the step coroutine
            StartCoroutine(MoveToHome(pairProjectDis));
        }
    }
}
