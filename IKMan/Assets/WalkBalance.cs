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
    [Header("--- GENERAL ---")]
    [SerializeField] float trackSpeed = 5;

    public Transform cam;
    private Vector3 transferDir;
    public float airRayCastDistance = 0.5f;
    public LayerMask airRayCastIgnoreLayer;
    public static float DOWN_RAY_OFFSET = 10;

    internal void ReturnToCenter() {
        Vector3 center = (left.transform.position + right.transform.position) / 2;
        Vector3 dist = new Vector3(center.x, target.position.y, center.z);
        dist += Utils.forward(transform) * overshoot;
        dist = new Vector3(dist.x, target.position.y, dist.z);
        humanIKController.logHomeOffset();
        target.position = Vector3.Lerp(
                                    target.position,
                                    dist,
                                    1 - Mathf.Exp(-returnCenterSpeed * Time.deltaTime)
                                );
        humanIKController.postUpdateTowHandPosition();
    }

    void Update()
    {
        if (move != null
            && move.name != MoveNameConstants.HipIdle
            && move.name != MoveNameConstants.HipDamp) {
            move = move.move(Time.deltaTime);
        } else {
            if ((leftLeg.move.IsLegMoving() && !leftLeg.Recover) || (rightLeg.move.IsLegMoving() && !rightLeg.Recover)) {
                // Debug.Log(this.GetType().Name + " walking ");
                moveManager.ChangeMove(MoveNameConstants.HipDamp);
                keepBalanceWhenWalking();
            } else {
                moveManager.ChangeMove(MoveNameConstants.HipIdle);
                ReturnToCenter();
            }
        }
        // Update rotation based on camera.
        if (humanIKController.currentStatus.getName() == LocomotionState.NAME
            && humanIKController.currentStatus.cs.name == LocomotionState.STATE_MOVE) {
            Vector2 m = humanIKController.getMovement();
            Vector3 dir = Utils.forward(cam) * m.y + Utils.right(cam) * m.x;
            updateTransferDirection(dir);
            transfer(0);
        }
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
        moveManager.ChangeMove(MoveNameConstants.HipIdle);
    }

    internal void adjustHeight(float h, Vector3 normal, float speed) {
        if (h < 0) {
            h = 0;
        }
        RaycastHit hit;
        Vector3 start = Utils.copy(transform.position);
        start.y += DOWN_RAY_OFFSET;
        if (Physics.Raycast(start, -normal, out hit, DOWN_RAY_OFFSET + airRayCastDistance, ~airRayCastIgnoreLayer)) {
            h -= hipHeightOffset;
            Vector3 desiredPos = hit.point + normal * h;
            desiredPos.y = h;
            Utils.deltaMove(transform, Vector3.MoveTowards(transform.position, desiredPos, speed * Time.deltaTime));
        }
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
    

    private void updateTransferDirection(Vector3 d) {
        transferDir = d;
    }

    internal void transfer(float angelOffset) {
        Vector3 forward = Utils.forward(target);
        Vector3 right = Utils.right(target);
        Quaternion tr = Quaternion.LookRotation(transferDir);       
        Quaternion offset = Quaternion.AngleAxis(angelOffset, Vector3.up);
        tr *= offset;
        Quaternion r = Quaternion.Slerp(
            target.rotation,
            tr, 
            1 - Mathf.Exp(-trackSpeed * Time.deltaTime)
        );
        leftLeg.transform.SetParent(target);
        rightLeg.transform.SetParent(target);
        humanIKController.logHomeOffset();
        target.rotation = r;
        rotateCurrentDampDist(forward, right);
        leftLeg.transform.SetParent(null);
        rightLeg.transform.SetParent(null);
        humanIKController.postUpdateTowHandPosition();
    }

    private void keepBalanceWhenWalking() {
        Vector3 forward2 = Utils.forward(target);
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
        humanIKController.logHomeOffset();
        transform.position += forward2 * Vector3.Dot(delta, forward2)
                                + right2 * Vector3.Dot(delta, right2)
                                + Vector3.zero * Vector3.Dot(delta, plane);
        humanIKController.postUpdateTowHandPosition();
        lastDampStart = target.position;
        Debug.DrawLine(target.position, dampDist, Color.red, Time.deltaTime);
        Debug.DrawLine(target.position, left.position, Color.blue, Time.deltaTime);
        Debug.DrawLine(target.position, right.position, Color.green, Time.deltaTime);
    }

    public void TryBattleIdle() {
        updateTransferDirection(Utils.forward(cam));
        moveManager.ChangeMove(MoveNameConstants.HipIdle2BattleIdle);
    }

    public void TryRotate(float targetRotation, float targetHeight) {
        updateTransferDirection(Utils.forward(cam));
        HipHeightChangeMove m = (HipHeightChangeMove)moveManager.ChangeMove(MoveNameConstants.HipHeightChangeMove);
        m.targetRotation = targetRotation;
        m.groundHeight = targetHeight;
    }


    public void rotateCurrentDampDist(Vector3 forward, Vector3 right) {
        Vector3 offset =  dampDist - target.position;
        float z = Vector3.Dot(offset, forward);
        float x = Vector3.Dot(offset, right);
        Vector3 forward2 = Utils.forward(target);
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
        dist += Utils.forward(target) * dampDis * fac;
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

}
