using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkBalance : TargetController
{
    public Transform left;
    public Transform right;
    public LegControllerType2 leftLeg;
    public LegControllerType2 rightLeg;

    public HandController leftHand;

    public HandController rightHand;

    public Transform target;

    [SerializeField] Transform direction;

    [Header("--- WALKING ---")]
    [SerializeField] float overshoot = 0.2f;
    [SerializeField] float dampSpeed = 2f;
    [SerializeField] float returnCenterSpeed = 6f;

    internal float finalDampSpeed = 2f;
    [SerializeField] float dampDis = 0.1f;
    // [SerializeField] float afterDampingSpeed = 1f;

    // [SerializeField] float walkPoseDuration = 0.2f;

    [SerializeField] float walkPoseLift = 0.1f;
    [SerializeField] bool walkPosing = false;



    internal Vector3 lastDampStart;

    internal float dampSum;

    internal int dampCount;

    public Vector3 dampDist = new Vector3(0,0,0);

    [Header("--- BATTLE ---")]
    public float battleIdleAngelOffset = 45;
    public float battleIdleTransferDuration = 1;
    public float battleIdleHipH = 0.2f;
    public float idleHipH = 0.4f;
    public float hipHeightOffset = 0.7f;
    public float hipBattleSpeed = 4f;
    public float battleLegDistance = 0.4f;
    [Header("--- GENERAL ---")]
    public float trackSpeed = 5;
    public float defaultLegDistance = 0.65f;
    public float defaultTransfer = 0;
    public float expectLegDistance = 0.65f;
    public float expectTransfer = 0;
    public float expectTransferSpeed = 5;
    public Transform cam;
    private Vector3 transferDir;
    public float airRayCastDistance = 2f;
    public LayerMask airRayCastIgnoreLayer;
    public static float DOWN_RAY_OFFSET = 10;

    internal void ReturnToCenter() {
        Vector3 center = (left.transform.position + right.transform.position) / 2;
        Vector3 dist = new Vector3(center.x, target.position.y, center.z);
        dist += Utils.forwardFlat(transform) * overshoot;
        dist = new Vector3(dist.x, target.position.y, dist.z);
        hic.logHomeOffset();
        target.position = Vector3.Lerp(
                                    target.position,
                                    dist,
                                    1 - Mathf.Exp(-returnCenterSpeed * Time.deltaTime)
                                );
        hic.postUpdateTowHandPosition();
    }

    internal void update()
    {
        if ((leftLeg.move != null && leftLeg.move.IsLegMoving() && !leftLeg.Recover)
             || (rightLeg.move != null && rightLeg.move.IsLegMoving() && !rightLeg.Recover)) {
            // Debug.Log(this.GetType().Name + " walking ");
            moveManager.ChangeMove(MoveNameConstants.HipDamp);
            keepBalanceWhenWalking();
        } else if (hic.currentStatus.legIdleChecker()) {
            moveManager.ChangeMove(MoveNameConstants.HipIdle);
            // ReturnToCenter();
        }
        // Move.
        if (move != null) {
            // && move.name != MoveNameConstants.HipIdle
            // && move.name != MoveNameConstants.HipDamp) {
            move = move.move(Time.deltaTime);
        }
        // Update rotation based on camera.
        // if (hic.currentStatus.getName() == LocomotionState.NAME
        //     && hic.currentStatus.cs.name == LocomotionState.STATE_MOVE) {
        //     Vector2 m = hic.getMovement();
        //     Vector3 dir = Utils.forwardFlat(cam) * m.y + Utils.right(cam) * m.x;
        //     updateTransferDirection(dir);
        //     transfer(0);
        // }
        // else if (
        //     (humanIKController.currentStatus.getName() == IdleStatus.NAME
        //     && humanIKController.currentStatus.cs.name == IdleStatus.STATE_TOBATTLEIDLE) ||
        //     (humanIKController.currentStatus.getName() == BattleIdleState.NAME
        //     && humanIKController.currentStatus.cs.name == BattleIdleState.STATE_BATTLE)) {
            // transfer(battleIdleAngelOffset);
        // }
    }

    protected override void initMove() {
        moveManager.addMove(new HipIdleMove());
        moveManager.addMove(new HipDampMove());
        moveManager.addMove(new Hip2BattleIdleMove());
        moveManager.addMove(new HipBattleIdleMove());
        moveManager.addMove(new Hip2IdleMove());
        moveManager.addMove(new HipHeightChangeMove());
        moveManager.addMove(new HipRunMove());
        moveManager.addMove(new HipAirMove());
        moveManager.ChangeMove(MoveNameConstants.HipIdle);
    }
    
    internal void adjustLegDistance() {
        ActionStateMachine asm = hic.currentStatus;
        if (asm.cs.name == IdleStatus.STATE_TOBATTLEIDLE
            || (asm.getName() == BattleIdleState.NAME && asm.cs.name != BattleIdleState.STATE_TO_IDLE)) {
            // expectLegDistance = battleLegDistance;
        } else {
            expectLegDistance = defaultLegDistance;
        }
    }

    internal Vector3 getDynamicHeight(Vector3 p1, Vector3 p2, float legDistance) {
        float x = (p1.x + p2.x) / 2;
        float z = (p1.z + p2.z) / 2;
        if (legDistance <= 0) {
            return new Vector3(x, defaultLegDistance, z);
        }
        float a = Vector3.Distance(p1, p2) / 2;
        float y = Mathf.Sqrt(Mathf.Pow(legDistance, 2) - Mathf.Pow(a, 2));
        return new Vector3(x, y, z);
    }

    internal void adjustGroundedHeight(float h, Vector3 normal, float speed, bool instant = false) {
        if (h < 0) {
            h = 0;
        }
        // RaycastHit hit;
        // Vector3 start = Utils.copy(adjustTarget.position);
        // start.y += DOWN_RAY_OFFSET;
        // if (Physics.Raycast(start, -normal, out hit, DOWN_RAY_OFFSET + airRayCastDistance, ~airRayCastIgnoreLayer)) {
            // h -= hic.ap.standHeight;
        Transform adjustTarget = hic.spin1.transform;
        RaycastHit hit = getGroundedHit(normal);
        if (hit.collider != null) {
            Vector3 desiredPos = hit.point + normal * h;
            desiredPos.y = h;
            if (!instant) {
                Utils.deltaMove(adjustTarget, Vector3.MoveTowards(adjustTarget.position, desiredPos, speed * Time.deltaTime));
            } else {
                Utils.deltaMove(adjustTarget, desiredPos);
            }
        }
        // }
    }

    internal RaycastHit getGroundedHit(Vector3 normal) {
        RaycastHit hit;
        Transform adjustTarget = hic.spin1.transform;
        Vector3 start = Utils.copy(adjustTarget.position);
        start.y += DOWN_RAY_OFFSET;
        if (Physics.Raycast(start, -normal, out hit, DOWN_RAY_OFFSET + airRayCastDistance, ~airRayCastIgnoreLayer)) {
            return hit;
        }
        return hit;
    }

    internal (bool, float) getGroundedHeight(Vector3 normal) {
        RaycastHit hit = getGroundedHit(normal);
        if (hit.collider != null) {
            float h = hic.spin1.transform.position.y - hit.point.y;
            if (h > 0) {
                return (true, h);
            }
        }
        return (false, 0);
    }

    internal float hipHeightDiff(float h, Vector3 normal) {
        float r = 0;
        RaycastHit hit;
        Vector3 start = Utils.copy(transform.position);
        start.y += DOWN_RAY_OFFSET;
        if (Physics.Raycast(start, -normal, out hit, DOWN_RAY_OFFSET + airRayCastDistance, ~airRayCastIgnoreLayer)) {
            h -= hipHeightOffset;
            Vector3 desiredPos = hit.point + normal * h;
            desiredPos.y = h;
            r = Vector3.Distance(transform.position, desiredPos);
        }
        return r;
    }
    

    internal void updateTransferDirection(Vector3 d) {
        transferDir = d;
    }
    internal float getTransferSpeed(float angel, float duration) {
        Vector3 d1 = Utils.forwardFlat(body.transform);
        Vector3 d2 = Quaternion.AngleAxis(angel, Vector3.up) * transferDir;
        return Vector3.Angle(d1, d2) / duration;
    }
    internal void transfer(float angelOffset, float speed) {
        Vector3 forward = Utils.forwardFlat(target);
        Vector3 right = Utils.right(target);
        Quaternion tr = Quaternion.LookRotation(transferDir);       
        Quaternion offset = Quaternion.AngleAxis(angelOffset, Vector3.up);
        tr *= offset;
        Quaternion r = Quaternion.Slerp(
            target.rotation,
            tr, 
            1 - Mathf.Exp(-speed * Time.deltaTime)
        );
        leftLeg.transform.SetParent(target);
        rightLeg.transform.SetParent(target);
        hic.logHomeOffset();
        target.rotation = r;
        rotateCurrentDampDist(forward, right);
        leftLeg.transform.SetParent(null);
        rightLeg.transform.SetParent(null);
        hic.postUpdateTowHandPosition();
    }
    internal void transferByTime(float angelOffset, float t, bool moveLeg = true) {
        Vector3 forward = Utils.forwardFlat(target);
        Vector3 right = Utils.right(target);
        Quaternion tr = Quaternion.LookRotation(transferDir);       
        Quaternion offset = Quaternion.AngleAxis(angelOffset, Vector3.up);
        tr *= offset;
        Quaternion r = Quaternion.Slerp(
            target.rotation,
            tr, 
            t
        );
        if (moveLeg) {
            leftLeg.transform.SetParent(target);
            rightLeg.transform.SetParent(target);
        }
        hic.logHomeOffset();
        target.rotation = r;
        rotateCurrentDampDist(forward, right);
        if (moveLeg) {
            leftLeg.transform.SetParent(null);
            rightLeg.transform.SetParent(null);
        }
        hic.postUpdateTowHandPosition();
    }

    internal void transfer(float angelOffset) {
        transfer(angelOffset, trackSpeed);
    }

    private void keepBalanceWhenWalking() {
        Vector3 forward2 = Utils.forwardFlat(target);
        Vector3 right2 = Utils.right(target);
        Vector3 plane = Vector3.up;
        Vector3 tp = Vector3.Lerp(
        // transform.position = Vector3.Lerp(
                                        target.position,
                                        dampDist,
                                        1 - Mathf.Exp(-finalDampSpeed * Time.deltaTime)
                                    );
        Vector3 delta = tp - lastDampStart;
        dampSum += delta.magnitude;
        dampCount++;
        hic.logHomeOffset();
        transform.position += forward2 * Vector3.Dot(delta, forward2)
                                + right2 * Vector3.Dot(delta, right2)
                                + Vector3.zero * Vector3.Dot(delta, plane);
        hic.postUpdateTowHandPosition();
        lastDampStart = target.position;
        Debug.DrawLine(target.position, dampDist, Color.red, Time.deltaTime);
        Debug.DrawLine(target.position, left.position, Color.blue, Time.deltaTime);
        Debug.DrawLine(target.position, right.position, Color.green, Time.deltaTime);
    }

    public void TryBattleIdle() {
        updateTransferDirection(Utils.forwardFlat(cam));
        moveManager.ChangeMove(MoveNameConstants.HipIdle2BattleIdle);
    }

    // public void TryRotate(float targetRotation, float targetHeight) {
    //     updateTransferDirection(Utils.forwardFlat(cam));
    //     HipHeightChangeMove m = (HipHeightChangeMove)moveManager.ChangeMove(MoveNameConstants.HipHeightChangeMove);
    //     m.targetRotation = targetRotation;
    //     m.groundHeight = targetHeight;
    // }

    // public void TryRun(GameObject movingSphere, Vector3 offset) {
    //     HipRunMove m = (HipRunMove)moveManager.ChangeMove(MoveNameConstants.HipRunMove);
    //     m.init(movingSphere, offset);
    // }


    public void rotateCurrentDampDist(Vector3 forward, Vector3 right) {
        Vector3 offset =  dampDist - target.position;
        float z = Vector3.Dot(offset, forward);
        float x = Vector3.Dot(offset, right);
        Vector3 forward2 = Utils.forwardFlat(target);
        Vector3 right2 = Utils.right(target);
        Vector3 new_offset = transform.position + forward2 * z + right2 * x;
        dampDist = new_offset;
    }

    public void setDampDist(float fac) {
        Vector3 center = (left.transform.position + right.transform.position) / 2;
        Vector3 dist = new Vector3(center.x, target.position.y, center.z);
        lastDampStart = target.position;
        // Debug.Log(this.GetType().Name + " dampSum " + dampSum);
        // Debug.Log(this.GetType().Name + " dampCount " + dampCount);
        dampSum = 0;
        dampCount = 0;
        dist += Utils.forwardFlat(target) * dampDis * fac;
        finalDampSpeed = dampSpeed * fac;
        dampDist = dist;
    }

    IEnumerator walkPose(float moveDuration)
    {
        walkPosing = true;
        float timeElapsed = 0;
        float duration = moveDuration;
        timeElapsed += Time.deltaTime;
        Vector2 startPoint = new Vector2(0, 0);
        Vector2 endPoint = new Vector2(2, 0);
        Vector2 centerPoint = new Vector2(1, walkPoseLift);
        Vector2 refY1, refY2 = new Vector2(0,0);
        // float sum = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / duration;
            
            // normalizedTime = EasingFunction.EaseInOutCubic(0, 1, normalizedTime);
            // Quadratic bezier curve
            refY1 = new Vector2(refY2.x, refY2.y);
            refY2 = Vector2.Lerp(
                Vector2.Lerp(startPoint, centerPoint, normalizedTime),
                Vector2.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime
            );
            // sum += refY2.y - refY1.y;
            // Vector3 before = new Vector3(target.position.x, target.position.y, target.position.z);
            target.position = new Vector3(target.position.x, target.position.y + refY2.y - refY1.y, target.position.z);
            // Debug.DrawLine(before, target.position, Color.red, 50);
            if (timeElapsed >= duration) {
                break;
            } 
            yield return null;
        }
        while (timeElapsed < duration);
        walkPosing = false;
    }
    public void startWalk(float moveDuration) {
        if (walkPosing) {
            return;
        } else {
        }
        StartCoroutine(walkPose(moveDuration));
    }

    internal void updateHipHeight() {
        ReturnToCenter();
        adjustLegDistance();
        Vector3 h = getDynamicHeight(leftLeg.transform.position,
                                            rightLeg.transform.position,
                                            expectLegDistance);
        adjustGroundedHeight(h.y, Vector3.up, hipBattleSpeed);
    }

    internal void TryRun(LegRunMove leftBeat, LegRunMove rightBeat) {
        HipRunMove m = (HipRunMove)moveManager.ChangeMove(MoveNameConstants.HipRunMove);
        leftBeat.AcceptLegRunBeat += m.onLegBeats;
        rightBeat.AcceptLegRunBeat += m.onLegBeats;
    }
    internal void TryIdle(float t) {
        moveManager.ChangeMove(MoveNameConstants.HipIdle);
        ((HipIdleMove)move).initBasic(t);
    }

    internal void justRotateHip(Vector3 dir, float angelOffset, float speed) {
        Vector3 forward = Utils.forwardFlat(target);
        Vector3 right = Utils.right(target);
        Quaternion tr = Quaternion.LookRotation(dir);       
        Quaternion offset = Quaternion.AngleAxis(angelOffset, Vector3.up);
        tr *= offset;
        Quaternion r = Quaternion.Slerp(
            target.rotation,
            tr, 
            1 - Mathf.Exp(-speed * Time.deltaTime)
        );
        target.rotation = r;
    }

    internal void TryAir() {
        moveManager.ChangeMove(MoveNameConstants.HipAirMove);
    }

    internal void TryChangeHeight(float h, float duration) {
        HipHeightChangeMove move = (HipHeightChangeMove)moveManager.ChangeMove(MoveNameConstants.HipHeightChangeMove);
        move.initBasics(h, duration);
    }
}
