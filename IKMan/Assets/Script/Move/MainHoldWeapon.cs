using UnityEngine;

public class MainHoldWeapon : HandMove
{

    protected override void subinit() {
        Debug.Log(" MainHoldeWeapon ");
        Vector3 readyPos = handController.humanIKController.weaponReady.transform.position;
        Vector3 spin1 = handController.humanIKController.spin1.transform.position;
        GameObject polePivot = PrefabCreator.CreatePrefab(new Vector3(spin1.x, readyPos.y, spin1.z),
                               "WeaponHandler");
        polePivot.transform.rotation = Quaternion.LookRotation(humanIKController.transform.forward,
                                                      humanIKController.transform.up);
        polePivot.transform.parent = humanIKController.spin1.transform;
        GameObject pole = PrefabCreator.CreatePrefab(new Vector3(spin1.x, readyPos.y, spin1.z),
                               "Pole");
        float zRotate = Vector3.Angle(pole.transform.forward, polePivot.transform.forward);
        pole.transform.eulerAngles = new Vector3(90, 0, -zRotate);
        CharacterJoint newJoint = pole.AddComponent<CharacterJoint>();
        newJoint.connectedBody = polePivot.GetComponent<Rigidbody>();
        newJoint.anchor = new Vector3(0,0,0);
        newJoint.autoConfigureConnectedAnchor = false;
        newJoint.axis = new Vector3(0,0,-1);
        newJoint.swingAxis = new Vector3(-1,0,0);
        SoftJointLimitSpring twistSpring = new SoftJointLimitSpring();
        twistSpring.spring = MoveConstants.spring;
        twistSpring.damper = MoveConstants.damper;
        newJoint.twistLimitSpring = twistSpring;
        SoftJointLimit l = new SoftJointLimit();
        l.limit = -45;
        newJoint.lowTwistLimit = l;
        SoftJointLimit h = new SoftJointLimit();
        h.limit = -42;
        newJoint.highTwistLimit = h;
        SoftJointLimit s1 = new SoftJointLimit();
        s1.limit = 0f;
        newJoint.swing1Limit = s1;
        SoftJointLimit s2 = new SoftJointLimit();
        s2.limit = 0f;
        newJoint.swing2Limit = s2;

        pole.AddComponent<EnhanceCharacterJoint>();
        humanIKController.poleJoint = newJoint;

        GameObject hand = handController.gameObject;
        HandDelayLooker handDelayLooker = hand.AddComponent<HandDelayLooker>();
        handDelayLooker.sun = pole.transform;
        handDelayLooker.direction = pole.transform;
        handDelayLooker.distance = humanIKController.swingRadius;
        handDelayLooker.horizonAngel = 0;
        handDelayLooker.hAd = 0;
        handDelayLooker.verticalAngel = 90;
        handDelayLooker.vAd = 90;
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
