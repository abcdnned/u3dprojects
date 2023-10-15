using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaMoveController
{
    internal GameObject target;

    internal HumanIKController hic;

    internal InputArgument ia;

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

}