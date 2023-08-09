using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
	// Drive constants
	[Header( "Drive Constants" )]
	public bool ACCEL_ENABLED = true;
	public float MAX_GROUND_SPEED = 0.3f;
	public float MAX_REVERSE_SPEED = 0.1f;
	public float GROUND_ACCEL = 0.2f;
	public float GROUND_DECEL = 0.25f;
	public float BRAKE_DECEL = 0.8f;
	public float ROTATIONAL_SPEED = 90f;

	// Components
	public Rigidbody body;

	// Input
	public InputAction moveAction;
	float verticalInput;
	float horizontalInput;

	// Drive Members
	float currentSpeed;

	private void Start() {
		moveAction.Enable();
		currentSpeed = 0;
	}

	void UpdateInput() {
		horizontalInput = moveAction.ReadValue<Vector2>().x;
		verticalInput = moveAction.ReadValue<Vector2>().y;
	}

	void UpdateMovement() {
		Vector3 forward = transform.forward * verticalInput;
		Vector3 right = transform.right * horizontalInput;
		Vector3 move = Vector3.Normalize( forward + right );
		Vector3 adjustedMove = move * MAX_GROUND_SPEED * Time.deltaTime;
		if ( horizontalInput == 0 && verticalInput == 0 ) {
			body.velocity = new Vector3( 0, body.velocity.y, 0 );
		}
		else {
			body.MovePosition( transform.position + adjustedMove );
		}
	}

	void UpdateAccelerator() {
		float accelerationConstant = GROUND_ACCEL;
		if ( verticalInput < 0 && body.velocity.z > 0 ) {
			accelerationConstant = BRAKE_DECEL;
		}
		else if ( verticalInput == 0 ) {
			accelerationConstant = GROUND_DECEL;
		}
		float targetSpeed = Mathf.Max( Mathf.Min( currentSpeed + ( accelerationConstant * verticalInput * Time.deltaTime ), MAX_GROUND_SPEED ), -MAX_REVERSE_SPEED );
		currentSpeed = targetSpeed;
		Vector3 move = transform.forward * targetSpeed;
		body.MovePosition( transform.position + move );
	}

	void UpdateRotation() {
		float newAngle = horizontalInput * ROTATIONAL_SPEED * Time.deltaTime;
		Quaternion rotQuat = Quaternion.AngleAxis( newAngle, Vector3.up );
		transform.rotation *= rotQuat;
	}

	void Update() {
		UpdateInput();
	}

	private void FixedUpdate() {
		if ( ACCEL_ENABLED ) {
			UpdateAccelerator();
			UpdateRotation();
		}

		else {
			UpdateMovement();
		}	
	}
}