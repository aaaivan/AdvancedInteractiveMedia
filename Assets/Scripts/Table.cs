using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, InteractableObject
{
	[SerializeField]
	List<Chair> chairs = new List<Chair>();
	[SerializeField]
	int tableNumber = 0;
	public int TableNumber { get { return tableNumber; } }

	private void OnEnable()
	{
		foreach (Chair c in chairs)
		{
			c.OnSitDown += SetTalkingAnimation;
		}
	}

	private void OnDisable()
	{
		foreach (Chair c in chairs)
		{
			c.OnSitDown -= SetTalkingAnimation;
		}
	}

	public void DoInteraction(bool primary)
	{
		List<CustomerOptions> customers = new List<CustomerOptions>();

		// find the customers that are sitting on a char at this table
		foreach(Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				customers.Add(c.SeatedCustomer.GetComponent<CustomerOptions>());
		}

		// leave on the table a food (i.e., non drink)
		Func<PubMenuItem, bool> condition = (item) => { return item.ItemData.type != PubMenuItemData.MenuItemType.Drink; };
		PlayerInventory.Instance.RemoveFromInventoryByCondition(condition, transform, OnFoodPutDownOnTable);
	}

	public int GetSize()
	{
		return chairs.Count;
	}

	public bool IsEmpty()
	{
		return GetNumberOfFreeChairs() == chairs.Count;
	}

	public int GetNumberOfFreeChairs()
	{
		int result = 0;
		foreach (Chair c in chairs)
		{
			if (c.SeatedCustomer == null)
				++result;
		}

		return result;
	}

	public Chair GetFreeChair()
	{
		foreach (Chair c in chairs)
		{
			if (c.SeatedCustomer == null)
				return c;
		}
		return null;
	}

	void OnFoodPutDownOnTable(PubMenuItem food)
	{
		List<MealConsumption> meals = new List<MealConsumption>();

		// find the customers that are sitting on a char at this table
		foreach (Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				meals.Add(c.SeatedCustomer.GetComponent<MealConsumption>());
		}

		bool found = false;
		foreach(MealConsumption m in meals)
		{
			// check whether customer which has the meal m is waiting for this food
			if (m.IsWaitingForFood(food.ItemData))
			{
				FoodOnTableManager f = m.GetComponent<CustomerAI>().Chair.FoodOnTable;
				f.AddFood(food);
				found = true;
				break;
			}
		}

		// no matching order for this food item was found
		if(!found)
		{
			Debug.Log("Wrong Order!");
			PlayerInventory.Instance.AddToInventory(food);
		}
	}

	public void SetTalkingAnimation(CustomerAI customer)
	{
		Animator anim = customer.GetComponent<Animator>();
		anim.SetBool("IsTalking", false);

		List<Transform> customers = new List<Transform>();
		// find the customers that are sitting on a char at this table
		foreach (Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				customers.Add(c.SeatedCustomer);
		}

		StartCoroutine(AnimationDelayCoroutine(customers));
	}

	IEnumerator AnimationDelayCoroutine(List<Transform> customers)
	{
		// play the talk animation if at least 2 customers are sitting at the table
		foreach (Transform c in customers)
		{
			Animator anim = c.GetComponent<Animator>();
			anim.SetBool("IsTalking", customers.Count > 1);
			yield return new WaitForSeconds(UnityEngine.Random.Range(2.0f, 4.0f));
		}
		yield return null;
	}
}
