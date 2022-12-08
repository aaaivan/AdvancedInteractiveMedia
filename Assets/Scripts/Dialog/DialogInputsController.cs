using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInputsController : MonoBehaviour
{
	UIControls uiControls;

	private void Awake()
	{
		uiControls = new UIControls();
	}

	private void OnEnable()
	{
		uiControls.Enable();
		uiControls.UI.DismissUI.performed += ctx => DismissUi();
	}
	private void OnDisable()
	{
		uiControls.UI.DismissUI.performed -= ctx => DismissUi();
		uiControls.Disable();
	}

	private void DismissUi()
	{
		FluentScript[] dialogs = FluentManager.Instance.GetActiveDialogs().ToArray();
		foreach(var d in dialogs)
		{
			WriteHandler writeHandler = d.gameObject.GetComponent<WriteHandler>();
			if(writeHandler != null)
				writeHandler.StopTyping();
		}
	}
}
