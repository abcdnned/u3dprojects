using System;
using UnityEngine;

public class HandMain2Battle : HandMove
{
    protected float duration2;

    protected GameObject greateSworad;

    internal GameObject hand;

    public HandMain2Battle() : base(MoveNameConstants.HandMain2Battle)
    {
    }

    public override string getMoveType() {
        return AdvanceIKController.FK;
    }
    public void initBasic(float duration,
                          float duration2,
                          GameObject gs,
                          GameObject hand) {
        this.duration = duration;
        this.duration2 = duration2;
        this.greateSworad = gs;
        this.hand = hand;
    }

    public override Move move(float dt) {
        normalizedTime += dt;
        if (state == 0) {
            state = 1;
            handController.SyncIKSample(IKSampleNames.FETCH_GREAT_SWORD_1, duration);
            if (handController.handLookIKController != null) {
                handController.handLookIKController.init(duration,
                                                        hic.mainHandle.position,
                                                        hic.body.transform);
            }
        }
        if (state == 1) {
            handController.LookToArmLook();
            if (normalizedTime > duration) {
                state = 2;
                handController.handLookIKController.transferCurPosToLv1();
                handController.SyncIKSample(IKSampleNames.FETCH_GREAT_SWORD_2, duration2);
                if (handController.handLookIKController != null) {
                    handController.handLookIKController.init(duration2,
                                                            hic.weaponReady.transform.position,
                                                            hic.body.transform);
                }
                switchJoint(greateSworad, hand);
                handController.twistTwist(new Vector3(0, 106, 0), duration2);
            } else {
            }
        }
        if (state == 2) {
            handController.LookToArmLook();
            if (normalizedTime > duration + duration2) {
                handController.handLookIKController.transferCurPosToLv1();
                state = 3;
                return moveManager.ChangeMove(MoveNameConstants.MainHoldWeaponIdle);
            }
        }
        return this;
    }

    private void adjustSwordPositionWithHand(GameObject hand, GameObject gs) {
        Vector3 direction = Utils.up(hand.transform);
        Quaternion quaternion = Quaternion.LookRotation(direction, Utils.forward(hand.transform));
        gs.transform.rotation = quaternion;
        // Debug.Log(" adjust sword ");
    }

    private void switchJoint(GameObject gs, GameObject hand) {
        CharacterJoint characterJoint = gs.GetComponent<CharacterJoint>();
        GameObject.Destroy(characterJoint);
        adjustSwordPositionWithHand(hand, gs);
        CharacterJoint newJoint = gs.AddComponent<CharacterJoint>();
        newJoint.anchor = hic.mainHandle.localPosition;
        newJoint.connectedBody = hand.GetComponent<Rigidbody>();
        newJoint.autoConfigureConnectedAnchor = false;
        newJoint.axis = new Vector3(1,0,0);
        newJoint.swingAxis = new Vector3(0,0,-1);
        SoftJointLimit l = new SoftJointLimit();
        l.limit = -5;
        newJoint.lowTwistLimit = l;
        SoftJointLimit h = new SoftJointLimit();
        h.limit = 5;
        newJoint.highTwistLimit = h;
        SoftJointLimit s1 = new SoftJointLimit();
        s1.limit = 20f;
        newJoint.swing1Limit = s1;
        SoftJointLimit s2 = new SoftJointLimit();
        s2.limit = 0f;
        newJoint.swing2Limit = s2;
    }
}
