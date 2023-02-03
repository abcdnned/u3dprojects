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
    // [SerializeField] Rigidbody owner;
    [SerializeField] WalkPointer owner;
    [SerializeField] Rigidbody body;
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

    [SerializeField]float preMoveOvershootFix = 0.3f;
    [SerializeField] Transform hint;
    [SerializeField] CameraModule cameraModule;
    [SerializeField] float maxFootBodyAngel = 30;
    [SerializeField] float detourFac = 3;

    //state variable
    public bool Moving;

    public float normalizedTime = -1f;

    private ReadTrigger walkingStop = new ReadTrigger(false);
    private ReadTrigger transferStand = new ReadTrigger(false);
    private ReadTrigger lastStep = new ReadTrigger(false);

    // Is the leg moving?

    private void Awake() {
        cameraModule = owner.cameraModule;
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

    private Vector3[] getEndPoint(Transform target, float pairProjectDis, Vector3 plane) {
        Vector3 forward = owner.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 footDir = forward;
        Vector3 detour = Vector3.zero;
        Vector3 right = owner.transform.right;
        right.y = 0;
        right.Normalize();
        Vector3 curDir = transform.forward;
        curDir.y = 0;
        curDir.Normalize();
        Vector3 pairDir = pair.transform.forward;
        pairDir.y = 0;
        pairDir.Normalize();
        Vector3 pairRight = pair.transform.right;
        pairRight.y = 0;
        pairRight.Normalize();
        bool followup = false;
        bool frontFollowup = false;
        float deg = Vector3.SignedAngle(curDir, forward, Vector3.up) * isRightFoot;
        Vector3 overshootVector = forward * (pairProjectDis + halfStepDistance);
        // Vector3 overshootVector = forward * halfStepDistance;
        float pairDeg = Math.Abs(Vector3.Angle(pairDir, curDir));
        int corss = crossDir(transform.position, pair.position, forward);
        if (corss * isRightFoot < 0) {
            Debug.Log(this.GetType().Name + " cross ");
        }
        if (deg > 20) {
                // if (deg < 30) {
                //     overshootVector = Utils.halfTowards(pairDir, forward, 0) * halfStepDistance * 0.90f;
                // } else {
                //     overshootVector = Utils.customTowards(pairDir, forward, 0.1f, 0) * halfStepDistance * 0.90f;
                // }
                overshootVector = forward * halfStepDistance * 0.90f;
                frontFollowup = true;
        } else if ((deg < -15 && pairDeg < 15) || (corss  * isRightFoot < 0)) {
                overshootVector =  Utils.halfTowards(pairDir, forward, 0) * halfStepDistance * 0.80f;
                Vector3 footHalfDir = Utils.customTowards(curDir, footDir, 0.75f, 0);
                footDir = footHalfDir;
                followup = true;
        } else {
        }
        overshootVector = Vector3.ProjectOnPlane(overshootVector, plane);
        Vector3 endPoint = transform.position + overshootVector;
        if (followup) {
            endPoint = pair.position + overshootVector;
        } else if (frontFollowup) {
            endPoint = pair.position + overshootVector;
        }
        if (!followup && !frontFollowup) {
            Debug.Log(this.GetType().Name + " straight " + isRightFoot);
            float curFeetBetween = calculateFeetBetween(endPoint, forward, pair.position) / 2;
            if (curFeetBetween < feetBetween) {
                if (corss * isRightFoot < 0) {
                    endPoint += right * feetBetween * isRightFoot * 3;
                } else {
                    endPoint += right * Mathf.Abs(feetBetween - curFeetBetween) * isRightFoot * 3;
                }
            } else if (curFeetBetween > feetBetween) {
                if (corss * isRightFoot < 0) {
                    endPoint += right * feetBetween * isRightFoot * 3;
                } else {
                    endPoint -= right * Mathf.Abs(feetBetween - curFeetBetween) * isRightFoot * 3;
                }
            }
        } else {
            endPoint += pairRight * 2 * feetBetween * isRightFoot;
        }
        //check detour
        int rc = routConflict(endPoint - transform.position, pair.position - transform.position);
        bool takeDetour = rc * isRightFoot > 0;
        if (corss * isRightFoot < 0 || takeDetour) {
            Debug.Log(this.GetType().Name + " rc * isRightFoot " + (rc * isRightFoot) + isRightFoot);
            detour = Utils.right(transform) * detourFac * feetBetween * isRightFoot;
            detour.y = 0;
        }
         endPoint.y = 0;
        return new Vector3[] { endPoint, footDir, detour};
    }

    private int routConflict(Vector3 route, Vector3 pairFoot) {
        Vector3 cross = Vector3.Cross(route, pairFoot);
        if (cross.y > 0) {
            return 1;
        } else if (cross.y < 0) {
            return -1;
        }
        return 0;
    }

    private int crossDir(Vector3 d1, Vector3 d2, Vector3 forward) {
        Vector3 center = (d1 + d2) / 2;
        Vector3 cross = Vector3.Cross(forward, d1 - center);
        if (cross.y > 0) {
            return 1;
        } else if (cross.y < 0) {
            return -1;
        }
        return 0;
    }
    IEnumerator MoveToHome(float pairProjectDis)
    {
        sync();
        Moving = true;

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = homeTransform.rotation;

        float timeElapsed = 0;
        float duration = moveDuration;
        Vector3 plane = Vector3.up;
        Vector3 forward = owner.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 up = owner.transform.up;
        Vector3[] Points = getEndPoint(owner.transform, pairProjectDis, plane);
        Vector3 endPoint = Points[0];
        Vector3 footDir = Points[1];
        Vector3 detour = Points[2];
        Vector3 forward2 = (endPoint - transform.position).normalized;
        float walkDis = Vector3.ProjectOnPlane((endPoint - transform.position), plane).magnitude;
        Vector3 wp1 = transform.position + (up * preLiftDistance) + forward2 * (walkDis / 6);
        Vector3 wp2 = transform.position + (up * -swingDownDistance) + forward2 * (walkDis / 2);
        if (detour.magnitude > 0) {
            Debug.Log(this.GetType().Name + " take detour ");
            wp2 += detour;
        }
        Vector3 wp3 = transform.position + (up * postLiftDistance) + forward2 * (5 * walkDis / 6);
        Vector3 wp4 = endPoint;

        bool forwardDampingStarted = false;
        bool walkPoseStarted = false;
        // bool stage2Started = false;
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
                // if (!stage2Started) {
                //     endPoint = getEndPoint(owner.transform, pairProjectDis, plane, footDir);
                //     Debug.Log(this.GetType().Name + "reclate endPoint " + endPoint);
                //     forward2 = (endPoint - transform.position).normalized;
                //     walkDis = Vector3.ProjectOnPlane((endPoint - transform.position), plane).magnitude;
                //     wp2 = transform.position + (up * -swingDownDistance) + forward2 * (walkDis / 2);
                //     wp3 = transform.position + (up * postLiftDistance) + forward2 * (5 * walkDis / 6);
                //     wp4 = endPoint;
                //     stage2Started = true;
                // }
                float poc = Mathf.Lerp(0, 1, (normalizedTime - stage1) / (stage2 - stage1));
                // poc = EasingFunction.EaseInOutCubic(0, 1, poc);
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

            syncPairFootDir();
            
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

    private void syncPairFootDir()
    {
        if (!pairComponent.Moving) {
            Vector3 bf = Utils.forward(body.transform);
            Vector3 tf = Utils.forward(pair);
            float deg = Vector3.Angle(bf, tf);
            if (deg > maxFootBodyAngel) {
                pair.rotation = Utils.dampTrack(pair, bf, 5);
            }
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

    private float calculateFeetDistance(Vector3 position1, Vector3 position2)
    {
        float c = Vector3.Distance(position1, position2);
        return c / 2;
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
            preMoveOvershootFix = pairComponent.preMoveOvershootFix;
            maxFootBodyAngel = pairComponent.maxFootBodyAngel;
            detourFac = pairComponent.detourFac;
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
        Vector3 direction = owner.gameObject.transform.forward;
        // Vector3 direction = pair.transform.forward;
        // int walkStright = 0;
        Vector3 curDir = transform.forward;
        curDir.y = 0;
        curDir.Normalize();
        Vector3 pairDir = pair.transform.forward;
        pairDir.y = 0;
        pairDir.Normalize();
        float angel = Vector3.Angle(curDir, direction);
        Vector3 twoFootForward = Vector3.RotateTowards(curDir, pairDir, Mathf.Deg2Rad * (angel / 2), 0);
        // float pairAngel = Vector3.Angle(pairDir, direction);
        // float avgAngel = (angel + pairAngel) / 2;
        // if (avgAngel < 10) {
        //     walkStright = 1;
        // } else if (avgAngel < 90) {
        //     walkStright = 0;
        // }
        float tfpairProject = Vector3.Dot(pair.transform.position, twoFootForward);
        float tfProject = Vector3.Dot(transform.position, twoFootForward);
        float tfProjectDis = tfpairProject - tfProject;
        
        float pairDirProject = Vector3.Dot(pair.transform.position, direction);
        float dirProject = Vector3.Dot(transform.position, direction);
        float pairProjectDis = pairDirProject - dirProject;
        bool twoFootAlign = false;
        if (pairComponent.Moving) {
            // Debug.Log(this.GetType().Name + " continue moving ");
            StartCoroutine(MoveToHome(pairProjectDis));
        } else {
            // Debug.Log(this.GetType().Name + " tfProjectDis " + tfProjectDis);
            if (Math.Abs(tfProjectDis) < 0.2) {
                twoFootAlign = true;
            } else {
                twoFootAlign  = false;
            }
            if (twoFootAlign) {
                Vector3 standVector =
                        isRightFoot > 0
                        ? (pair.transform.position - transform.position)
                        : (transform.position - pair.transform.position);
                float startAngel = Vector3.SignedAngle(standVector, direction, Vector3.up);
                bool startRight = false;
                if (startAngel >= 90 && startAngel < 180) {
                    startRight = true;
                } else if (startAngel > 0 && startAngel < 90) {
                    startRight = false;
                }
                // Debug.Log(this.GetType().Name + " startRight " + startRight);
                // Debug.Log(this.GetType().Name + " isRightFoot " + isRightFoot);
                if (!pairComponent.Moving
                    && ((startRight && isRightFoot > 0)
                    || (!startRight && isRightFoot < 0)))
                {
                    // Start the step coroutine
                    // Debug.Log(this.GetType().Name + " start ");
                    StartCoroutine(MoveToHome(pairProjectDis));
                }
            } else {
                if (pairDirProject >= dirProject) {
                    StartCoroutine(MoveToHome(pairProjectDis));
                }
            }
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
