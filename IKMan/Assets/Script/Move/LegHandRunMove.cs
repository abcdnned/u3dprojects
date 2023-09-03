using System;
using UnityEngine;

public class LegHandRunMove : LegHandMove
{
    float initTime;
    float half_duration;
    float offsetTime;

    int previousIndex;

    public LegHandRunMove(string name) : base(name)
    {
    }

    public void initBasic(float half_duration, float initTime, float offsetTime) {
        this.half_duration = half_duration;
        this.initTime = initTime;
        this.offsetTime = offsetTime;
    }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }
    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            state++;
        } 
        if (state == 1) {
            float t = (Time.time - initTime) + offsetTime;
            // Debug.Log(targetController.name + " t " + t);
            // Debug.Log(targetController.name + " Time.time " + Time.time + " frameCount " + Time.frameCount);
            int index = Mathf.CeilToInt(t / half_duration) % getIndexMapping().Length;
            if (previousIndex != index) {
                previousIndex = index;
                String syncName = getSyncName(index);
                ((TwoNodeController)targetController).SyncIKSample(syncName, half_duration);
            }
        }
        if (state == 2) {
            previousIndex = -1;
        }
        return this;
    }
    private String getSyncName(int i) {
        return getNames()[getIndexMapping()[i]];
    }

    protected virtual string[] getNames() {
        return null;
    }

    protected virtual int[] getIndexMapping() {
        return null;
    }

}
