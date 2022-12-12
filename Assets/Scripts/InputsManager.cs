using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsManager : MonoBehaviour
{
	FirstPersonController firstPersonController;
	TabletViewControls tabletControls;
	public TabletViewControls TabletControls { get { return tabletControls; } }
	UIControls dialogControls;
	public UIControls DialogControls { get { return dialogControls; } }

	public enum InputsType
	{
		Gameplay,
		Tablet,
		Dialog,

		None
	}
	Stack<InputsType> inputsStack = new Stack<InputsType>();

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
		if(instance == this)
			instance = null;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			firstPersonController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
			tabletControls = new TabletViewControls();
			dialogControls = new UIControls();

			inputsStack.Clear();
			tabletControls.Disable();
			dialogControls.Disable();
			EnableInputsByType(InputsType.Gameplay);
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}

	void EnablePlayerMovement()
	{
		if (firstPersonController.AreGameInputsEnabled())
			return;

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

	void DisablePlayerMovement()
	{
		if (!firstPersonController.AreGameInputsEnabled())
			return;

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

	void EnableTabletInputs()
	{
		tabletControls.Enable();
	}
	void DisableTabletInputs()
	{
		tabletControls.Disable();
	}

	void EnableDialogInputs()
	{
		dialogControls.Enable();
	}
	void DisableDialogInputs()
	{
		dialogControls.Disable();
	}

	public void EnableInputsByType(InputsType inputs)
	{
		InputsType disableInputs = InputsType.None;
		if (inputsStack.Count > 0 && inputsStack.Peek() == inputs)
			return;
		else if(inputsStack.Count > 0)
			disableInputs = inputsStack.Peek();

		switch(disableInputs)
		{
			case InputsType.Gameplay:
				DisablePlayerMovement();
				break;
			case InputsType.Tablet:
				DisableTabletInputs();
				break;
			case InputsType.Dialog:
				DisableDialogInputs();
				break;
			default:
				break;
		}

		inputsStack.Push(inputs);

		switch (inputs)
		{
			case InputsType.Gameplay:
				EnablePlayerMovement();
				break;
			case InputsType.Tablet:
				EnableTabletInputs();
				break;
			case InputsType.Dialog:
				EnableDialogInputs();
				break;
			default:
				return;
		}
	}
	public void DisableInputsByType(InputsType inputs)
	{
		if (inputsStack.Count == 0 || inputsStack.Peek() != inputs)
			return;

		switch (inputs)
		{
			case InputsType.Gameplay:
				DisablePlayerMovement();
				break;
			case InputsType.Tablet:
				DisableTabletInputs();
				break;
			case InputsType.Dialog:
				DisableDialogInputs();
				break;
			default:
				return;
		}

		inputsStack.Pop();
		if (inputsStack.Count == 0)
			return;

		InputsType enableInputs = inputsStack.Peek();
		switch (enableInputs)
		{
			case InputsType.Gameplay:
				EnablePlayerMovement();
				break;
			case InputsType.Tablet:
				EnableTabletInputs();
				break;
			case InputsType.Dialog:
				EnableDialogInputs();
				break;
			default:
				return;
		}
	}
}
