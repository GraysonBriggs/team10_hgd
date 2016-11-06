﻿using UnityEngine;
using System.Collections;

public class RotateTowardsInput : MonoBehaviour {

    Vector3 inputVector;
	Vector3 pointOfRotation;
	bool isFlipped;
	float aimPosX, aimPosY;

	public float extendDistance;

	// Use this for initialization
	void Start () {	}
	
	// Update is called once per frame
	void Update () {
		isFlipped = transform.root.localScale.x < 1;

		if (Input.GetAxis ("R_XAxis_1") != 0 || Input.GetAxis ("R_YAxis_1") != 0) {
			aimPosX = Input.GetAxis ("R_XAxis_1");
			aimPosY = Input.GetAxis ("R_YAxis_1");
		} else {
			aimPosX = transform.localScale.x;
			aimPosY = 0;
		}

		pointOfRotation = transform.parent.position;
		pointOfRotation.z = 0;

		Vector3 inputPos = new Vector3 (aimPosX + pointOfRotation.x, aimPosY + pointOfRotation.y, pointOfRotation.z);
		// To re-enable mousePos to aim
		// inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// inputPos.z = 0;
		inputVector = inputPos - pointOfRotation;

		repositionArm ();
		rotate ();
	}

    public void rotate()
    {
		float angle = Mathf.Atan2(aimPosY, aimPosX) * Mathf.Rad2Deg * -1;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

		float scaleX, scaleY;

		if (isFlipped)
			scaleX = -Mathf.Abs(transform.localScale.x);
		else
			scaleX = Mathf.Abs(transform.localScale.x);
		
		if (Mathf.Abs(angle) > 90f)
			scaleY = -Mathf.Abs(transform.localScale.y);
        else
			scaleY = Mathf.Abs(transform.localScale.y);

		transform.localScale = new Vector2 (scaleX, scaleY);
    }

	void repositionArm()
	{
		if (extendDistance != 0) {
			Vector3 vec = inputVector.normalized;
			if (isFlipped)
				vec.x *= -1;
			// Uhhh this is done because of weird math.  Probably need to look into this later
			// -> for a better implementation of aiming with the joystick
			vec.y *= -1;
			transform.localPosition = vec * extendDistance;
		}
	}
}
