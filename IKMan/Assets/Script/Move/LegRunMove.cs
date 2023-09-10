using System;
using UnityEngine;

public class LegRunMove : LegHandRunMove
{
    String[] names = new string[] { "LegRun1", "LegRun2", "LegRun3" };


    public LegRunMove() : base(MoveNameConstants.LegRunMove)
    {
    }

    protected override string[] getNames() {
        return names;
    }

    protected override void updateIKRotation() {
        legController().LookToArmLook(-90);
    }

}
