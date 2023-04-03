
using UnityEngine;

public class HipMove : Move
{
    protected WalkBalance parent;

    public HipMove(string name) : base(name) {

    }
    public override void init() {
        parent = (WalkBalance)targetController;
        normalizedTime = 0;
        state = 0;
        duration = parent.battleIdleTransferDuration;
    }
}