using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MealConsumption : MonoBehaviour
{
	CustomerOptions order;
	CustomerAI customer;
	FoodOnTableManager foodOnTable;
	List<PubMenuItemData> consumedFood = new List<PubMenuItemData>();
	bool hasFinishedEating = false;
	bool isOrderComplete = false;
	public bool HasFinishedEating { get { return hasFinishedEating; } }

	private void Awake()
	{
		order = GetComponent<CustomerOptions>();
		customer = GetComponent<CustomerAI>();
	}

	private void Start()
	{
		foodOnTable = customer.Chair.FoodOnTable;
	}

	public bool IsWaitingForFood(PubMenuItemData food)
	{
		return order.OrderHasItem(food) && !consumedFood.Contains(food) && foodOnTable.IsSpotFreeForItemType(food.type);
	}

	private void Update()
	{
		if (customer.Chair == null || !customer.Chair.HasCustomerArrived)
			return;

		List<PubMenuItem> items = foodOnTable.GetFoods();
		if(!isOrderComplete && items.Count + consumedFood.Count == order.GetItemsCount())
		{
			isOrderComplete = true;
			GetComponent<TipCalculator>().foodWaitTime = Time.time - customer.SitAtTableTime;
		}
		foreach (PubMenuItem item in items)
		{
			float deltaProgress = Time.deltaTime / (item.ItemData.timeToEat * items.Count);
			item.ConsumptionProgression = item.ConsumptionProgression - deltaProgress;
			if(item.ConsumptionProgression <= 0)
			{
				item.ConsumptionProgression = 0;
				consumedFood.Add(item.ItemData);
				Destroy(item.gameObject);
				if(order.IsOrderMatching(consumedFood))
					hasFinishedEating= true;
			}
		}
	}
}
