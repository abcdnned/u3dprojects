using System;
using UnityEngine;
using static IKSampleNames;

public class HandAirMove : LegHandRunMove
{
    String[] names = new string[] { HAND_AIR_1, HAND_AIR_2 };

    int[] indexMapping = new int[] { 0, 1 };

    

    public HandAirMove() : base(MoveNameConstants.HandAirMove)
    {
    }

    protected override string[] getNames() {
        return names;
    }

    internal override int[] getIndexMapping() {
        return indexMapping;
    }
}
