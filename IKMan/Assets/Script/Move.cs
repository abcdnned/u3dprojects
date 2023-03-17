using UnityEngine;

public class Move
{
    public static int RUNING = 0;
    public static int FINISH = 1;
    public string name;
    internal int state;

    public float normalizedTime;

    public float duration;

    public Move nextMove;

    public TargetController targetController;

    public HumanIKController humanIKController;

    public MoveManager moveManager;

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

    public bool isMoving() {
        return this.name.Contains("moving");
    }

    public virtual Move transfer() {
        return this;
    }
    public virtual Move move(float dt) {
        return this;
    }

    public virtual void init() {
    }

}
