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

	TabletViewControls controls;

	private void Awake()
	{
		controls = new TabletViewControls();
		controls.Disable();
	}

	private void OnEnable()
	{
		controls.Enable();
		controls.TabletView.Exit.performed += ctx => DoInteraction();
		controls.TabletView.Click.performed += ctx => DoRaycast();
	}
	private void OnDisable()
	{
		controls.TabletView.Click.performed += ctx => DoRaycast();
		controls.TabletView.Exit.performed -= ctx => DoInteraction();
		controls.Disable();
	}

	public void DoInteraction()
	{
		InputsManager.Instance.EnablePlayerMovement();
		cam.enabled = false;
		enterTrigger.SetActive(true);
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
				DoInteraction();
			}
		}
	}
}
