using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, InteractableObject
{
	[SerializeField]
	List<Chair> chairs = new List<Chair>();

	private void OnEnable()
	{
		foreach (Chair c in chairs)
		{
			c.OnSitDown += SetAnimatorParameters;
		}
	}

	private void OnDisable()
	{
		foreach (Chair c in chairs)
		{
			c.OnSitDown -= SetAnimatorParameters;
		}
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

	public void SetAnimatorParameters(CustomerAI customer)
	{
		List<Transform> customers = new List<Transform>();

		foreach (Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				customers.Add(c.SeatedCustomer);
		}

		StartCoroutine(AnimationDelayCoroutine(customers));
	}

	IEnumerator AnimationDelayCoroutine(List<Transform> customers)
	{
		foreach (Transform c in customers)
		{
			Animator anim = c.GetComponent<Animator>();
			anim.SetBool("IsTalking", customers.Count > 1);
			yield return new WaitForSeconds(UnityEngine.Random.Range(2.0f, 4.0f));
		}
		yield return null;
	}
}
