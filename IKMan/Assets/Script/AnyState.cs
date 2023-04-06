using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnyState : ActionStateMachine
{
    // protected Banner stopWalkingBanner;
    protected Banner keepWalkingBanner;
    protected Banner idleBanner;
    public AnyState(HumanIKController humanIKController) : base(humanIKController)
    {
        // stopWalkingBanner = new Banner(Banner.STOP_WALKING);
        initState();
    }
    protected virtual void initState() {
    }
    public override ActionStateMachine handleEvent(Event e) {
        (States, ActionStateMachine) value = cs.handleEvent(e);
        if (value.Item1 != null && cs != value.Item1) {
            Debug.Log(this.GetType().Name + " states change to " + value.Item1.name);
            cs = value.Item1;
        }
        return value.Item2;
    }
}