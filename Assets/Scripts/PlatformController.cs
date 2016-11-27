﻿using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 objScale = transform.lossyScale;
		transform.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2(objScale.x, objScale.y);
	}
}
