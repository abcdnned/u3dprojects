using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : TargetController
{
    public Transform handHome;
    [SerializeField] float moveDuration = 0.2f;
    [SerializeField] WalkBalance walkBalance;
    [SerializeField]float stage1 = 0.5f;
    [SerializeField]float stage2 = 1f;
    [SerializeField]int isRightHand = 1;
    [SerializeField]float swingHandDis = 0.2f;
    [SerializeField]float swingHandUp = 0.1f;
    [SerializeField] Transform hint;
    [SerializeField] float swingBackDF = .5f;
    public float normalizedTime = -1f;
    [SerializeField] bool syncPair;
    public HandLooker handHint;
    public Transform arm;

    public Transform HandLook;

    [Header("--- Main2BattleIdle ---")]
    public float m2b_pivotOffset = 0.3f;
    public float m2b_pivotOffset2 = 0.5f;
    public float m2b_duration = 0.2f;
    public float m2b_duration2 = 0.2f;
    public Vector3 m2b_rotation = new Vector3(-220, -70, 0);
    public Vector3 m2b_rotation2 = new Vector3(20, 180, -90);
    public float m2b_elbow = 60f;
    public float m2b_idle_elbow = 180f;

    public Vector3 m2b_tpOffset = new Vector3(0,0,0);

    private ReadTrigger walkingStop = new ReadTrigger(false);
    private ReadTrigger lastStep = new ReadTrigger(false);

    private Vector3 homeOffset = Vector3.zero;

    private Quaternion homeRotationDelta = Quaternion.identity;

    protected override void initMove() {
        HandMovingMove handMovingMove = new HandMovingMove();
        moveManager.addMove(handMovingMove);
        moveManager.addMove(new HandIdleMove());
        moveManager.addMove(new MainHoldWeapon());
        moveManager.addMove(new HandMain2Battle());
        moveManager.addMove(new HandMainBattle2Idle());
        moveManager.ChangeMove(MoveNameConstants.HandIdle);
    }

    private Vector3[] getEndPoint(Transform body, Transform home, Vector3 up, int isRightFoot, int isRightHand) {
        Vector3 forward = Utils.forwardFlat(body.transform);
        Vector3 endPoint = Vector3.zero;
        if (isRightFoot * isRightHand < 0) {
            endPoint = home.position + forward * swingHandDis + up * swingHandUp;
        } else {
            endPoint = home.position + forward * swingHandDis * -1 + up * swingHandUp;
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
        moveManager.ChangeMove(MoveNameConstants.HandMoving);
        float timeElapsed = 0;
        Vector3 plane = Vector3.up;
        Vector3 forward = Utils.forwardFlat(body.transform);
        Vector3 right = Utils.right(body.transform);
        Vector3[] Points = getEndPoint(body.transform, handHome.transform, plane, isRightFoot, isRightHand);
        Vector3 endPoint = Points[0];

        Vector3 forward2 = (endPoint - transform.position).normalized;
        Vector3 wp1 = transform.position;
        bool shouldSwing = Utils.IsSecondPositionBetween(wp1, handHome.position, endPoint, Utils.forwardFlat(body.transform));
        Steper steper1 = null;
        //Instantiate a new SteperBuilder object with the given options
        SteperBuilder steperBuilder = new SteperBuilder()
            .WithForward(forward) //specify the forward direction
            .WithRight(right) //specify the right direction
            .WithDuration(duration) //specify the duration of the stepping 
            .WithBody(body.transform) //specify the body to be stepped on 
            .WithTarget(transform); //specify the target transform for the stepping
        if (!shouldSwing) {
            steper1 = steperBuilder.WithLerpFunction(Steper.LEFP)
                                   .WithPoints(new Vector3[] {wp1, endPoint}).Build();
            // PrefabCreator.SpawnDebugger(endPoint, "DebugBall", duration, 0.1f, body.transform);
        } else {
            Vector3 wp2 = handHome.transform.position;
            steper1 = steperBuilder.WithLerpFunction(Steper.BEARZ)
                                   .WithPoints(new Vector3[] {wp1, wp2, endPoint}).Build();
            // PrefabCreator.SpawnDebugger(wp2, "DebugBall", duration, 0.1f, body.transform);
            // PrefabCreator.SpawnDebugger(endPoint, "DebugBall", duration, 0.1f, body.transform);
        }
        do
        {
            Vector3 forward3 = Utils.forwardFlat(body.transform);
            Vector3 right3 = Utils.right(body.transform);
            timeElapsed += Time.deltaTime;
            steper1.step(Time.deltaTime);
            if (timeElapsed >= duration) {
                break;
            } 
            yield return null;
            walkingStopTime.countDown(Time.deltaTime);
            if (timeElapsed >= 0 && walkingStopTime.check()) {
                // Debug.Log(this.GetType().Name + " postwalkingHand ");
                postWalkingTrigger.set();
                break;
            }
        }
        while (timeElapsed < duration);
        if (walkingStopTime.getTime() > 0) {
            postWalkingTrigger.set();
        }
        normalizedTime = -1;
        moveManager.ChangeMove(MoveNameConstants.HandIdle);
        if (postWalkingTrigger.read()) {
            TryTransferDirectly(handHome.transform, swingBackDF);
        } else {
            // notifyBanner();
        }
    }


    protected override void sync()
    {
        if (syncPair) {
            moveDuration = ((HandController)pairComponent).moveDuration;
            stage1 = ((HandController)pairComponent).stage1;
            stage2 = ((HandController)pairComponent).stage2;
            isRightHand = -((HandController)pairComponent).isRightHand;
            swingHandDis = ((HandController)pairComponent).swingHandDis;
            swingHandUp = ((HandController)pairComponent).swingHandUp;
            swingBackDF = ((HandController)pairComponent).swingBackDF;
        }
    }

    public void TryMove(float duration, float isRightFoot)
    {
        if (move.name == MoveNameConstants.HandMoving) return;

        // registerBanner();
        StartCoroutine(MoveToHome(duration, Mathf.FloorToInt(isRightFoot)));
    }

    internal void TryGetGreatSword()
    {
        HandMain2Battle move = (HandMain2Battle)moveManager.ChangeMove(MoveNameConstants.HandMain2Battle);
        move.initTargetRotation(m2b_rotation.x, m2b_rotation.y, m2b_rotation.z);
        move.initTargetRotation2(m2b_rotation2.x, m2b_rotation2.y, m2b_rotation2.z);
        move.initBasic(m2b_duration, m2b_duration2,
                       humanIKController.weaponHandle,
                       humanIKController.weaponReadyHandle,
                       1, m2b_pivotOffset);
        move.initHint(handHint.transform);
        move.beReady();
    }

    internal void TryReturnSword() {
        HandMainBattle2Idle move = (HandMainBattle2Idle)moveManager.ChangeMove(MoveNameConstants.HandMainBattle2Idle);
        move.initTargetRotation(m2b_rotation.x, m2b_rotation.y, m2b_rotation.z);
        move.initTargetRotation2(0,0,0);
        move.initBasic(m2b_duration, m2b_duration2,
                       humanIKController.weaponHandle,
                       humanIKController.rightHand.handHome,
                       1, m2b_pivotOffset);
        move.initHint(handHint.transform);
        move.beReady();
    }

    internal void LookToHandLook() {

    }

    private void Update() {
        move.move(Time.deltaTime);
    }
}
