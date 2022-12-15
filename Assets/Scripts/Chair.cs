using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Chair : MonoBehaviour
{
	[SerializeField]
	FoodOnTableManager foodOntable;
	public FoodOnTableManager FoodOnTable { get { return foodOntable; } }

	public delegate void SitDownHandler(CustomerAI c);
	public event SitDownHandler OnSitDown;

	Transform seatedCustomer = null;
	public Transform SeatedCustomer
	{ 
		get { return seatedCustomer; }
	}
	bool isCustomerSitting = false;
	public bool HasCustomerArrived
	{
		get { return isCustomerSitting; }
	}

	public void SetCustomer(Transform _customer)
	{
		if(seatedCustomer == null && _customer != null)
		{
			isCustomerSitting = false;
			seatedCustomer = _customer;
		}
	}

	public void SetIsCustomerSitting(bool sitting)
	{
		if (isCustomerSitting == sitting)
			return;

		isCustomerSitting = sitting;
		if (isCustomerSitting && OnSitDown != null)
			OnSitDown.Invoke(seatedCustomer.GetComponent<CustomerAI>());
	}
}
