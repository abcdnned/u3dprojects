using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TY {

public class EnemyLocomotionManager : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimatorManager enemyAnimatorManager;

    public NavMeshAgent navMeshAgent;

    public Rigidbody enemyRigidBody;
    public float distanceFromTarget;
    public float stoppingDistance = 10f;

    public float chasingDistance = 20f;

    public float rotationSpeed = 15;

    public CharacterStats currentTarget;
    public LayerMask detectionLayer;
    public new Rigidbody rigidbody;

    internal bool fly = false;

    internal bool antiGravity = false;

    internal float flySpeed = 9;

    internal float flyHeight = 10;

    public float minimumDistanceNeededToBeginFall = 2.3f;
    public float groundDetectionRayStartPoint = 2f;

    private LayerMask ignoreForGroundCheck;

    private void Awake() {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidBody = GetComponent<Rigidbody>();
        rigidbody = GetComponent<Rigidbody>();
        ignoreForGroundCheck = 1 << 9 | 1 << 10;
    }

    private void Start() {
        navMeshAgent.enabled = false;
        enemyRigidBody.isKinematic = false;
    }
    public void HandleDetection() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++) {
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

            if (characterStats != null) {
                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
                if (viewableAngle > enemyManager.minimumDectionAngle && viewableAngle < enemyManager.maximumDetectionAngle) {
                    currentTarget = characterStats;
                }
            }
        }
    }
    
    public void HandleAI() {
    }

    public void HandleMoveToTarget() {
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        if (enemyManager.isPerformingAction) {
            // enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            // navMeshAgent.enabled = false;
        } else {
            if (!enemyManager.chasing && distanceFromTarget > chasingDistance) {
                enemyManager.chasing = true;
            }
            if (enemyManager.chasing && distanceFromTarget > stoppingDistance) {
                enemyManager.chasing = true;
            } else if (distanceFromTarget <= stoppingDistance) {
                enemyManager.chasing = false;
            }
            if (enemyManager.chasing) {
                Vector3 projectedVelocity = transform.forward * 5;
                projectedVelocity.y = 0;
                enemyRigidBody.velocity = projectedVelocity;
                enemyAnimatorManager.SetWanderStat(true);
            } else {
                enemyAnimatorManager.SetWanderStat(false);
            }
        }

        HandleRotateTowardsTarget();

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;
    }

    private void HandleRotateTowardsTarget() {
        // Debug.Log("HandlerRotateTowards");
        if (enemyManager.isPerformingAction) {
            Vector3 direction = currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            if (direction == Vector3.zero) {
                direction = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed);
        } else {
            Vector3 relativeDirection = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyRigidBody.velocity;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(currentTarget.transform.position);
            enemyRigidBody.velocity = targetVelocity;
            transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
            // Debug.Log("finish else");
        }

    }

    public void HandleFalling(float delta) {
        RaycastHit hit;
        Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        // if(Physics.Raycast(origin, transform.forward, out hit, 0.4f)) {
        //     moveDirection = Vector3.zero;
        // }

        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.0f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck)) {
            Vector3 normalVector = hit.normal;
            Vector3 tp = hit.point;
            if (enemyManager.isGrounded == false) {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            }
            enemyManager.isGrounded = true;
        } else if (!antiGravity) {
            enemyManager.isGrounded = false;
            rigidbody.AddForce(-Vector3.up * (Constants.FALLING_G * rigidbody.mass));
        }

        if (fly && transform.position.y < flyHeight) {
            transform.position = new Vector3(transform.position.x,
                                             transform.position.y + flySpeed * Time.deltaTime,
                                             transform.position.z);
        }

        // Vector3 dir = moveDirection;
        // dir.Normalize();
        // origin = origin + dir * groundDirectionRayDistance;

        // targetPosition = myTransform.position;

    }
    
}

}