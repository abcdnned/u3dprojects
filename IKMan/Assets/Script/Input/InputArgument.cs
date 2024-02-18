using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputArgument : MonoBehaviour
{
    internal Vector3 movement;

    internal bool jumpFlag;

    internal bool leftClick;
    internal bool leftHold = false;
    internal void reset() {
        jumpFlag = false;
        leftClick = false;
    }

}