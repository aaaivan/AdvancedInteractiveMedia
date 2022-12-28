using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
	Animator doorAnimator;
	int counter = 0;
	private void Awake()
	{
		doorAnimator = GetComponent<Animator>();
	}

	private void OnTriggerEnter(Collider other)
	{
		counter++;
		doorAnimator.SetBool("Open", true);
	}

	private void OnTriggerExit(Collider other)
	{
		counter--;
		if(counter <= 0)
		{
			counter = 0;
			doorAnimator.SetBool("Open", false);
		}
	}
}
