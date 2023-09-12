using System;
using UnityEngine;

public class LegRunMove : LegHandRunMove
{
    String[] names = new string[] { "LegRun1", "LegRun2", "LegRun3" };

    internal Func<Quaternion> getFootRotation;




    public LegRunMove() : base(MoveNameConstants.LegRunMove)
    {
    }

    protected override string[] getNames() {
        return names;
    }

    protected override void updateIKRotation() {
        if (getFootRotation != null) {
            legController().transform.rotation = getFootRotation();
        } else {
            legController().LookToArmLook(-90);
        }
    }

}
