using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsManager : MonoBehaviour
{
	FirstPersonController firstPersonController;


	static InputsManager instance;
	public static InputsManager Instance
	{
		get
		{
			if (instance == null)
				throw new UnityException("You need to add an InputsManager to your scene");
			return instance;
		}
	}

	private void OnDestroy()
	{
		instance = null; 
	}

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
			firstPersonController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}

	public void EnablePlayerMovement()
	{
		GameObject player = firstPersonController.gameObject;
		StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
		if (Input.mousePresent)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			inputs.cursorLocked = true;
			inputs.cursorInputForLook = true;
		}
		firstPersonController.EnableGameInputs();
	}

	public void DisablePlayerMovement()
	{
		GameObject player = firstPersonController.gameObject;
		StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
		if (Input.mousePresent)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			inputs.cursorLocked = false;
			inputs.cursorInputForLook = false;
		}
		firstPersonController.DisableGameInputs();
	}
}
