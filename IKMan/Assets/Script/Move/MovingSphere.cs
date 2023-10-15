using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingSphere : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
	float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
	float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField]
	Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

	[SerializeField, Range(0f, 10f)]
	float jumpHeight = 2f;

    [SerializeField, Range(0, 5)]
	int maxAirJumps = 0;

    [SerializeField, Range(0f, 100f)]
	float maxSnapSpeed = 100f;

    Vector3 velocity, desiredVelocity;

	internal Rigidbody body;

    int jumpPhase;

    bool desiredJump;
	int groundContactCount, steepContactCount;

	internal bool OnGround => groundContactCount > 0;
	internal bool OnSteep => steepContactCount > 0;

	[SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f, maxStairsAngle = 50f;
	float minGroundDotProduct, minStairsDotProduct;
	[SerializeField, Min(0f)]
	float probeDistance = 1f;
    Vector3 contactNormal, steepNormal;
    int stepsSinceLastGrounded, stepsSinceLastJump;
	[SerializeField]
	LayerMask probeMask = -1, stairsMask = -1;
	internal Func<float> getSpeed;
	internal Transform direction;






	// public HumanIKController humanIKController;

	void OnValidate () {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
	}

	void Awake () {
		body = GetComponent<Rigidbody>();
		OnValidate();
	}

	internal void updateInput (Vector2 movement, Transform direction, bool jumpFlag) {
		this.direction = direction;
		Vector2 playerInput;
        playerInput.x = movement.x;
		playerInput.y = movement.y;
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
		Vector3 forward = direction.forward;
		forward.y = 0f;
		forward.Normalize();
		Vector3 right = direction.right;
		right.y = 0f;
		right.Normalize();
		float speed = getSpeed == null ? maxSpeed : getSpeed();
		desiredVelocity = 
			(forward * playerInput.y + right * playerInput.x)  * speed;
        //Jump
        desiredJump |= jumpFlag;
		// GetComponent<Renderer>().material.SetColor(
		// 	"_Color", OnGround ? Color.black : Color.white
		// );
    }

	
	void Jump() {
		Vector3 jumpDirection;
		if (OnGround) {
			jumpDirection = contactNormal;
		}
		else if (OnSteep) {
			jumpDirection = steepNormal;
			jumpPhase = 0;
		}
		else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) {
			if (jumpPhase == 0) {
				jumpPhase = 1;
			}
			jumpDirection = contactNormal;
		}
		else {
			return;
		}
		stepsSinceLastJump = 0;
		jumpPhase += 1;
		float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
		jumpDirection = (jumpDirection + Vector3.up).normalized;
		float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
		if (alignedSpeed > 0f) {
			jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
		}
		velocity += jumpDirection * jumpSpeed;
	}
	void FixedUpdate () {
		velocity = body.velocity;
        UpdateState();
		Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
		Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
		float currentX = Vector3.Dot(velocity, xAxis);
		float currentZ = Vector3.Dot(velocity, zAxis);
		float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;
		// float newX =
		// 	Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
		// float newZ =
		// 	Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
		Vector3 curV = new Vector3(currentX, 0, currentZ);
		Vector3 desV = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);
		Vector3 newV = Vector3.MoveTowards(curV, desV, maxSpeedChange);
		float newX = newV.x;
		float newZ = newV.z;
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        // velocity = desiredVelocity;
		if (desiredJump) {
			desiredJump = false;
			Jump();
		}
		// Debug.Log(" veloc " + velocity);
		body.velocity = velocity;
		ClearState();
    }

	void ClearState () {
        groundContactCount = steepContactCount = 0;
		contactNormal = steepNormal = Vector3.zero;
	}
	void UpdateState () {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
		if (OnGround || SnapToGround() || CheckSteepContacts()) {
			if (stepsSinceLastJump > 1) {
				jumpPhase = 0;
			}
            stepsSinceLastGrounded = 0;
            contactNormal.Normalize();
		}
        else {
			contactNormal = Vector3.up;
		}
	}
	void OnCollisionEnter (Collision collision) {
		//onGround = true;
		EvaluateCollision(collision);
	}

	void OnCollisionStay (Collision collision) {
		//onGround = true;
		EvaluateCollision(collision);
	}
	
	void EvaluateCollision (Collision collision) {
        float minDot = GetMinDot(collision.gameObject.layer);
        for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
				groundContactCount++;
				contactNormal += normal;
			}
			else if (normal.y > -0.01f) {
				steepContactCount += 1;
				steepNormal += normal;
			}
		}
    }
	Vector3 ProjectOnContactPlane (Vector3 vector) {
		return vector - contactNormal * Vector3.Dot(vector, contactNormal);
	}

	// void AdjustVelocity () {
	// 	Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
	// 	Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
	// }
    bool SnapToGround () {
		if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) {
			return false;
		}
		if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask)) {
			return false;
		}
		float speed = velocity.magnitude;
		if (speed > maxSnapSpeed) {
			return false;
		}
		if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer)) {
			return false;
		}
		groundContactCount = 1;
		contactNormal = hit.normal;
		float dot = Vector3.Dot(velocity, hit.normal);
        if (dot >= 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
		return true;
	}
	float GetMinDot (int layer) {
		return (stairsMask & (1 << layer)) == 0 ?
			minGroundDotProduct : minStairsDotProduct;
	}
	bool CheckSteepContacts () {
		if (steepContactCount > 1) {
			steepNormal.Normalize();
			if (steepNormal.y >= minGroundDotProduct) {
				groundContactCount = 1;
				contactNormal = steepNormal;
				return true;
			}
		}
		return false;
	}
}
