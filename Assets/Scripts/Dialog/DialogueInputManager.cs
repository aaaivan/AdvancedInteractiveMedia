using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fluent;

public class DialogueInputManager : MonoBehaviour
{
	UIControls controls;

	private void Awake()
	{
		controls = new UIControls();
	}

	private void OnEnable()
	{
		controls.Enable();
		controls.UI.DismissUI.performed += ctx => DismissUi();
	}
	private void OnDisable()
	{
		controls.UI.DismissUI.performed -= ctx => DismissUi();
		controls.Disable();
	}

	private void DismissUi()
	{
		FluentScript convo = DialogueManager.Instance.ActiveDialogueGet();
		if (convo != null)
		{
			WriteHandler writeHandler = convo.gameObject.GetComponent<WriteHandler>();
			writeHandler.StopTyping();
		}
	}

	public void ToggleActionMap(bool active)
	{
		if(controls == null)
		{
			return;
		}

		if (active)
		{
			controls.Enable();
		}
		else
		{
			controls.Disable();
		}
	}
}
