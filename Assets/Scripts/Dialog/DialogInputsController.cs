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
	}
	private void OnDisable()
	{
		InputsManager inputsManager = InputsManager.Instance;
		inputsManager.DisableInputsByType(InputsManager.InputsType.Dialog);
	}
}
