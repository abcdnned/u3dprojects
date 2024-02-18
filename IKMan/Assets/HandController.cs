using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : TwoNodeController
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

    public Transform LocalHand;
    public TimeValue<Vector3> handRotation = new TimeValue<Vector3>();
    public float handLookSpeed = 10;
    

    [Header("--- Main2BattleIdle ---")]
    public float m2b_duration = 0.2f;
    public float m2b_duration2 = 0.2f;

    public Vector3 m2b_tpOffset = new Vector3(0,0,0);

    private ReadTrigger walkingStop = new ReadTrigger(false);
    private ReadTrigger lastStep = new ReadTrigger(false);

    private Vector3 homeOffset = Vector3.zero;

    private Quaternion homeRotationDelta = Quaternion.identity;
    public float armLookRotationH = 0;
    public float armLookRotationV = 0;
    private ReadTrigger hardStop = new ReadTrigger(false);



    protected override void initMove() {
        moveManager.addMove(new HandMovingMove());
        moveManager.addMove(new HandIdleMove());
        moveManager.addMove(new MainHoldWeapon());
        moveManager.addMove(new HandMain2Battle());
        moveManager.addMove(new HandMainBattle2Idle());
        moveManager.addMove(new HandSwingMove());
        moveManager.addMove(new HandConsonant2Battle());
        moveManager.addMove(new HandRunMove());
        moveManager.addMove(new HandAirMove());
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
            if (hardStop.read()) {
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
        if (move.name == MoveNameConstants.HandMoving) {
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

    internal void TryGetGreatSwordConsonant() {
        Debug.Log(" try get gs consonant ");
        HandConsonant2Battle move = (HandConsonant2Battle)moveManager.ChangeMove(MoveNameConstants.HandConsonant2Battle);
        move.initBasic(m2b_duration, m2b_duration2);
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

    // private void Update() {
        // move.move(Time.deltaTime);
        // updateHandLocalRotation();
    // }

    public void updateHintByFK() {
        Vector3 elbow = MiddleNode.transform.position;
        Vector3 hand = EndNode.transform.position;
        Vector3 shoulder = ParentNode.transform.position;
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

    internal void updateHandLocalRotation() {
        if (handRotation.vaild() && !handRotation.overdue()) {
            Vector3 r = handRotation.getValue();
            // Debug.Log(" r " + r);
            LocalHand.localEulerAngles = r;
        }
    }

    public void TryLeftSwing() {
        HandSwingMove move = (HandSwingMove)moveManager.ChangeMove(MoveNameConstants.HandSwingMove);
        move.init(hic.poleJoint,
                  hic.attchment_rightHand.GetComponent<CharacterJoint>(),
                  transform, hic.walkPointer.transform.forward);
    }

    public void TryRun(float offset, float initTime) {
        if (!(move is HandRunMove)) {
            HandRunMove move = (HandRunMove)moveManager.ChangeMove(MoveNameConstants.HandRunMove);
            // Debug.Log(name + " initTime " + initTime);
            move.initBasic(hic.ap.runHalfDuration, initTime, offset);
        }
    }
    public override void handleEvent(string eventId) {
        if (eventId == HumanIKController.EVENT_HARD_STOP_WALKING) {
            hardStop.set();
        } else {
            base.handleEvent(eventId);
        }
    }

    public void TryIdle() {
        moveManager.ChangeMove(MoveNameConstants.HandIdle);
    }

    internal void TryAir(float offsetTime, float initTime, float duration) {
        HandAirMove move = (HandAirMove)moveManager.ChangeMove(MoveNameConstants.HandAirMove);
        move.initBasic(duration, initTime, offsetTime);
    }

}
