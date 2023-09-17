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
    }


    internal override void init() {
        if (movingSphere == null) {
            Vector3 p = Utils.copy(target.transform.position);
            p.y = 0.55f;
            movingSphere = PrefabCreator.CreatePrefab(p, "MovingSphere").GetComponent<MovingSphere>();
            offset = target.transform.position - movingSphere.transform.position;
        }
    }

    internal override void exit() {
        if (movingSphere != null) {
            GameObject.Destroy(movingSphere);
            movingSphere = null;
        }
    }
}