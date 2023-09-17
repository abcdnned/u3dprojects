
using UnityEngine;

public class HipMove : Move
{
    protected WalkBalance controller;

    public HipMove(string name) : base(name) {

    }
    public override void init() {
        controller = (WalkBalance)targetController;
        normalizedTime = 0;
        state = 0;
        duration = controller.battleIdleTransferDuration;
    }

}