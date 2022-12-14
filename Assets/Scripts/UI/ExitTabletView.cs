using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitTabletView : MonoBehaviour, InteractableObject
{
	[SerializeField]
	CinemachineVirtualCamera cam;
	[SerializeField]
	GameObject enterTrigger;

	private void OnEnable()
	{
		InputsManager inputsManager = InputsManager.Instance;
		inputsManager.EnableInputsByType(InputsManager.InputsType.Tablet);
		inputsManager.TabletControls.TabletView.Exit.performed += ctx => DoInteraction(true);
		inputsManager.TabletControls.TabletView.Click.performed += ctx => DoRaycast();
	}
	private void OnDisable()
	{
		InputsManager inputsManager = InputsManager.Instance;
		inputsManager.TabletControls.TabletView.Click.performed += ctx => DoRaycast();
		inputsManager.TabletControls.TabletView.Exit.performed -= ctx => DoInteraction(true);
		inputsManager.DisableInputsByType(InputsManager.InputsType.Tablet);
	}

	public void DoInteraction(bool primary)
	{
		cam.enabled = false;
		gameObject.SetActive(false);
	}

	private void DoRaycast()
	{
		Vector2 mousePos = Mouse.current.position.ReadValue();
		int layer_mask = LayerMask.GetMask("Interactable");
		Ray ray = Camera.main.ScreenPointToRay(mousePos);
		//do the raycast specifying the mask
		if (Physics.Raycast(ray, out RaycastHit hit, 3, layer_mask))
		{
			if (hit.transform.gameObject == gameObject)
			{
				DoInteraction(true);
			}
		}
	}
}
