using UnityEngine;
using System;

public class LegHandMove : Move
{
    internal Func<Quaternion> getFootRotation;


    public LegHandMove(string name) : base(name) {
    }
    public override void init() {
        normalizedTime = 0;
        state = 0;
    }

    public LegControllerType2 legController() {
        if (targetController is LegControllerType2) {
            return (LegControllerType2)targetController;
        }
        return null;
    }
    
    public HandController handController() {
        if (targetController is HandController) {
            return (HandController)targetController;
        }
        return null;
    }

    public TwoNodeController twoNodeController() {
        if (targetController is TwoNodeController) {
            return (TwoNodeController)targetController;
        }
        return null;

    }

    protected virtual void updateIKRotation() {
        if (getFootRotation != null) {
            legController().transform.rotation = getFootRotation();
        } else {
            legController().transform.rotation = getBaseOnArmRotation();
        }
    }

    internal virtual Quaternion getBaseOnArmRotation() {
        return legController().LookToArmLook(-90, false);
    }
}