using System;
using UnityEngine;
using static IKSampleNames;

public class LegRunMove : LegHandRunMove
{
    String[] names = new string[] {LEG_RUN_1, LEG_RUN_2_PUSH, LEG_RUN_2_LIFT, LEG_RUN_3};

    int[] indexMapping = new int[] { 0, 1, 3, 2 };

    

    public LegRunMove() : base(MoveNameConstants.LegRunMove)
    {
    }

    protected override string[] getNames() {
        return names;
    }

    internal override int[] getIndexMapping() {
        return indexMapping;
    }
    protected override void dropBeats(int previousIndex, int index) {
        base.dropBeats(previousIndex, index);
        if (getIndexMapping()[index] == 3) {
            AcceptLegRunBeat?.Invoke(1, legController().IsRightPart());
        } else if (getIndexMapping()[index] == 1) {
            AcceptLegRunBeat?.Invoke(0, legController().IsRightPart());
        }
    }
}
