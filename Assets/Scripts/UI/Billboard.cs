using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	Camera cam;

	private void Awake()
	{
		cam = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		transform.LookAt(transform.position + cam.transform.forward);
	}
}
