using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalConfigSpace : MonoBehaviour {

    public const int SPACE_SIZE = 1000;
    private Vector2[] spacePoint = new Vector2[SPACE_SIZE];
    internal int usedSpaceCount = 0;

    Dictionary<int, int> idToIndex = new Dictionary<int, int>();

    private void Start() {
    }

    private void Update() {
    }

    public Vector2 getSpace(int instanceId) {
        float x = transform.position.x + 1 + usedSpaceCount * 1 + .5f;
        float y = transform.position.z;
        spacePoint[usedSpaceCount] = new Vector2(x, y);
        idToIndex[instanceId] = usedSpaceCount;
        usedSpaceCount++;
        return new Vector2(x, y);
    }


}