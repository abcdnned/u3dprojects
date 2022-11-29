using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TY {

public class EnemyManager : MonoBehaviour
{
    public bool isPerformingAction;

    EnemyLocomotionManager enemyLocomotionManager;

    [Header("AI Setting")]
    public float detectionRadius = 20;
    public float maximumDetectionAngle = 50;
    public float minimumDectionAngle = -50;

    public bool isGrounded = true;

    public bool chasing = false;

    private void Awake() {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();

    }

    private void Update() {
        enemyLocomotionManager.HandleFalling(Time.deltaTime);
    }

    private void FixedUpdate() {
        // HandleCurrentAction();
    }
    private void HandleCurrentAction() {
         if (enemyLocomotionManager.currentTarget == null) {
            enemyLocomotionManager.HandleDetection();
         } else {
            enemyLocomotionManager.HandleMoveToTarget();
         }
    }
}

}