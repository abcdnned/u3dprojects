using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkBalance : MonoBehaviour
{
    [SerializeField] Transform left;
    [SerializeField] Transform right;

    [SerializeField] Transform target;

    [SerializeField] float overshoot = 0.2f;
    [SerializeField] float dampSpeed = 2f;
    [SerializeField] float dampDis = 0.1f;
    [SerializeField] float afterDampingSpeed = 1f;

    [SerializeField] float walkPoseDuration = 0.2f;

    [SerializeField] float walkPoseLift = 0.1f;
    [SerializeField] bool walkPosing = false;
    [SerializeField] HumanIKController humanIKController;


    private bool afterDamping = false;
    private bool forwardDamping = false;

    private float forwardDampingDuration = 0f;

    private float forwardDampingTimeElapse = 0f;

    private Vector3 moveAcc = new Vector3(0,0,0);

    private Vector3 dist1, dist2 = new Vector3(0,0,0);


    void Update()
    {
        Vector3 center = (left.transform.position + right.transform.position) / 2;

        Vector3 dist = new Vector3(center.x, target.position.y, center.z);
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
        target.position = dist;

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

    public void startForwardDamping(float duration) {
        forwardDampingDuration = duration;
        forwardDampingTimeElapse = 0f;
        forwardDamping = true;
        // Debug.Log(this.GetType().Name + " start damp ");
    }

    public void stopForwardDamping() {
        forwardDampingDuration = 0f;
        forwardDampingTimeElapse = 0f;
        forwardDamping = false;
        // Debug.Log(this.GetType().Name + " stop damp ");
    }

    IEnumerator walkPose()
    {
        walkPosing = true;
        float timeElapsed = 0;
        float duration = walkPoseDuration;
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
    public void startWalk() {
        if (walkPosing) return;
        StartCoroutine(walkPose());
        // Debug.Log(this.GetType().Name + " startWalk ");
    }

    public void handleEvent(string eventId) {

    }
}
