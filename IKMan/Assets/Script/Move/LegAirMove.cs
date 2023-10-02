using System;
using UnityEngine;
using static IKSampleNames;

public class LegAirMove : LegHandRunMove
{
    String[] names = new string[] { LEG_AIR_1, LEG_AIR_2 };

    int[] indexMapping = new int[] { 0, 1 };

    

    public LegAirMove() : base(MoveNameConstants.LegAirMove)
    {
    }

    protected override string[] getNames() {
        return names;
    }

    internal override int[] getIndexMapping() {
        return indexMapping;
    }
}
