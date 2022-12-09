using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInitiator : MonoBehaviour, InteractableObject
{
	CustomerAI customer;

	private void Awake()
	{
		customer = GetComponent<CustomerAI>();
	}

	public void DoInteraction()
	{
		if (customer.State != CustomerAI.CustomerState.FrontOfTheQueue)
			return;

		FluentManager.Instance.ExecuteAction(GetComponentInChildren<FluentScript>());
	}
}