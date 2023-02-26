using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnyState : ActionStateMachine
{
    protected Banner stopWalkingBanner;
    protected Banner keepWalkingBanner;
    protected Banner idleBanner;
    public AnyState(HumanIKController humanIKController) : base(humanIKController)
    {
        stopWalkingBanner = new Banner();
        keepWalkingBanner = new Banner();
        idleBanner = new Banner();
    }
}