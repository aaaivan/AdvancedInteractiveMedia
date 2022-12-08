using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInitiator : MonoBehaviour, InteractableObject
{
	public void DoInteraction()
	{
		FluentManager.Instance.ExecuteAction(GetComponentInChildren<FluentScript>());
	}
}