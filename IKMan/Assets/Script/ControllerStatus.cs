using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerStatus
{
    public const int INPUT_R = 0;

    public virtual bool checkAcceptInput(int inputCode) {
        return true;
    }
}
