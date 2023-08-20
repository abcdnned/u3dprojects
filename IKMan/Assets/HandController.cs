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
    public float HintDis = 0.3f;
    public Transform arm;

    public HandDelayLooker HandLook;
    public HandDelayLooker HandElbow;
    public HandDelayLooker HandFK;
    public Transform Shoulder;

    public Transform LocalHand;
    public TimeValue<Vector3> handRotation = new TimeValue<Vector3>();
    public float handLookSpeed = 10;
    
    public HandLookIKController handLookIKController;

    [Header("--- Main2BattleIdle ---")]
    public float m2b_pivotOffset = 0.3f;
    public float m2b_pivotOffset2 = 0.5f;
    public float m2b_duration = 0.2f;
    public float m2b_duration2 = 0.2f;
    public Vector3 m2b_rotation = new Vector3(-220, -70, 0);
    public Vector3 m2b_rotation2 = new Vector3(20, 180, -90);
    public float m2b_elbow = 60f;
    public float m2b_idle_elbow = 180f;
    public float m2b_hangel = 30f;
    public float m2b_vangel = 220f;
    public float m2b_battle_h = 180f;
    public float m2b_battle_v = 30f;

    public Vector3 m2b_tpOffset = new Vector3(0,0,0);

    private ReadTrigger walkingStop = new ReadTrigger(false);
    private ReadTrigger lastStep = new ReadTrigger(false);

    private Vector3 homeOffset = Vector3.zero;

    private Quaternion homeRotationDelta = Quaternion.identity;
    public float armLookRotationH = 0;
    public float armLookRotationV = 0;
    protected override void initMove() {
        moveManager.addMove(new HandMovingMove());
        moveManager.addMove(new HandIdleMove());
        moveManager.addMove(new MainHoldWeapon());
        moveManager.addMove(new HandMain2Battle());
        moveManager.addMove(new HandMainBattle2Idle());
        moveManager.addMove(new HandSwingMove());
        moveManager.ChangeMove(MoveNameConstants.HandIdle);
        handRotation.init(Vector3.zero, Vector3.zero, 0.1f, (v1, v2, t) => Vector3.Lerp(v1, v2, t));
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

    internal void SyncIKSample(string sampleName, float duration, bool horizon_mirror = false) {
        // Debug.Log(" isRightHand " + isRightHand);
        String elbow = IKSampleNames.ELBOW + "_" + sampleName;
        String hand = IKSampleNames.HAND + "_" + sampleName;
        HandDelayLooker elbowLooker = humanIKController.poseManager.handDelayLookerMap[elbow];
        HandDelayLooker handLooker = humanIKController.poseManager.handDelayLookerMap[hand];
        if (HandElbow != null && HandFK != null) {
            HandElbow.setDuration(duration);
            HandFK.setDuration(duration);
            SyncTwoHandLooker(elbowLooker, HandElbow, horizon_mirror);
            SyncTwoHandLooker(handLooker, HandFK, horizon_mirror);
            // HandElbow.init(duration, elbowLooker.hAd, elbowLooker.vAd,
            //                          elbowLooker.hAd_lv2, elbowLooker.vAd_lv2);
            // HandFK.init(duration, handLooker.hAd, handLooker.vAd,
            //                       handLooker.hAd_lv2, handLooker.vAd_lv2);
        }
    }

    internal void finishFK() {
        HandElbow.finishFK();
        HandFK.finishFK();
    }

    protected void SyncTwoHandLooker(HandLooker source, HandLooker target, bool horizon_mirror) {
        if (source == null || target == null) return;
        target.enable_lv2 = source.enable_lv2;
        target.hAd = horizon_mirror ? -source.horizonAngel : source.horizonAngel;
        target.vAd = source.verticalAngel;
        target.hAd_lv2 = source.horizonAngel_lv2;
        target.vAd_lv2 = source.verticalAngel_lv2;
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
        activeHandLooker(duration, isRightFoot);
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
            LookToArmLook();
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

    internal void TryGetGreatSword(GameObject greateSword, GameObject hand)
    {
        HandMain2Battle move = (HandMain2Battle)moveManager.ChangeMove(MoveNameConstants.HandMain2Battle);
        move.initBasic(m2b_duration, m2b_duration2, greateSword, hand
                       );
        move.beReady();
    }

    internal void TryReturnSword(GameObject greateSword, GameObject attachPoint) {
        HandMainBattle2Idle move = (HandMainBattle2Idle)moveManager.ChangeMove(MoveNameConstants.HandMainBattle2Idle);
        move.initBasic(m2b_duration, m2b_duration2,
                       greateSword, attachPoint
                       );
        move.beReady();
    }

    internal void LookToHandLook(Vector3 upwards) {
        Quaternion look = Quaternion.LookRotation(HandLook.transform.position - transform.position,
                                                  upwards);
        Quaternion r = Quaternion.Slerp(transform.rotation,
                                        look,
                                        1 - Mathf.Exp(-handLookSpeed * Time.deltaTime));
        transform.rotation = look;
    }
    internal void LookToArmLook() {
        if (HandFK == null || HandElbow == null) {
            return;
        }
        Vector3 v1 = getArmDirection();
        // Vector3 v2 = HandLook.transform.position - transform.position;
        Vector3 v2 = getBicepDirection();
        Quaternion rotate = Quaternion.AngleAxis(90, Vector3.Cross(v2, v1));
        v1 = rotate * v1;
        Quaternion look = Quaternion.LookRotation(v1,
                                                  -getArmDirection());
        Quaternion r = Quaternion.Slerp(transform.rotation,
                                        look,
                                        1 - Mathf.Exp(-handLookSpeed * Time.deltaTime));
        transform.rotation = look;
        Quaternion hr = Quaternion.AngleAxis(armLookRotationH, transform.right);
        transform.rotation = hr * transform.rotation;
        Quaternion vr = Quaternion.AngleAxis(armLookRotationV, transform.up);
        transform.rotation = vr * transform.rotation;
    }

    private void Update() {
        move.move(Time.deltaTime);
        updateHandLocalRotation();
    }

    public Vector3 getArmDirection() {
        Vector3 r = HandFK.transform.position - HandElbow.transform.position;
        return r.normalized;
    }

    public Vector3 getBicepDirection() {
        Vector3 r = HandElbow.transform.position - Shoulder.transform.position;
        return r.normalized;
    }

    public void updateHintByFK() {
        Vector3 elbow = HandElbow.transform.position;
        Vector3 hand = HandFK.transform.position;
        Vector3 shoulder = Shoulder.transform.position;
        Vector3 v1 = elbow - shoulder;
        Vector3 v2 = elbow - hand;
        Vector3 normal = Vector3.Cross(v2, v1);
        float angel = Vector3.Angle(v1, v2);
        Quaternion rotation = Quaternion.AngleAxis(angel / 2, normal);
        Vector3 forward = rotation * v2;
        Debug.DrawLine(shoulder, elbow, Color.red);
        Debug.DrawLine(hand, elbow, Color.blue);
        // Debug.DrawRay(elbow, v1, Color.red);
        // Debug.DrawRay(elbow, v2, Color.blue);
        Debug.DrawRay(elbow, forward, Color.green);
        Vector3 offset = HintDis * forward.normalized;
        handHint.transform.position = elbow + offset;
    }

    private void activeHandLooker(float duration, float isRightFoot) {
        HandLook.init(0.1f, 0, 0);
        if (isRightFoot * isRightHand < 0) {
            SyncIKSample(IKSampleNames.WALK_FRONT_SWING, duration, isRightHand == -1);
        } else {
            SyncIKSample(IKSampleNames.WALK_BACK_SWING, duration, isRightHand == -1);
        }
    }

    internal void twistTwist(Vector3 v, float duration) {
        // Debug.Log(" twist " + v);
        handRotation.init(LocalHand.localEulerAngles, v, duration, (v1, v2, t) => Vector3.Lerp(v1, v2, t));
    }

    private void updateHandLocalRotation() {
        if (!handRotation.overdue()) {
            Vector3 r = handRotation.getValue();
            // Debug.Log(" r " + r);
            LocalHand.localEulerAngles = r;
        }
    }

    public void TryLeftSwing() {
        HandSwingMove move = (HandSwingMove)moveManager.ChangeMove(MoveNameConstants.HandSwingMove);
        move.init(humanIKController.poleJoint, transform, humanIKController.walkPointer.transform.forward, 20f);
    }
}
