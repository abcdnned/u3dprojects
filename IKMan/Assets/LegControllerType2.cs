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
    public int stepCount;

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
    [SerializeField] CameraFollower cameraFollower;
    [SerializeField] float maxFootBodyAngel = 30;
    [SerializeField] float detourFac = 3;

    [SerializeField] bool cgFoot = true;
    [SerializeField] float firstStepFac = 0.75f;
    [SerializeField] float bigFirstStepDampFac = 0.8f;
    [SerializeField] float firstStepDampFac = 0.7f;


    //state variable
    public bool Moving;
    public bool Recover;

    public string status;


    public float normalizedTime = -1f;

    private ReadTrigger walkingStop = new ReadTrigger(false);
    private ReadTrigger transferStand = new ReadTrigger(false);
    private ReadTrigger lastStep = new ReadTrigger(false);

    internal Timer walkingStopTime = new Timer();
    private Banner recentBanner;

    // internal float walkedDis_fw = 0;
    // internal float walkDis_fw = 0;

    // Is the leg moving?

    private void setPairCGFoot(bool v) {
        cgFoot = v;
        pairComponent.cgFoot = !v;
    }

    private void setCGFoot(bool v) {
        cgFoot = v;
    }

    private void Awake() {
        stepCount = 0;
    }

    public bool isStandGravity() {
        return transform.position.y < 0.1f;
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
            // Debug.Log(this.GetType().Name + " cross ");
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
            // Debug.Log(this.GetType().Name + " straight " + isRightFoot);
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
            // Debug.Log(this.GetType().Name + " rc * isRightFoot " + (rc * isRightFoot) + isRightFoot);
            detour = Utils.right(transform) * detourFac * feetBetween * isRightFoot;
            detour.y = 0;
        }
         endPoint.y = 0;
        return new Vector3[] { endPoint, footDir, detour};
    }

    private Vector3[] getEndPoint2(Transform target, float pairProjectDis, Vector3 plane) {
        Vector3 forward = Utils.forward(body.transform);
        Vector3 right = Utils.right(body.transform);
        Vector3 curDir = Utils.forward(transform);
        Vector3 pairDir = Utils.forward(pair);
        Vector3 pairRight = Utils.right(pair);
        float stepDistance = pairComponent.Moving ? halfStepDistance : (halfStepDistance * firstStepFac);
        Vector3 overshootVector = forward * (pairProjectDis + stepDistance);
        overshootVector = Vector3.ProjectOnPlane(overshootVector, plane);
        Vector3 endPoint = transform.position + overshootVector;
        int corss = crossDir(transform.position, pair.position, forward);
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
        endPoint.y = 0;
        return new Vector3[] { endPoint };
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
        Vector2 m = humanIKController.getMovement();
        Vector3 targetDir = Utils.forward(cameraModule.Camera.transform) * m.y + Utils.right(cameraModule.Camera.transform) * m.x;
        // Debug.Log(this.GetType().Name + " td " + targetDir);
        cameraFollower.setDir(targetDir);
        stepCount++;

        Vector3 startPoint = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = homeTransform.rotation;

        float timeElapsed = 0;
        float duration = moveDuration;
        Vector3 plane = Vector3.up;
        Vector3 forward = Utils.forward(body.transform);
        Vector3 right = Utils.right(body.transform);
        Vector3 up = owner.transform.up;
        Vector3[] Points = getEndPoint2(owner.transform, pairProjectDis, plane);
        Vector3 endPoint = Points[0];
        // Vector3 footDir = Points[1];
        // Vector3 detour = Points[2];

        Vector3 forward2 = (endPoint - transform.position).normalized;
        float walkDis = Vector3.ProjectOnPlane((endPoint - transform.position), plane).magnitude;
        if (!pairComponent.Moving) {
            if (walkDis > halfStepDistance * 1.15f) {
                walkBalance.setDampDist(bigFirstStepDampFac);
            } else {
                walkBalance.setDampDist(firstStepDampFac);
            }
        }
        // walkDis_fw = Vector3.Dot(endPoint - transform.position, forward);
        // walkedDis_fw = 0;
        Vector3 wp1 = transform.position + (up * preLiftDistance) + forward2 * (walkDis / 6);
        Vector3 wp2 = transform.position + (up * -swingDownDistance) + forward2 * (walkDis / 2);
        // if (detour.magnitude > 0) {
        //     // Debug.Log(this.GetType().Name + " take detour ");
        //     wp2 += detour;
        // }
        Vector3 wp3 = transform.position + (up * postLiftDistance) + forward2 * (5 * walkDis / 6);
        Vector3 wp4 = endPoint;

        bool forwardDampingStarted = false;
        // bool stage2Started = false;
        bool init1 = false;
        bool init2 = false;
        bool init3 = false;
        Vector3 lastPosition = Vector3.zero;
        bool walkPoseStarted = false;
        // bool centerForward = false;
        do
        {
            Vector3 forward3 = Utils.forward(body.transform);
            Vector3 right3 = Utils.right(body.transform);
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;
            if (normalizedTime > preStartMovingDistance && !walkPoseStarted) {
                walkBalance.startWalk(moveDuration - (moveDuration * preStartMovingDistance));
                walkPoseStarted = true;
            }
            // if (normalizedTime > 0.1 && !centerForward) {
            //     walkBalance.centerForwardIncrease();
            //     centerForward = true;
            // }
            if (normalizedTime >= 0 && normalizedTime <= stage1) {
                if (!init1) {
                    lastPosition = startPoint;
                    init1 = true;
                    float handDuration = moveDuration - (moveDuration * preStartMovingDistance);
                    humanIKController.leftHand.TryMove(handDuration, isRightFoot);
                    humanIKController.rightHand.TryMove(handDuration, isRightFoot);
                }
                float poc = Mathf.Lerp(0, 1, normalizedTime / stage1);
                Vector3 targetPosition =
                Vector3.Lerp(
                    startPoint,
                    wp1,
                    poc
                );
                Vector3 delta = targetPosition - lastPosition;
                transform.position += forward3 * Vector3.Dot(delta, forward)
                                      + right3 * Vector3.Dot(delta, right)
                                      + plane * Vector3.Dot(delta, plane);
                // walkedDis_fw += Vector3.Dot(delta, forward);
                lastPosition = targetPosition;
                Vector3 curAngel = transform.localEulerAngles;
                float walkAngel = Mathf.Lerp(0, walkFeetAngel, poc);
                transform.localEulerAngles = new Vector3(walkAngel, curAngel.y, curAngel.z);
            } else if (normalizedTime >= stage1 && normalizedTime <= stage2) {
                if (!init2) {
                    lastPosition = wp1;
                    init2 = true;
                }
                float poc = Mathf.Lerp(0, 1, (normalizedTime - stage1) / (stage2 - stage1));
                // Vector3.Lerp(wp1, wp3, poc);
                Vector3 targetPosition =
                Vector3.Lerp(
                    Vector3.Lerp(wp1, wp2, poc),
                    Vector3.Lerp(wp2, wp3, poc),
                    poc
                );
                Vector3 delta = targetPosition - lastPosition;
                transform.position += forward3 * Vector3.Dot(delta, forward)
                                      + right3 * Vector3.Dot(delta, right)
                                      + plane * Vector3.Dot(delta, plane);
                // walkedDis_fw += Vector3.Dot(delta, forward);
                lastPosition = targetPosition;
                Vector3 curAngel = transform.localEulerAngles;
                float walkAngel = Mathf.Lerp(walkFeetAngel, walkFeetAngel2, poc);
                transform.localEulerAngles = new Vector3(walkAngel, curAngel.y, curAngel.z);
                // syncFootDirection(footDir, normalizedTime);
            } else {
                if (!init3) {
                    lastPosition = wp3;
                    init3 = true;
                }
                float poc = Mathf.Lerp(0, 1, (normalizedTime - stage2) / (1 - stage2));
                Vector3 targetPosition =
                Vector3.Lerp(
                    wp3,
                    wp4,
                    poc
                );
                Vector3 delta = targetPosition - lastPosition;
                transform.position += forward3 * Vector3.Dot(delta, forward)
                                      + right3 * Vector3.Dot(delta, right)
                                      + plane * Vector3.Dot(delta, plane);
                // walkedDis_fw += Vector3.Dot(delta, forward);
                lastPosition = targetPosition;
                Vector3 curAngel = transform.localEulerAngles;
                float walkAngel = Mathf.Lerp(walkFeetAngel2, 0, poc);
                transform.localEulerAngles = new Vector3(walkAngel, curAngel.y, curAngel.z);
            }

            syncPairFootDir();
            syncFootBodyRotation();
            
            if (timeElapsed >= duration) {
                if (!forwardDampingStarted) {
                    // walkBalance.startForwardDamping(dampingDuration);
                    walkBalance.setDampDist(1f);
                    forwardDampingStarted = true;
                }
                break;
            } 
            walkingStopTime.countDown(Time.deltaTime);
            if (normalizedTime >= stage1 && walkingStopTime.check()) {
                // Debug.Log(this.GetType().Name + " transferStand.set() ");
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
            Debug.Log(this.GetType().Name + " transferStand ");
        } else {
            Debug.Log(this.GetType().Name + " direct finish ");
            notifyBanner();
        }
    }

    private void syncFootBodyRotation() {
        Vector3 bf = Utils.forward(body.transform);
        transform.rotation = Utils.dampTrack(transform, bf, 5);
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
            firstStepFac = pairComponent.firstStepFac;
            bigFirstStepDampFac = pairComponent.bigFirstStepDampFac;
            firstStepDampFac = pairComponent.firstStepDampFac;
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
        Vector3 direction = Utils.forward(body.transform);
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
                if (startAngel >= 90 && startAngel <= 180) {
                    startRight = true;
                } else if (startAngel >= 0 && startAngel < 90) {
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

    public void handleEvent(String evtId, Banner banner) {
        recentBanner = banner;
        banner.addSub(this);
        handleEvent(evtId);
    }

    private void notifyBanner() {
        if (recentBanner != null && recentBanner.available()) {
            Debug.Log(this.GetType().Name + " notifyBanner " + isRightFoot);
            recentBanner.Finish();
            recentBanner = null;
        }
    }

    public void handleEvent(string eventId) {
        if (Moving && String.Equals(eventId, HumanIKController.EVENT_STOP_WALKING)) {
            // Debug.Log(this.GetType().Name + " event trigger ");
            walkingStopTime.setTimer(0.1f);
        }
        if (String.Equals(eventId, HumanIKController.EVENT_KEEP_WALKING)) {
            walkingStopTime.reset();
            pairComponent.walkingStopTime.reset();
        }
    }

    IEnumerator TransferStand()
    {
        sync();
        Moving = true;
        Recover = true;

        Quaternion endRot = homeTransform.rotation;

        float timeElapsed = 0;
        Vector3 wp1 = transform.position;
        // Vector3 wp2 = homeTransform.position;
        // Vector3 direction = owner.gameObject.transform.forward;
        Vector3 direction = Utils.forward(body.transform);
        Vector3 right = Utils.right(body.transform);
        Vector3 plane = Vector3.up;
        // Vector3 wp2 = homeTransform.position;

        // Vector3 wp2 = pair.position + Utils.right(owner.transform) * 2 * feetBetween * isRightFoot;
        Vector3 wp2 = pair.position + right * 2 * feetBetween * isRightFoot;
        // float homeDot = Vector3.Dot(homeTransform.position, direction);
        float thisDot = Vector3.Dot(transform.position, direction);
        float pairDot = Vector3.Dot(pairComponent.transform.position, direction);
        if (pairDot > thisDot) {
            // wp2 = transform.position + direction * (pairDot - thisDot);
        } else {
            wp2 = wp1;
        }
        // else if (homeDot < thisDot) {
        //     wp2 = wp1;
        // }
        // wp2 = wp1;
        // float homeDot = Vector3.Dot(homeTransform.position, direction);
        // float thisDot = Vector3.Dot(transform.position, direction);
        // float pairDot = Vector3.Dot(pairComponent.transform.position, direction);
        // if (pairDot > homeDot) {
        //     wp2 = transform.position + direction * (pairDot - thisDot);
        // }
        // else if (homeDot < thisDot) {
        //     wp2 = wp1;
        // }
        float duration = (wp2 - wp1).magnitude * 0.5f;
        // Debug.Log(this.GetType().Name + " duration " + duration);
        wp2.y = 0.01f;
        float curAngel = transform.localEulerAngles.x;
        if (curAngel >= 180) curAngel -= 360;
        // Debug.Log(this.GetType().Name + " tranasfer stand ");
        Vector3 lastPosition = Vector3.zero;
        bool init = false;
        do
        {
            if (!init) {
                lastPosition = wp1;
                init = true;
            }
            Vector3 forward3 = Utils.forward(body.transform);
            Vector3 right3 = Utils.right(body.transform);
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / duration;
            // Debug.Log(this.GetType().Name + " time " + normalizedTime);
            // transform.position =
            Vector3 targetPosition =
            Vector3.Lerp(
                wp1,
                wp2,
                normalizedTime
            );
            Vector3 delta = targetPosition - lastPosition;
            transform.position += forward3 * Vector3.Dot(delta, direction)
                                    + right3 * Vector3.Dot(delta, right)
                                    + plane * Vector3.Dot(delta, plane);
            lastPosition = targetPosition;

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
        Recover = false;
        Debug.Log(this.GetType().Name + " transfer notify ");
        notifyBanner();
    }
}
