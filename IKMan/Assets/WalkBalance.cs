using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkBalance : MonoBehaviour
{
    [SerializeField] Transform left;
    [SerializeField] Transform right;
    public LegControllerType2 leftLeg;
    public LegControllerType2 rightLeg;

    public HandController leftHand;

    public HandController rightHand;

    [SerializeField] Transform target;

    [SerializeField] float overshoot = 0.2f;
    [SerializeField] float dampSpeed = 2f;
    [SerializeField] float returnCenterSpeed = 6f;

    private float finalDampSpeed = 2f;
    [SerializeField] float dampDis = 0.1f;
    [SerializeField] float afterDampingSpeed = 1f;

    [SerializeField] float walkPoseDuration = 0.2f;

    [SerializeField] float walkPoseLift = 0.1f;
    [SerializeField] bool walkPosing = false;
    [SerializeField] HumanIKController humanIKController;
    [SerializeField] Transform direction;

    private Vector3 lastDampStart;

    private float dampSum;

    private int dampCount;



    // private bool afterDamping = false;
    // private bool forwardDamping = false;

    // private float forwardDampingDuration = 0f;

    // private float forwardDampingTimeElapse = 0f;

    // private Vector3 moveAcc = new Vector3(0,0,0);

    // private Vector3 dist1, dist2 = new Vector3(0,0,0);

    // private float centerForward = 0;
    // public float centerForwardSpeed = 0.05f;
    // public float centerFowardDownSpeed = 0.03f;

    // public float centerForwardMax = 0.15f;

    // public bool centerForwardFalg = false;

    public Vector3 dampDist = new Vector3(0,0,0);

    // public void centerForwardIncrease() {
    //     centerForwardFalg = true;
    // }

    void Update()
    {
        // Vector3 ov = Vector3.zero;
        // if (humanIKController.walking) {
        // }
        // if (centerForwardFalg) {
        //     if (centerForward < centerForwardMax) {
        //         centerForward += centerForwardSpeed;
        //     } else {
        //         centerForward -= centerFowardDownSpeed;
        //         centerForwardFalg = false;
        //     }
        // } else {
        //     centerForward -= centerFowardDownSpeed;
        // }
        // centerForward = Mathf.Clamp(centerForward, 0, centerForwardMax);
        // if (centerForward != 0) {
        //     Debug.Log(this.GetType().Name + " centerForward " + centerForward);
        // }
        // ov = Utils.forward(transform) * centerForward;
        // if (!humanIKController.walking || leftLeg.Recover || rightLeg.Recover) {
        // Debug.Log(this.GetType().Name + "wu");
        if ((leftLeg.Moving && !leftLeg.Recover) || (rightLeg.Moving && !rightLeg.Recover)) {
            keepBalanceWhenWalking();
        } else {
            // Debug.Log(this.GetType().Name + " center ");
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
        // dist += ov;
        // dist1 = Vector3.Lerp(
        //                                 target.position,
        //                                 dist,
        //                                 1 - Mathf.Exp(-dampSpeed * Time.deltaTime)
        //                             );

        // Debug.Log(this.GetType().Name + " left " + leftLeg.isStandGravity());
        // Debug.Log(this.GetType().Name + " right " + rightLeg.isStandGravity());
        // if ((!leftLeg.isStandGravity() && rightLeg.isStandGravity())
        //     || (leftLeg.isStandGravity() && !rightLeg.isStandGravity())) {
        //     Vector3 dist = leftLeg.isStandGravity() ? leftLeg.transform.position : rightLeg.transform.position;
        //     dist1 = Vector3.Lerp(
        //                                     target.position,
        //                                     dist,
        //                                     1 - Mathf.Exp(-dampSpeed * Time.deltaTime)
        //                                 );
        //     target.position = new Vector3(dist1.x, target.position.y, dist1.z);
        // } else {
        //     Debug.Log(this.GetType().Name + " center ");
        //     Vector3 center = (left.transform.position + right.transform.position) / 2;
        //     Vector3 dist = new Vector3(center.x, target.position.y, center.z);
        //     dist1 = Vector3.Lerp(
        //                                     target.position,
        //                                     dist,
        //                                     1 - Mathf.Exp(-dampSpeed * Time.deltaTime)
        //                                 );
        //     target.position = new Vector3(dist1.x, target.position.y, dist1.z);
        // }

        // dist += target.forward * overshoot;
        // target.position = dist;
        // if (humanIKController.walking && forwardDamping) {
        //     dist += target.forward * (overshoot + dampDis);
        //     dist1 = Vector3.Lerp(
        //                                     target.position,
        //                                     dist,
        //                                     1 - Mathf.Exp(-dampSpeed * Time.deltaTime)
        //                                 );
        //     target.position = new Vector3(dist1.x, target.position.y, dist1.z);
        //     forwardDampingTimeElapse += Time.deltaTime;
        //     if (forwardDampingTimeElapse >= forwardDampingDuration) {
        //         stopForwardDamping();
        //         afterDamping = true;
        //     }
        // } else {
        //     dist += target.forward * overshoot;
        //     float dp = Vector3.Dot(dist, target.forward);
        //     float cp = Vector3.Dot(target.position, target.forward);
        //     if (!humanIKController.walking || !afterDamping) {
        //         target.position = dist;
        //     } else {
        //         if (dp > cp) {
        //             // Debug.Log(this.GetType().Name + " yes ");
        //             afterDamping = false;
        //             target.position = dist;
        //         } else {
        //             // Debug.Log(this.GetType().Name + " no ");
        //             target.position = target.position + target.forward *  (1 - Mathf.Exp(-afterDampingSpeed * Time.deltaTime));
        //         }
        //     }
        // }

        // Vector3 direction = target.forward;
        // float lp = Vector3.Dot(left.position, direction);
        // float rp = Vector3.Dot(right.position, direction);

        // Vector3 front = left.position;
        // Vector3 back = right.position;
        // if (lp < rp) {
        //     front = right.position;
        //     back = left.position;
        // }

        // Vector3 dist = Vector3.Lerp(back, front, 1f);
        // target.position = new Vector3(target.position.x, target.position.y, dist.z);
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
        // Debug.Log(this.GetType().Name + " dc " + dampCount);
        // Debug.Log(this.GetType().Name + " delta " + delta);
        // transform.position += delta;
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

    // public void startForwardDamping(float duration) {
    //     forwardDampingDuration = duration;
    //     forwardDampingTimeElapse = 0f;
    //     forwardDamping = true;
    // }

    // public void stopForwardDamping() {
    //     forwardDampingDuration = 0f;
    //     forwardDampingTimeElapse = 0f;
    //     forwardDamping = false;
    //     // Debug.Log(this.GetType().Name + " stop damp ");
    // }

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
            // Debug.Log(this.GetType().Name + " still walk pose ");
            return;
        } else {
            // Debug.Log(this.GetType().Name + " walkPose ");
        }
        StartCoroutine(walkPose(moveDuration));
        // Debug.Log(this.GetType().Name + " startWalk ");
    }

    public void handleEvent(string eventId) {

    }
}
