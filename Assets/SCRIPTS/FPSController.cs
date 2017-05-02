using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GroundState { Grounded, Falling, Jumping };
public enum LeanState { Left, None, Right};

[RequireComponent(typeof (CharacterController))]
[RequireComponent(typeof (Rigidbody))]
public class FPSController : MonoBehaviour 
{
	[Header ("Inputs")]
	public string horizontalMovement = "Horizontal";
	public string verticalMovement = "Vertical";
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode leftLeanInput = KeyCode.A;
	public KeyCode rightLeanInput = KeyCode.E;

	[Header ("Velocity")]
	public float currentVelocity;

	[Header ("Movement")]
	public float movementSpeed;
	public float sprintSpeed;

	[Header ("Jump")]
	public LayerMask groundLayer;
	public GroundState groundState = GroundState.Grounded;
	public float jumpForce;

	[Header ("Gravity")]
	public float gravityForce = 10;

	[Header ("Lean")]
	public LeanState leanState = LeanState.None;
	public float leanMaxAngle;
	public float leanSpeed;
	public float leanResetSpeed;

	private Rigidbody _rigidbody;
	private Vector3 _movement;
	private Transform _mainCamera;
	private Transform _bobbingParent;
	private CharacterController _characterController;

	void Start () 
	{
		_characterController = GetComponent<CharacterController>();
		_rigidbody = GetComponent<Rigidbody> ();
		_characterController = GetComponent<CharacterController> ();
		_mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		_bobbingParent = _mainCamera.parent;
	}
	
	void Update () 
	{
		GetInputs ();

		currentVelocity = _rigidbody.velocity.magnitude;
	}

	void GetInputs ()
	{
		if (Input.GetKeyDown (jumpKey) && groundState == GroundState.Grounded)
			Jump ();
		
		Lean ();
	}

	void FixedUpdate () 
	{
		Gravity ();
		//Movement ();

		// always move along the camera forward as it is the direction that it being aimed at
		Vector3 desiredMove = transform.forward * Input.GetAxisRaw (verticalMovement) + transform.right * Input.GetAxisRaw (horizontalMovement);

		// get a normal for the surface that is being touched to move along it
		RaycastHit hitInfo;
		Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out hitInfo,
			_characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
		
		desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

		desiredMove.x *= movementSpeed;
		desiredMove.y *= movementSpeed;

		if(_characterController.isGrounded)
		{
			
		}
	}

	void Movement ()
	{
		_movement = new Vector3 (Input.GetAxisRaw (horizontalMovement), 0, Input.GetAxisRaw (verticalMovement));
		_movement = _mainCamera.TransformDirection (_movement);

		if (_movement.magnitude > 1)
			_movement.Normalize ();

		if(Input.GetKey (sprintKey) && groundState == GroundState.Grounded)
			_movement *= sprintSpeed;
		else
			_movement *= movementSpeed;


		Vector3 velocityChange = (_movement - _rigidbody.velocity);
		velocityChange.y = 0;

		//_rigidbody.AddForce (velocityChange, ForceMode.VelocityChange);

		_characterController.Move (_movement * Time.fixedDeltaTime);
	}

	void Gravity ()
	{
		_rigidbody.AddForce (Vector3.down * gravityForce, ForceMode.Force);
	}

	void Jump ()
	{
		groundState = GroundState.Jumping;
		_rigidbody.velocity = new Vector3 (0, jumpForce, 0);
	}

	void Lean ()
	{
		if(Input.GetKey (rightLeanInput) && leanState != LeanState.Right)
		{
			DOTween.Kill ("Lean");
			leanState = LeanState.Right;
			_bobbingParent.DOLocalRotate (new Vector3(0, 0, -leanMaxAngle), leanSpeed).SetSpeedBased ().SetId ("Lean");
		}
		else if(Input.GetKey (leftLeanInput) && leanState != LeanState.Left)
		{
			DOTween.Kill ("Lean");
			leanState = LeanState.Left;
			_bobbingParent.DOLocalRotate (new Vector3(0, 0, leanMaxAngle), leanSpeed).SetSpeedBased ().SetId ("Lean");
		}
		else if(!Input.GetKey (rightLeanInput) && !Input.GetKey (leftLeanInput) && leanState != LeanState.None)
		{
			DOTween.Kill ("Lean");
			leanState = LeanState.None;
			_bobbingParent.DOLocalRotate (new Vector3(0, 0, 0), leanResetSpeed).SetSpeedBased ().SetId ("Lean");
		}
	}

	public void Grounded ()
	{
		groundState = GroundState.Grounded;
	}

	public void InAir ()
	{
		if (groundState == GroundState.Grounded)
			groundState = GroundState.Falling;
	}
}
