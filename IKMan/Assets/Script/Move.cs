using UnityEngine;

public class Move
{
    public static int RUNING = 0;
    public static int FINISH = 1;
    public string name;
    internal int state;

    public float normalizedTime;

    public Move(string n) {
        this.name = n;
    }
    public bool IsHandMoving()
    {
        return this.name == MoveNameConstants.HandMoving;
    }

    public bool IsHandIdle()
    {
        return this.name == MoveNameConstants.HandIdle;
    }

    public bool IsLegMoving()
    {
        return this.name == MoveNameConstants.LegMoving;
    }

    public bool IsLegIdle()
    {
        return this.name == MoveNameConstants.LegIdle;
    }

}
