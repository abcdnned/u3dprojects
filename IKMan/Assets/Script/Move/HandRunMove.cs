using System;
using UnityEngine;

public class HandRunMove : LegHandRunMove
{

    String[] names = new string[] { "HandRun1", "HandRun2", "HandRun3" };

    public HandRunMove() : base(MoveNameConstants.HandRunMove)
    {
    }
    protected override string[] getNames() {
        return names;
    }

    protected override void updateIKRotation() {
        handController().LookToArmLook();
    }

}
