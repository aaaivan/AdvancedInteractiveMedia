using Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Table : MonoBehaviour, InteractableObject
{
	[SerializeField]
	TMP_Text[] tableNumText;

	[SerializeField]
	List<Transform> seats = new List<Transform>();
	List<Chair> chairs = new List<Chair>();
	int tableNumber = 0;
	public int TableNumber { get { return tableNumber; } }

	WrongOrderDialog wrongOrderDialog;

	private void OnEnable()
	{
		foreach (Chair c in chairs)
		{
			c.OnSitDown += SetTalkingAnimation;
			c.OnStandUp += SetTalkingAnimation;
		}
	}

	private void OnDisable()
	{
		foreach (Chair c in chairs)
		{
			c.OnSitDown -= SetTalkingAnimation;
			c.OnStandUp -= SetTalkingAnimation;
		}
	}

	private void Awake()
	{
		wrongOrderDialog = GetComponent<WrongOrderDialog>();
		foreach(Transform t in seats)
		{
			Chair c = t.Find("Chair").GetComponent<Chair>();
			chairs.Add(c);
		}
	}

	private void Start()
	{
		tableNumber = LevelManager.Instance.GetTableNumber(this);
		foreach(var num in tableNumText)
			num.text = tableNumber.ToString();
	}

	private void Update()
	{
		List<MealConsumption> meals = new List<MealConsumption>();

		// find the customers that are sitting on a char at this table
		foreach (Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				meals.Add(c.SeatedCustomer.GetComponent<MealConsumption>());
		}

		bool freeTable = true;
		foreach (MealConsumption m in meals)
		{
			// check whether all customers have finished their meal
			if (!m.HasFinishedEating)
			{
				freeTable = false;
				break;
			}
		}

		if(freeTable)
		{
			// make all customers at this table leave
			foreach (MealConsumption m in meals)
			{
				CustomerAI c = m.GetComponent<CustomerAI>();
				c.LeaveRestaurant();
			}
		}
	}

	public void DoInteraction(bool primary)
	{
		List<CustomerOptions> customers = new List<CustomerOptions>();

		// find the customers that are sitting on a chair at this table
		foreach(Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				customers.Add(c.SeatedCustomer.GetComponent<CustomerOptions>());
		}

		if (customers.Count == 0)
			return;

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

	public int GetNumberOfCustomers()
	{
		int result = 0;
		foreach (Chair c in chairs)
		{
			if (c.HasCustomerArrived && c.SeatedCustomer != null)
				result++;
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

	public bool HasAllFoodBeenDeliveredToTable()
	{
		foreach (Chair c in chairs)
		{
			if (c.SeatedCustomer != null)
			{
				if (c.SeatedCustomer.GetComponent<MealConsumption>().IsOrderComplete == false)
					return false;
			}
		}
		return true;
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
				m.OrderUI.HideFood(food.ItemData.item);
				found = true;
				break;
			}
		}

		// no matching order for this food item was found
		if(!found)
		{
			PlayerInventory.Instance.AddToInventory(food);

			List<Transform> customers = new List<Transform>();
			foreach (MealConsumption m in meals)
			{
				customers.Add(m.transform);
				m.GetComponent<TipCalculator>().mistakes++;
			}
			Action<Animator> fn = (anim) => anim.SetTrigger("DoDisapproval");
			StartCoroutine(AnimationDelayCoroutine(customers, fn, 0.2f, 0.5f));
			wrongOrderDialog.SetWrongFood(food.ItemData);
			FluentManager.Instance.ExecuteAction(wrongOrderDialog);
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

		Action<Animator> fn = (anim) => anim.SetBool("IsTalking", customers.Count > 1);
		StartCoroutine(AnimationDelayCoroutine(customers, fn, 2f, 4f));
	}

	IEnumerator AnimationDelayCoroutine(List<Transform> customers, Action<Animator> animFunction , float minDelay, float maxDelay)
	{
		// play the talk animation if at least 2 customers are sitting at the table
		foreach (Transform c in customers)
		{
			Animator anim = c.GetComponent<Animator>();
			animFunction(anim);
			yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay));
		}
		yield return null;
	}
}
