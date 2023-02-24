using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TargetController : MonoBehaviour {

    protected Timer walkingStopTime = new Timer();
    protected ReadTrigger postWalkingTrigger = new ReadTrigger(false);
    protected ReadTrigger keepWalkingTrigger = new ReadTrigger(false);
    protected ReadTrigger stopWalkingTrigger = new ReadTrigger(false);
    protected ReadTrigger idleTrigger = new ReadTrigger(false);
    public bool Recover;

    public bool Moving;
    public bool enable = true;
    public Rigidbody body;

    public TargetController pairComponent;

    protected virtual void sync() {
        // implementation of sync for base class goes here
    }

    public void handleEvent(Event evt) {
        String eventId = evt.eventId;
        if (Moving && String.Equals(eventId, HumanIKController.EVENT_STOP_WALKING)) {
            // Debug.Log(this.GetType().Name + " event trigger ");
            walkingStopTime.setTimer(0.1f);
        }
        if (String.Equals(eventId, HumanIKController.EVENT_KEEP_WALKING)) {
            walkingStopTime.reset();
            pairComponent.walkingStopTime.reset();
        }
        if (evt.eventId == HumanIKController.EVENT_IDLE) {
            idleTrigger.set();
        }
    }
    protected void TryTransferDirectly(Transform target, float durationFactor)
    {
        if (!enable) return;
        if (Moving) return;
        Debug.Log(this.GetType().Name + " startCoroutine ");
        StartCoroutine(TransferDirectly(target, durationFactor));
    }

    protected IEnumerator TransferDirectly(Transform target, float durationFactor)
    {
        sync();
        Moving = true;
        Recover = true;

        Quaternion endRot = target.rotation;

        float timeElapsed = 0;
        Vector3 wp1 = transform.position;
        Vector3 direction = Utils.forward(target);
        Vector3 right = Utils.right(target);
        Vector3 plane = Vector3.up;
        Vector3 wp2 = target.position;
        float duration = (wp2 - wp1).magnitude * durationFactor;
        Steper steper = new Steper(direction, right, duration, Steper.LEFP, body.transform, transform,
                                   new Vector3[] { wp1, wp2 } );
        do
        {
            Vector3 forward3 = Utils.forward(body.transform);
            Vector3 right3 = Utils.right(body.transform);
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / duration;
            steper.step(Time.deltaTime);
            if (timeElapsed >= duration) {
                break;
            } 
            yield return null;
        }
        while (timeElapsed < duration);
        Moving = false;
        Recover = false;
    }
}
