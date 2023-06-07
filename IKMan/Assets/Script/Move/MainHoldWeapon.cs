using UnityEngine;

public class MainHoldWeapon : HandMove
{
    protected override void subinit() {
        handController.handHint.hAd = 90f;
        handController.handHint.horizonAngel = 90f;
    }
    
    public MainHoldWeapon() : base(MoveNameConstants.MainHoldWeaponIdle)
    {
    }

    public override Move move(float dt)
    {
        return this;
    }
}
