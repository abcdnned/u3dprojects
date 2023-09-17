using System;
using UnityEngine;

public class LegRunMove : LegHandRunMove
{
    String[] names = new string[] { "LegRun1", "LegRun2.push", "LegRun2.lift", "LegRun3" };

    internal Func<Quaternion> getFootRotation;

    int[] indexMapping = new int[] { 0, 1, 3, 2 };

    

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
            legController().transform.rotation = getBaseOnArmRotation();
        }
    }

    internal Quaternion getBaseOnArmRotation() {
        return legController().LookToArmLook(-90, false);
    }
    internal override int[] getIndexMapping() {
        return indexMapping;
    }

}
