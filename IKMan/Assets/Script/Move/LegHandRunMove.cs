using System;
using UnityEngine;

public class LegHandRunMove : LegHandMove
{
    float initTime;
    float half_duration;
    float offsetTime;

    internal int previousIndex;
    // internal int previousPreviousIndex;
    internal delegate void acceptLegRunBeat(int beat, bool isRight);
    internal acceptLegRunBeat AcceptLegRunBeat;




    int[] indexMapping = new int[] { 0, 1, 2, 1 };

    public LegHandRunMove(string name) : base(name)
    {
    }

    public void initBasic(float half_duration, float initTime, float offsetTime) {
        this.half_duration = half_duration;
        this.initTime = initTime;
        this.offsetTime = offsetTime;
        previousIndex = 0;
        // previousPreviousIndex = 0;
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
                // previousPreviousIndex = previousIndex;
                previousIndex = index;
                String syncName = getSyncName(index);
                dropBeats(previousIndex, index);
                ((TwoNodeController)targetController).SyncIKSample(syncName, half_duration, !twoNodeController().IsRightPart());
            }
            updateIKRotation();
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

    internal virtual int[] getIndexMapping() {
        return indexMapping;
    }

    protected virtual void updateIKRotation() {
    }

    protected virtual void dropBeats(int previousIndex, int index) {
    }

}
