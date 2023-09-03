using System;
using UnityEngine;

public class LegRunMove : LegMove
{
    float initTime;
    float half_duration;
    float offsetTime;

    int previousIndex;

    String[] names = new string[] { "LegRun1", "LegRun2", "LegRun3" };

    int[] indexMapping = new int[] { 0, 1, 2, 1 };

    public LegRunMove() : base(MoveNameConstants.LegRunMove)
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
            float t = normalizedTime + offsetTime - initTime;
            int index = Mathf.CeilToInt(t / half_duration) % 4;
            if (previousIndex != index) {
                previousIndex = index;
                String syncName = getSyncName(index);
                legController.SyncIKSample(syncName, half_duration);
            }
        }
        if (state == 2) {
            previousIndex = -1;
        }
        return this;
    }

    private String getSyncName(int i) {
        return names[indexMapping[i]];
    }


}
