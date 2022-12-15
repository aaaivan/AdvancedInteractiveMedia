using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, InteractableObject
{
	[SerializeField]
	List<Chair> chairs = new List<Chair>();

	int customerCount = 0;
	public int CustomerCount
	{
		get { return customerCount; }
		set { customerCount = value; SetAnimatorParameters(); }
	}

	public void DoInteraction(bool primary)
	{
		List<CustomerOptions> customers = new List<CustomerOptions>();

		foreach(Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				customers.Add(c.SeatedCustomer.GetComponent<CustomerOptions>());
		}

		Func<PubMenuItem, bool> condition = (item) => { return item.ItemData.type != PubMenuItemData.MenuItemType.Drink; };
		PlayerInventory.Instance.RemoveFromInventoryByCondition(condition, transform, OnFoodPutDownOnTable);
	}

	void OnFoodPutDownOnTable(PubMenuItem food)
	{
		List<CustomerOptions> customers = new List<CustomerOptions>();

		foreach (Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				customers.Add(c.SeatedCustomer.GetComponent<CustomerOptions>());
		}

		bool found = false;
		foreach(CustomerOptions c in customers)
		{
			FoodOnTableManager f = c.GetComponent<CustomerAI>().Chair.FoodOnTable;
			if (c.OrderHasItem(food.ItemData) && f.IsSpotFreeForItemType(food.ItemData.type))
			{
				f.AddFood(food);
				found = true;
				break;
			}
		}

		if(!found)
		{
			Debug.Log("Wrong Order!");
			PlayerInventory.Instance.AddToInventory(food);
		}
	}

	void SetAnimatorParameters()
	{
		List<Transform> customers = new List<Transform>();

		foreach (Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				customers.Add(c.SeatedCustomer);
		}

		foreach (Transform c in customers)
		{
			Animator anim = c.GetComponent<Animator>();
			anim.SetBool("IsTalking", customerCount > 1);
		}

	}
}
