using System;
using UnityEngine;

public class HandRunMove : LegHandRunMove
{

    String[] names = new string[] { "HandRun1", "HandRun2", "HandRun3" };

    int[] indexMapping = new int[] { 0, 1, 2, 1 };

    public HandRunMove() : base(MoveNameConstants.HandRunMove)
    {
    }
    protected override string[] getNames() {
        return names;
    }

    protected override int[] getIndexMapping() {
        return indexMapping;
    }

}
