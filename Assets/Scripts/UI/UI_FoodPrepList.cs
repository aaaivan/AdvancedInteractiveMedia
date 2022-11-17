using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FoodPrepList : MonoBehaviour
{
	[SerializeField]
	UI_FoodItemPrep foodItemPrefab;
	List<GameObject> foodItemEntries = new List<GameObject>();
	Queue<Tuple<CafeMenuItem, int>> queueForCounter = new Queue<Tuple<CafeMenuItem, int>>();

	private void OnEnable()
	{
		UI_FoodItemPrep.OnFoodReady += AddFoodItemToCounter;
		UI_OrderItemsList.OnOrderSubmitted += AddFoodItemsToList;
	}

	private void OnDisable()
	{
		UI_FoodItemPrep.OnFoodReady -= AddFoodItemToCounter;
		UI_OrderItemsList.OnOrderSubmitted -= AddFoodItemsToList;
	}

	private void AddFoodItemsToList(List<CafeMenuItem> _foodItems, int tableNum)
	{
		foreach (CafeMenuItem item in _foodItems)
		{
			GameObject go = Instantiate(foodItemPrefab.gameObject, transform);
			UI_FoodItemPrep foodItem = go.GetComponent<UI_FoodItemPrep>();
			foodItem.Initialise(item, tableNum);
			foodItemEntries.Add(go);
		}
	}

	private void AddFoodItemToCounter(GameObject foodItemEntry)
	{
		if(foodItemEntries.Remove(foodItemEntry))
		{
			UI_FoodItemPrep readyItem = foodItemEntry.GetComponent<UI_FoodItemPrep>();
			if (queueForCounter.Count == 0)
			{
				if(!FoodSpawningManager.Instance.TrySpawnFood(readyItem.FoodItem, readyItem.TableNumber))
				{
					queueForCounter.Enqueue(new Tuple<CafeMenuItem, int>(readyItem.FoodItem, readyItem.TableNumber));
				}
			}
			else
			{
				queueForCounter.Enqueue(new Tuple<CafeMenuItem, int>(readyItem.FoodItem, readyItem.TableNumber));
			}
			Destroy(foodItemEntry);
		}
	}

	private void Update()
	{
		if (queueForCounter.Count > 0)
		{
			Tuple<CafeMenuItem, int> readyItem = queueForCounter.Peek();
			if (FoodSpawningManager.Instance.TrySpawnFood(readyItem.Item1, readyItem.Item2))
			{
				queueForCounter.Dequeue();
			}
		}
	}
}
