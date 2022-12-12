using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInputsController : MonoBehaviour
{
	private void OnEnable()
	{
		InputsManager inputsManager = InputsManager.Instance;
		inputsManager.EnableInputsByType(InputsManager.InputsType.Dialog);

		inputsManager.DialogControls.Enable();
		inputsManager.DialogControls.UI.DismissUI.performed += ctx => DismissUi();
	}
	private void OnDisable()
	{
		InputsManager inputsManager = InputsManager.Instance;

		inputsManager.DialogControls.UI.DismissUI.performed -= ctx => DismissUi();
		inputsManager.DisableInputsByType(InputsManager.InputsType.Dialog);
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
