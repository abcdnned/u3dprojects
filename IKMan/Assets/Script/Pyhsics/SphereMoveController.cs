using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMoveController : DeltaMoveController
{

    private Vector3 offset;
    private MovingSphere movingSphere;


    internal override void deltaMove() {
        movingSphere.updateInput(ia.movement, hic.walkPointer.cam, ia.jumpFlag);
        Utils.deltaMove(target.transform, movingSphere.transform.position + offset);
        // Utils.deltaMove(target.transform, movingSphere.transform.position);
    }


    internal override void init() {
        if (movingSphere == null) {
            Vector3 p = Utils.copy(target.transform.position);
            p.y = p.y + 0.5f;
            movingSphere = PrefabCreator.CreatePrefab(p, "MovingSphere", hic.walkPointer.transform.rotation).GetComponent<MovingSphere>();
            movingSphere.getSpeed = getSpeed;
            offset = target.transform.position - movingSphere.transform.position;
        }
    }

    internal override void exit() {
        if (movingSphere != null) {
            GameObject.Destroy(movingSphere.gameObject);
            movingSphere = null;
            // Debug.Log(" moving sphere destroy ");
        }
    }

    internal float getSpeed() {
        return hic.runMaxSpeed;
    }
    internal override Vector3 getVelocity() {
        return movingSphere.body.velocity;
    }

    internal override int onGround() {
        if (movingSphere.OnGround) {
            return 1;
        } else {
            return 0;
        }
    }
}