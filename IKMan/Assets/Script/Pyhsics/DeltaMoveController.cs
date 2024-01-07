using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaMoveController
{
    internal GameObject target;

    internal HumanIKController hic;

    internal InputArgument ia;

    protected Queue<int> signalQueue = new Queue<int>();

    internal void update() {
        deltaMove();
        signalQueue.Clear();
    }
    internal virtual void handleSignal(int s) {
    }
    internal virtual void deltaMove() {
    }

    internal virtual void init() {
    }

    internal virtual void exit() {
    }
    internal virtual Vector3 getVelocity() {
        return Vector3.zero;
    }

    internal virtual int onGround() {
        return -1;
    }

    internal void addInputSignal(int sig) {
        signalQueue.Enqueue(sig);
    }

}