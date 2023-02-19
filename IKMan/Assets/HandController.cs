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
    [SerializeField]float stage1 = 0.2f;
    [SerializeField]float stage2 = 0.85f;
    [SerializeField]float isRightHand = 1;
    [SerializeField] Transform hint;
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

    private Vector3[] getEndPoint(Transform target, float pairProjectDis, Vector3 plane) {
        return null;
    }
    public void postUpdateTowHandPosition() {
        Vector3 delta = handHome.forward * homeOffset.z
                      + handHome.right * homeOffset.x
                      + handHome.up * homeOffset.y;
        Vector3 target = handHome.position + delta;
        Utils.deltaMove(transform, target);
    }

    public void logHomeOffset() {
        Vector3 mathOffset = transform.position - handHome.position;
        float z = Vector3.Dot(mathOffset, handHome.forward);
        float x = Vector3.Dot(mathOffset, handHome.right);
        float y = Vector3.Dot(mathOffset, handHome.up);
        homeOffset = new Vector3(x, y, z);
    }

    IEnumerator MoveToHome(float pairProjectDis)
    {
        sync();
        Moving = true;
        yield return null;
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

    public void TryMove()
    {
        if (Moving) return;

        StartCoroutine(MoveToHome(0f));
    }

}
