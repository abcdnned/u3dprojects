using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUtils
{
    public static void drawBall(Vector3 center)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = center;
        float radius = 0.5f;
        sphere.transform.localScale = new Vector3(radius, radius, radius);
    }
}
