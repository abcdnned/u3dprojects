using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegControllerType2 : MonoBehaviour
{
    // The position and rotation we want to stay in range of
    [SerializeField] Transform homeTransform;
    [SerializeField] Transform pair;
    [SerializeField] LegControllerType2 pairComponent;
    [SerializeField] Transform toe;
    [SerializeField] Transform foot;
    [SerializeField] Rigidbody owner;
    // Stay within this distance of home
    [SerializeField] float halfStepDistance = 0.2f;
    // How long a step takes to complete
    [SerializeField] float moveDuration = 0.2f;
    [SerializeField] float dampingDuration = 0.1f;
    // [SerializeField] float stepOvershootFraction = 1f;
    // [SerializeField] float velocityStepDistance = 0.8f;
    [SerializeField] float preLiftDistance = 0.4f;
    [SerializeField] float postLiftDistance = 0.15f;
    [SerializeField] float preStartMovingDistance = 0.05f;
    [SerializeField] float swingDownDistance = 0.4f;

    [SerializeField] float walkFeetAngel = 50f;
    [SerializeField] float walkFeetAngel2 = 15f;
    [SerializeField] float feetBetween = 0.2f;


    [SerializeField] bool enable;
    [SerializeField] bool syncPair;

    [SerializeField] HumanIKController humanIKController;
    [SerializeField] WalkBalance walkBalance;
    [SerializeField]float stage1 = 0.2f;
    [SerializeField]float stage2 = 0.85f;
    [SerializeField]float isRightFoot = 1;
    [SerializeField] Transform hint;

    [SerializeField] CameraModule cameraModule;

    //state variable
    public bool Moving;

    public float normalizedTime = -1f;

    private ReadTrigger walkingStop = new ReadTrigger(false);
    private ReadTrigger transferStand = new ReadTrigger(false);
    private ReadTrigger lastStep = new ReadTrigger(false);
    // Is the leg moving?

    private void Awake() {
        cameraModule = owner.GetComponent<CameraModule>();
    }
    private void Update() {
        Vector3 d = transform.forward;
        Vector3 r = transform.right;
        r.y = 0;
        r.Normalize();
        d.y = 0;
        d.Normalize();
        Vector3 target = owner.transform.position + d * 1f;
        target += r * Mathf.Abs(feetBetween) * isRightFoot;
        target.y = 0.7f;
        hint.position = target;
    }
    IEnumerator MoveToHome(float pairProjectDis)
    {
        sync();
        Moving = true;

        float preMoveOvershootFix = 0.1f;

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
        // Vector3 overshootVector = (homeTransform.position - transform.position) * wantStepAtDistance * 20;


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
        // Vector3 endPoint = homeTransform.position;

        // We want to pass through the center point
        // Vector3 centerPoint = (startPoint + endPoint) / 2;
        // But also lift off, so we move it up by half the step distance (arbitrarily)
        // centerPoint += homeTransform.up * liftDistance;
                //calculage walk distance
                Vector3 plane = Vector3.up;
                // Vector3 forward = Vector3.ProjectOnPlane(cameraModule.Camera.transform.forward, plane).normalized;
                // forward.y = 0;
                // forward = forward.normalized;
                // Vector3 right = cameraModule.Camera.transform.right;
                // right.y = 0;
                // right = right.normalized;
                // Vector3 up = plane;
                Vector3 forward = owner.transform.forward;
                Vector3 right = owner.transform.right;
                Vector3 up = owner.transform.up;
        Vector3 footDir = cameraModule.Camera.transform.forward;
        footDir.y = 0;
        footDir.Normalize();
        Vector3 curDir = transform.forward;
        curDir.y = 0;
        curDir.Normalize();
        Vector3 pairDir = pair.transform.forward;
        pairDir.y = 0;
        pairDir.Normalize();
        bool followup = false;
        float deg = Vector3.Angle(curDir, footDir) * isRightFoot;
                Vector3 overshootVector = forward * (pairProjectDis + halfStepDistance);
        float pairDeg = Math.Abs(Vector3.Angle(pairDir, curDir));
        if (deg > 45) {
            Debug.Log(this.GetType().Name + " >45 ");
                overshootVector = footDir * (pairProjectDis + halfStepDistance / 2);
        } else if (deg < -15 && pairDeg < 15) {
            Debug.Log(this.GetType().Name + " <-15 && > 15 ");
                overshootVector = curDir * (pairProjectDis + halfStepDistance / 4 + preMoveOvershootFix);
                footDir = curDir;
                // followup = true;
        } else {
            Debug.Log(this.GetType().Name + " normalMode ");
        }
                overshootVector = Vector3.ProjectOnPlane(overshootVector, plane);
                // Vector3 centerStartPoint = Vector3.Project(transform.position, owner.transform.position);
                // Vector3 centerStartPoint = owner.transform.position;
                // Vector3 endPoint = centerStartPoint + overshootVector;
                // Vector3 endPoint = centerStartPoint + overshootVector + right * isRightFoot * feetBetween;
                Vector3 endPoint = transform.position + overshootVector;
                if (!followup) {
                    float curFeetBetween = calculateFeetBetween(owner.transform.position, forward, transform.position);
                    if (curFeetBetween < feetBetween) {
                        endPoint += right * Mathf.Abs(feetBetween - curFeetBetween) * isRightFoot;
                    } else if (curFeetBetween > feetBetween) {
                        endPoint -= right * Mathf.Abs(feetBetween - curFeetBetween) * isRightFoot;
                    }
                }
                Vector3 forward2 = (endPoint - transform.position).normalized;
                float walkDis = Vector3.ProjectOnPlane((endPoint - transform.position), plane).magnitude;
                Vector3 wp1 = transform.position + (up * preLiftDistance) + forward2 * (walkDis / 6);
                Vector3 wp2 = transform.position + (up * -swingDownDistance) + forward2 * (walkDis / 2);
                Vector3 wp3 = transform.position + (up * postLiftDistance) + forward2 * (5 * walkDis / 6);
                Vector3 wp4 = endPoint;

        bool forwardDampingStarted = false;
        bool walkPoseStarted = false;
        if (!walkPoseStarted) {
            walkBalance.startWalk();
            walkPoseStarted = true;
        }
        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;
            if (normalizedTime >= 0 && normalizedTime <= stage1) {
                float poc = Mathf.Lerp(0, 1, normalizedTime / stage1);
                transform.position =
                Vector3.Lerp(
                    startPoint,
                    wp1,
                    poc
                );
                Vector3 curAngel = transform.localEulerAngles;
                float walkAngel = Mathf.Lerp(0, walkFeetAngel, poc);
                transform.localEulerAngles = new Vector3(walkAngel, curAngel.y, curAngel.z);
            } else if (normalizedTime >= stage1 && normalizedTime <= stage2) {
                //start stage2
                float poc = Mathf.Lerp(0, 1, (normalizedTime - stage1) / (stage2 - stage1));
                transform.position =
                Vector3.Lerp(
                    Vector3.Lerp(wp1, wp2, poc),
                    Vector3.Lerp(wp2, wp3, poc),
                    poc
                );
                Vector3 curAngel = transform.localEulerAngles;
                float walkAngel = Mathf.Lerp(walkFeetAngel, walkFeetAngel2, poc);
                transform.localEulerAngles = new Vector3(walkAngel, curAngel.y, curAngel.z);
                syncFootDirection(footDir, normalizedTime);
            } else {
                if (!forwardDampingStarted) {
                    walkBalance.startForwardDamping(dampingDuration);
                    forwardDampingStarted = true;
                }
                float poc = Mathf.Lerp(0, 1, (normalizedTime - stage2) / (1 - stage2));
                transform.position =
                Vector3.Lerp(
                    wp3,
                    wp4,
                    poc
                );
                Vector3 curAngel = transform.localEulerAngles;
                float walkAngel = Mathf.Lerp(walkFeetAngel2, 0, poc);
                transform.localEulerAngles = new Vector3(walkAngel, curAngel.y, curAngel.z);
            }
            
            if (timeElapsed >= duration) {
                break;
            } 
            if (normalizedTime >= stage1 && walkingStop.read()) {
                transferStand.set();
                break;
            }
            yield return null;
        }
        while (timeElapsed < duration);

        normalizedTime = -1;
        Moving = false;
        if (transferStand.read()) {
            TryTransferStand();
        }
    }

    private void syncFootDirection(Vector3 footDir, float normalizedTime) {
        float poc = Mathf.Lerp(0, 1, (normalizedTime - stage1) / (stage2 - stage1));
        Quaternion tr = Quaternion.LookRotation(footDir);       
        transform.rotation = Quaternion.Slerp(transform.rotation, tr, poc);
    }

    private float calculateFeetBetween(Vector3 position1, Vector3 forward, Vector3 position2)
    {
        float d1 = Vector3.Dot(position1, forward);
        float d2 = Vector3.Dot(position2, forward);
        float a = Mathf.Abs(d1 - d2);
        float c = Vector3.Distance(position1, position2);
        float b = Mathf.Sqrt(c * c - a * a);
        return b;
    }

    private void sync()
    {
        if (syncPair) {
            halfStepDistance = pairComponent.halfStepDistance;
            moveDuration = pairComponent.moveDuration;
            preLiftDistance = pairComponent.preLiftDistance;
            postLiftDistance = pairComponent.postLiftDistance;
            preStartMovingDistance = pairComponent.preStartMovingDistance;
            swingDownDistance = pairComponent.swingDownDistance;
            walkFeetAngel = pairComponent.walkFeetAngel;
            walkFeetAngel2 = pairComponent.walkFeetAngel2;
            stage1 = pairComponent.stage1;
            stage2 = pairComponent.stage2;
            dampingDuration = pairComponent.dampingDuration;
            enable = pairComponent.enable;
            feetBetween = pairComponent.feetBetween;
            isRightFoot = -pairComponent.isRightFoot;
        }
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

        if (!checkPairStatusAndDecideToMove()) return;

        // Vector3 direction = cameraModule.Camera.transform.forward;
        // Vector3 direction = owner.gameObject.transform.forward;
        Vector3 direction = pair.transform.forward;
        float pairDirProject = Vector3.Dot(pair.transform.position, direction);
        float dirProject = Vector3.Dot(transform.position, direction);
        if (pairDirProject >= dirProject)
            // || Math.Abs(Vector3.Angle(transform.forward, direction)) > Math.Abs(Vector3.Angle(pair.transform.forward, direction)))
        {
            float pairProjectDis = pairDirProject - dirProject;
            // Start the step coroutine
            StartCoroutine(MoveToHome(pairProjectDis));
        }
    }

    private void TryTransferStand()
    {
        if (!enable) return;
        if (Moving) return;
        StartCoroutine(TransferStand());
    }

    

    public bool checkPairStatusAndDecideToMove() {
        if (pairComponent.Moving == false) return true;
        if (pairComponent.normalizedTime >= 1 - preStartMovingDistance) {
            return true;
        }
        return false;
    }

    public void handleEvent(string eventId) {
        if (Moving && String.Equals(eventId, HumanIKController.EVENT_STOP_WALKING)) {
            // Debug.Log(this.GetType().Name + " event trigger ");
            walkingStop.set();
        }
    }

    IEnumerator TransferStand()
    {
        sync();
        Moving = true;

        Quaternion endRot = homeTransform.rotation;

        float timeElapsed = 0;
        Vector3 wp1 = transform.position;
        Vector3 wp2 = homeTransform.position;
        Vector3 direction = owner.gameObject.transform.forward;
        float homeDot = Vector3.Dot(homeTransform.position, direction);
        float thisDot = Vector3.Dot(transform.position, direction);
        float pairDot = Vector3.Dot(pairComponent.transform.position, direction);
        if (pairDot > homeDot) {
            wp2 = transform.position + direction * (pairDot - thisDot);
        }
        else if (homeDot < thisDot) {
            wp2 = wp1;
        }
        float duration = (wp2 - wp1).magnitude * 0.5f;
        // Debug.Log(this.GetType().Name + " duration " + duration);
        wp2.y = 0.01f;
        float curAngel = transform.localEulerAngles.x;
        if (curAngel >= 180) curAngel -= 360;
        // Debug.Log(this.GetType().Name + " tranasfer stand ");
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / duration;
            // Debug.Log(this.GetType().Name + " time " + normalizedTime);
            transform.position =
            Vector3.Lerp(
                wp1,
                wp2,
                normalizedTime
            );
            float walkAngel = Mathf.Lerp(curAngel, 0, normalizedTime);
            transform.localEulerAngles = new Vector3(walkAngel,
                                                     transform.localEulerAngles.y,
                                                     transform.localEulerAngles.z);
            if (timeElapsed >= duration) {
                break;
            } 
            yield return null;
        }
        while (timeElapsed < duration);
        Moving = false;
    }
}
