using UnityEngine;
using System;

public class LegHandMove : Move
{
    internal Func<Quaternion> getFootRotation;
    
    private bool mirror;
    private string sampleName;
    private bool inited = false;

    public LegHandMove(string name) : base(name) {
    }
    public LegHandMove() : base(MoveNameConstants.LegHandMove) {
    }
    public override void init() {
        normalizedTime = 0;
        state = 0;
    }
    public void initSyncProperties(float duration, string sampleName, bool mirror) {
        this.duration = duration;
        this.sampleName = sampleName;
        this.mirror = mirror;
        inited = true;
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
        } else if (legController() != null) {
            legController().transform.rotation = getBaseOnArmRotation();
        } else if (handController() != null) {
            handController().LookToArmLook();
        }
    }

    internal virtual Quaternion getBaseOnArmRotation() {
        return legController().LookToArmLook(-90, false);
    }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }

    public override Move move(float dt) {
        if (!inited) throw new Exception("LegHandMove not inited!!!");
        normalizedTime += dt;
        if (state == 0) {
            ((TwoNodeController)targetController).SyncIKSample(sampleName, duration, mirror);
            state++;
        } 
        if (state == 1) {
            updateIKRotation();
        }
        return this;
    }
}