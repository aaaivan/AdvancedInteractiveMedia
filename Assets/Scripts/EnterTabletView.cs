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
		DisablePlayerMovement();
		cam.enabled = true;
		exitTrigger.SetActive(true);
	}
	private void DisablePlayerMovement()
	{
		FirstPersonController fpc = InputsManager.Instance.firstPersonController;
		GameObject player = fpc.gameObject;
		StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
		if (Input.mousePresent)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			inputs.cursorLocked = false;
			inputs.cursorInputForLook = false;
		}

		fpc.DisableGameInputs();
	}
}
