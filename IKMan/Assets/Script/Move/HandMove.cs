using UnityEngine;

public class HandMove : Move
{
    protected HandController parent;
    public HandMove(string name) : base(name)
    {

    }
    public override void init() {
        parent = (HandController)targetController;
        normalizedTime = 0;
        state = 0;
        subinit();
    }

    protected virtual void subinit() {
    }
}
