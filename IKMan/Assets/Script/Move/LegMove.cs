using UnityEngine;

public class LegMove : Move
{
    protected LegControllerType2 parent;
    public LegMove(string name) : base(name) {
    }
    public override void init() {
        parent = (LegControllerType2)targetController;
        normalizedTime = 0;
        state = 0;
        subinit();
    }

    protected virtual void subinit() {
    }

}