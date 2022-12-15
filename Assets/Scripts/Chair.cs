using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Chair : MonoBehaviour
{
	[SerializeField]
	FoodOnTableManager foodOntable;
	public FoodOnTableManager FoodOnTable { get { return foodOntable; } }

	Transform seatedCustomer = null;
	public Transform SeatedCustomer
	{ 
		get { return seatedCustomer; }
		set { seatedCustomer = value; }
	}
	bool hasCustomerArrived = false;
	public bool HasCustomerArrived
	{
		get { return hasCustomerArrived; }
		set { hasCustomerArrived = value; }
	}

	public void SetCustomerOnChair(Transform _customer)
	{
		if(seatedCustomer == null && _customer != null)
		{
			Table t = GetComponentInParent<Table>();
			t.CustomerCount = t.CustomerCount + 1;
			seatedCustomer = _customer;
			hasCustomerArrived = false;
		}
		else if (seatedCustomer != null && _customer == null)
		{
			Table t = GetComponentInParent<Table>();
			t.CustomerCount = t.CustomerCount - 1;
			seatedCustomer = _customer;
			hasCustomerArrived = false;
		}
	}
}
