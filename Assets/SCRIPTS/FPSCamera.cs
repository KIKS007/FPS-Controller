using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour 
{
	[Header ("Mouse Inputs")]
	public string horizontalMouseAxis = "Mouse X";
	public string verticalMouseAxis = "Mouse Y";

	[Header ("Sensitivity")]
	public float verticalSensitivity = 15;
	public float horizontalSensitivity = 15;

	[Header ("Vertical Rotation")]
	[Range (0, -120)]
	public float upwardsRotationLimit = -90;
	[Range (0, 120)]
	public float downwardsRotationLimit = 90;

	[Header ("Bobbing")]
	public float bobbingMaxHeight;
	public float bobbingSpeed = 1;

	private Transform _FPSCamera;
	private Quaternion _initialRotation;
	private Quaternion _thisInitialRotation;
	private Transform _bobbingParent;
	private FPSController _FPSController;

	private float _previousFrequency = 0.5f;
	private float _frequency;
	private float phase = 0.0f;

	private float yRotation;
	//private float xRotation;

	// Use this for initialization
	void Start () 
	{
		_FPSCamera = transform.GetComponentInChildren<Camera> ().transform;
		_initialRotation = _FPSCamera.localRotation;
		_thisInitialRotation = transform.localRotation;
		_bobbingParent = _FPSCamera.parent;
		_FPSController = GetComponent<FPSController> ();

		LockMouse ();
	}

	void LockMouse ()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		RotateCamera ();
		Bobbing ();
	}

	void RotateCamera ()
	{
		float xRotation = Input.GetAxis (horizontalMouseAxis) * horizontalSensitivity;

		yRotation += Input.GetAxis (verticalMouseAxis) * verticalSensitivity;
		yRotation = ClampAngle (yRotation, upwardsRotationLimit, downwardsRotationLimit);

		transform.Rotate (new Vector3(0, xRotation, 0));

		//transform.localRotation = _thisInitialRotation * Quaternion.AngleAxis (xRotation, Vector3.up);
		_FPSCamera.localRotation = _initialRotation * Quaternion.AngleAxis (yRotation, -Vector3.right);
	}

	void Bobbing ()
	{
		if (_FPSController.groundState != GroundState.Grounded)
			return;

		_previousFrequency = bobbingSpeed * (_FPSController.currentVelocity / _FPSController.movementSpeed);

		if (_previousFrequency != _frequency) 
			CalcNewFreq();

		Vector3 position = _bobbingParent.localPosition;
		position.y = Mathf.Sin (Time.time * _frequency + phase) * bobbingMaxHeight;

		_bobbingParent.localPosition = position;
	}



	void CalcNewFreq() {
		float curr = (Time.time * _frequency + phase) % (2.0f * Mathf.PI);
		float next = (Time.time * _previousFrequency) % (2.0f * Mathf.PI);
		phase = curr - next;
		_frequency = _previousFrequency;
	}

	float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		
		if (angle > 360F)
			angle -= 360F;
		
		return Mathf.Clamp (angle, min, max);
	}
}
