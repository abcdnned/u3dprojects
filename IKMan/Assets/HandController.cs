using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] HandController pairComponent;
    public Transform handHome;
    [SerializeField] float moveDuration = 0.2f;
    [SerializeField] HumanIKController humanIKController;
    [SerializeField] WalkBalance walkBalance;
    [SerializeField]float stage1 = 0.5f;
    [SerializeField]float stage2 = 1f;
    [SerializeField]int isRightHand = 1;
    [SerializeField]float swingHandDis = 0.2f;
    [SerializeField]float swingHandDown = 0.1f;
    [SerializeField] Transform hint;
    [SerializeField] Rigidbody body;
    public bool Moving;
    public bool Recover;
    public float normalizedTime = -1f;
    [SerializeField] bool enable;
    [SerializeField] bool syncPair;

    private ReadTrigger walkingStop = new ReadTrigger(false);
    private ReadTrigger transferStand = new ReadTrigger(false);
    private ReadTrigger lastStep = new ReadTrigger(false);

    internal Timer walkingStopTime = new Timer();

    private Vector3 homeOffset = Vector3.zero;

    private Quaternion homeRotationDelta = Quaternion.identity;

    private Vector3[] getEndPoint(Transform body, Transform home, Vector3 up, int isRightFoot, int isRightHand) {
        Vector3 forward = Utils.forward(body.transform);
        Vector3 endPoint = Vector3.zero;
        if (isRightFoot * isRightHand < 0) {
            endPoint = home.position + forward * swingHandDis;
        } else {
            endPoint = home.position + forward * swingHandDis * -1;
        }
        return new Vector3[] { endPoint };
    }
    public void postUpdateTowHandPosition() {
        Vector3 delta = handHome.forward * homeOffset.z
                      + handHome.right * homeOffset.x
                      + handHome.up * homeOffset.y;
        Vector3 target = handHome.position + delta;
        Utils.deltaMove(transform, target);
        transform.rotation = handHome.rotation * homeRotationDelta;
    }

    public void logHomeOffset() {
        Vector3 mathOffset = transform.position - handHome.position;
        float z = Vector3.Dot(mathOffset, handHome.forward);
        float x = Vector3.Dot(mathOffset, handHome.right);
        float y = Vector3.Dot(mathOffset, handHome.up);
        homeOffset = new Vector3(x, y, z);
        homeRotationDelta = Quaternion.Inverse(handHome.rotation) * transform.rotation;
    }

    IEnumerator MoveToHome(float duration, int isRightFoot)
    {
        sync();
        Moving = true;
        float timeElapsed = 0;
        Vector3 plane = Vector3.up;
        Vector3 forward = Utils.forward(body.transform);
        Vector3 right = Utils.right(body.transform);
        Vector3[] Points = getEndPoint(body.transform, handHome.transform, plane, isRightFoot, isRightHand);
        Vector3 endPoint = Points[0];

        Vector3 forward2 = (endPoint - transform.position).normalized;
        Vector3 wp1 = transform.position;
        Vector3 wp2 = endPoint;
        do
        {
            timeElapsed += Time.deltaTime;
            normalizedTime = timeElapsed / duration;
            float poc = Mathf.Lerp(0, 1, normalizedTime / normalizedTime);
            Vector3 targetPosition =
            Vector3.Lerp(
                wp1,
                wp2,
                poc
            );
            Utils.deltaMove(transform, targetPosition);
            if (timeElapsed >= duration) {
                break;
            } 
            yield return null;
        }
        while (timeElapsed < duration);
        normalizedTime = -1;
        Moving = false;
    }


    private void sync()
    {
        if (syncPair) {
            moveDuration = pairComponent.moveDuration;
            stage1 = pairComponent.stage1;
            stage2 = pairComponent.stage2;
            isRightHand = -pairComponent.isRightHand;
        }
    }

    public void TryMove(float duration, float isRightFoot)
    {
        if (Moving) return;

        StartCoroutine(MoveToHome(duration, Mathf.FloorToInt(isRightFoot)));
    }

}
