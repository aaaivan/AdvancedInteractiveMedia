using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTabletView : MonoBehaviour, InteractableObject
{
	[SerializeField]
	CinemachineVirtualCamera cam;
	[SerializeField]
	GameObject exitTrigger;
	public void DoInteraction(bool primary)
	{
		cam.enabled = true;
		exitTrigger.SetActive(true);
	}
}
