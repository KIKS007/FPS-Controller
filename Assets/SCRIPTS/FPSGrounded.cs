using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSGrounded : MonoBehaviour 
{
	private FPSController _FPSController;

	// Use this for initialization
	void Start () 
	{
		_FPSController = transform.GetComponentInParent<FPSController> ();
	}

	void OnTriggerEnter (Collider collider)
	{
		if((_FPSController.groundLayer.value & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
		{
			if (_FPSController.groundState != GroundState.Grounded)
				_FPSController.Grounded ();
		}
	}

	void OnTriggerStay (Collider collider)
	{
		if((_FPSController.groundLayer.value & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
		{
			if (_FPSController.groundState != GroundState.Grounded)
				_FPSController.Grounded ();
		}
	}

	void OnTriggerExit (Collider collider)
	{
		if((_FPSController.groundLayer.value & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
		{
			if (_FPSController.groundState == GroundState.Grounded)
				_FPSController.InAir ();
		}
	}
}
