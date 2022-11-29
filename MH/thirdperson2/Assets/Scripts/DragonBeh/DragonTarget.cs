using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TY {
public class DragonTarget : MonoBehaviour
{
    public float detectionRadius = 20;
    public LayerMask detectionLayer;
    public CharacterStats currentTarget;
    public float maximumDetectionAngle = 50;
    public float minimumDectionAngle = -50;
    public float distanceFromTarget;

    internal float chasingDistance = 30f;

    internal float tracingDistanceBuffer = 5f;

    internal float backingDistance = 7.6f;

    internal float rotationSpeed = 5;

    internal string attackType;

    public GameObject biteCol;

    public GameObject tailCol;

    public GameObject tailBigCol;

    public ParticleSystem fireBall;

    public Transform headTransform;

    public Transform[] tailTransform;

    public static string ATTACK_NONE = "AttackNone";

    public static string ATTACK_BITE = "AttackBite";
}
}