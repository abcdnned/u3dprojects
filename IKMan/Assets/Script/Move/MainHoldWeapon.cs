using UnityEngine;

public class MainHoldWeapon : HandMove
{
    protected override void subinit() {
        handController.SyncIKSample(IKSampleNames.FETCH_GREAT_SWORD_2, .1f);
    }
    
    public MainHoldWeapon() : base(MoveNameConstants.MainHoldWeaponIdle)
    {
    }

    public override Move move(float dt)
    {
        return this;
    }
    public override string getMoveType() {
        return AdvanceIKController.FIK;
    }
}
