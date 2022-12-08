using Cinemachine;
using Fluent;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTabletView : MonoBehaviour, InteractableObject
{
	[SerializeField]
	CinemachineVirtualCamera cam;
	[SerializeField]
	GameObject exitTrigger;
	public void DoInteraction()
	{
		InputsManager.Instance.DisablePlayerMovement();
		cam.enabled = true;
		exitTrigger.SetActive(true);
	}
}
