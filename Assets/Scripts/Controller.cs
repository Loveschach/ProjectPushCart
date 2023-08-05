using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
	// Walk constants
	[Header( "Walk Constants" )]
	public bool ACCEL_ENABLED = true;
	public bool DECEL_ENABLED = true;
	public float GROUND_SPEED = 3;
	public float GROUND_ACCEL = 0.2f;
	public float GROUND_DECEL = 0.25f;

	// Components
	public Rigidbody body;

	public InputAction moveAction;
	float verticalInput;
	float horizontalInput;

	private void Start() {
		moveAction.Enable();
	}

	void UpdateInput() {
		horizontalInput = moveAction.ReadValue<Vector2>().x;
		verticalInput = moveAction.ReadValue<Vector2>().y;
	}

	void UpdateMovement() {
		Vector3 forward = transform.forward * verticalInput;
		Vector3 right = transform.right * horizontalInput;
		Vector3 move = Vector3.Normalize( forward + right );
		Vector3 adjustedMove = move * GROUND_SPEED * Time.deltaTime;
		if ( horizontalInput == 0 && verticalInput == 0 ) {
			body.velocity = new Vector3( 0, body.velocity.y, 0 );
		}
		else {
			body.MovePosition( transform.position + adjustedMove );
		}
	}

	void Update() {
		UpdateInput();
	}

	private void FixedUpdate() {
		UpdateMovement();
	}
}