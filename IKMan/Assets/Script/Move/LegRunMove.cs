using System;
using UnityEngine;

public class LegRunMove : LegHandRunMove
{
    String[] names = new string[] { "LegRun1", "LegRun2", "LegRun3" };

    int[] indexMapping = new int[] { 0, 1, 2, 1 };

    public LegRunMove() : base(MoveNameConstants.LegRunMove)
    {
    }

    protected override string[] getNames() {
        return names;
    }

    protected override int[] getIndexMapping() {
        return indexMapping;
    }

}
