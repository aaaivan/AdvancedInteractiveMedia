using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FoodPrepList : MonoBehaviour
{
	[SerializeField]
	UI_FoodItemPrep foodItemPrefab;
	List<GameObject> foodItemEntries = new List<GameObject>();

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
			// spawn food
			Destroy(foodItemEntry);
		}
	}

}
