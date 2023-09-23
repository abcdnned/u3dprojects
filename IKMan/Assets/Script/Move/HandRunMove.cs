using System;
using UnityEngine;
using static IKSampleNames;
public class HandRunMove : LegHandRunMove
{
    // private GameObject anchor;
    // private GameObject spring;
    
    // private Vector3 offset;





    String[] names = new string[] { HAND_RUN_1, HAND_RUN_2, HAND_RUN_3 };

    public HandRunMove() : base(MoveNameConstants.HandRunMove)
    {
    }
    protected override string[] getNames() {
        return names;
    }

    protected override void updateIKRotation() {
        handController().LookToArmLook();
    }

    // public override void init() {
    //     base.init();
    //     // Anchor
    //     Transform parent = twoNodeController().IsRightPart()
    //                        ? handController().hic.rightHand.advanceIKController.shoulder
    //                        : handController().hic.leftHand.advanceIKController.shoulder;
    //     anchor = PrefabCreator.CreatePrefab(parent.position, "SpinHelper");
    //     anchor.AddComponent<SphereCollider>();
    //     Rigidbody rigidbody = anchor.GetComponent<Rigidbody>();
    //     rigidbody.isKinematic = true;
    //     rigidbody.useGravity = false;
    //     anchor.transform.SetParent(handController().hic.spin3.transform);
    //     // Spring
    //     spring = PrefabCreator.CreatePrefab(parent.position, "SpinHelper");
    //     spring.AddComponent<SphereCollider>();
    //     spring.GetComponent<Rigidbody>().useGravity = false;
    //     SpringJoint newJoint = spring.AddComponent<SpringJoint>();
    //     newJoint.anchor = Vector3.zero;
    //     newJoint.connectedBody = rigidbody;
    //     newJoint.autoConfigureConnectedAnchor = true;
    //     newJoint.spring = 1000;
    //     newJoint.damper = 10;
    //     Transform shoulder = twoNodeController().IsRightPart()
    //                        ? handController().hic.rightShoulder
    //                        : handController().hic.leftShoulder;
    //     offset = shoulder.position - spring.transform.position;
    // }

    // public override void finish() {
    //     if (anchor != null) {
    //         GameObject.Destroy(anchor);
    //     }
    //     if (spring != null) {
    //         GameObject.Destroy(spring);
    //     }
    // }

    // public override Move move(float dt) {
    //     normalizedTime += dt;
        // Transform shoulder = twoNodeController().IsRightPart()
        //                    ? handController().hic.rightShoulder
        //                    : handController().hic.leftShoulder;
        // shoulder.position = offset + spring.transform.position;
    //     return this;
    // }
}
