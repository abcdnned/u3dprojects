using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TargetController : MonoBehaviour {

    internal Timer walkingStopTime = new Timer();
    protected ReadTrigger postWalkingTrigger = new ReadTrigger(false);
    protected ReadTrigger keepWalkingTrigger = new ReadTrigger(false);
    protected ReadTrigger stopWalkingTrigger = new ReadTrigger(false);
    protected ReadTrigger idleTrigger = new ReadTrigger(false);
    public WalkPointer walkPointer;
    public HumanIKController humanIKController;

    public const float DEFAULT_DURATION_FACTOR = 0.5f;
    public bool Recover;

    public Move move;
    public bool enable = true;
    public Rigidbody body;

    public TargetController pairComponent;
    private Banner recentBanner;
    protected MoveManager moveManager;

    private void Awake() {
        moveManager = new MoveManager(this);
        initMove();
    }

    protected virtual void initMove() {

    }
    protected virtual void sync() {
        // implementation of sync for base class goes here
    }

    // protected void registerBanner() {
    //     if (recentBanner != null) {
    //         recentBanner.registerSub(this);
    //     }
    // }
    // protected void notifyBanner() {
    //     if (recentBanner != null && recentBanner.available()) {
    //         recentBanner.Finish();
    //         recentBanner = null;
    //         Debug.Log(this.GetType().Name + " notifyBanner ");
    //     }
    // }

    public void handleEvent(string evtId, Banner banner) {
        // recentBanner = banner;
        handleEvent(evtId);
    }
    public void handleEvent(string eventId) {
        if ((move.IsHandMoving() || move.IsLegMoving()) && String.Equals(eventId, HumanIKController.EVENT_STOP_WALKING)) {
            // Debug.Log(this.GetType().Name + " event trigger ");
            walkingStopTime.setTimer(0.1f);
        }
        if (String.Equals(eventId, HumanIKController.EVENT_KEEP_WALKING)) {
            walkingStopTime.reset();
            pairComponent.walkingStopTime.reset();
        }
        if (eventId == HumanIKController.EVENT_IDLE) {
            idleTrigger.set();
        }
    }

    public void TryTransferDirectly(Vector3 point, float angelOffset)
    {
        TryTransferDirectly(point, DEFAULT_DURATION_FACTOR, angelOffset);
    }

    public void TryTransferDirectly(Transform target)
    {
        TryTransferDirectly(target, DEFAULT_DURATION_FACTOR);
    }
    public void TryTransferDirectly(Vector3 point, float durationFactor, float angelOffset)
    {
        if (!enable) return;
        if (move.IsHandMoving()) return;
        // registerBanner();
        StartCoroutine(TransferDirectly(point, durationFactor, angelOffset));
    }
    public void TryTransferDirectly(Transform target, float durationFactor)
    {
        if (!enable) return;
        if (move.IsHandMoving()) return;
        // registerBanner();
        StartCoroutine(TransferDirectly(target, durationFactor));
    }

    protected IEnumerator TransferDirectly(Vector3 point, Vector3 direction,
                                           Vector3 right, Transform forwardTarget,
                                           float angelOffset, float durationFactor)
    {
        sync();
        moveManager.ChangeMove(MoveNameConstants.HandMoving);
        Recover = true;

        Quaternion endRot = forwardTarget.rotation;

        float timeElapsed = 0;
        Vector3 wp1 = transform.position;
        Vector3 plane = Vector3.up;
        Vector3 wp2 = point;
        float duration = (wp2 - wp1).magnitude * durationFactor;
        Steper steper = new Steper(direction, right, duration, Steper.LEFP, body.transform, transform,
                                   new Vector3[] { wp1, wp2 } );
        Rotater rotater = new Rotater(body.transform, transform,
                                      duration,
                                      new Vector3(angelOffset, 0, 0));
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / duration;
            steper.step(Time.deltaTime);
            rotater.rot(Time.deltaTime);
            if (timeElapsed >= duration) {
                break;
            } 
            yield return null;
        }
        while (timeElapsed < duration);
        moveManager.ChangeMove(MoveNameConstants.HandIdle);
        Recover = false;
        // notifyBanner();
    }

    protected IEnumerator TransferDirectly(Vector3 point, float durationFactor, float angelOffset)
    {
        return TransferDirectly(point, Utils.forward(body.transform),
                                Utils.right(body.transform), walkPointer.transform,
                                angelOffset, durationFactor);
    }
    protected IEnumerator TransferDirectly(Transform target, float durationFactor)
    {
        return TransferDirectly(target.position, Utils.forward(target), Utils.right(target),
                                walkPointer.transform, 0, durationFactor);
    }
}
