using UnityEngine;

public class LegMove : Move
{
    protected LegControllerType2 legController;
    public LegMove(string name) : base(name) {
    }
    public override void init() {
        legController = (LegControllerType2)targetController;
        normalizedTime = 0;
        state = 0;
        subinit();
    }

    protected virtual void subinit() {
    }

}