using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTable : MonoBehaviour, InteractableObject
{
	[SerializeField]
	FoodOnTableManager foodOnTable;

	public void DoInteraction(bool primary)
	{
		// leave on the table a food (i.e., non drink)
		Func<PubMenuItem, bool> condition = (item) => { return item.ItemData.item == PubMenuItemData.MenuItemEnum.Salad; };
		PlayerInventory.Instance.RemoveFromInventoryByCondition(condition, transform, OnFoodPutDownOnTable);
	}
	void OnFoodPutDownOnTable(PubMenuItem food)
	{
		foodOnTable.AddFood(food);
	}

	public bool IsFoodOnTable()
	{
		return foodOnTable.GetFoods().Count > 0;
	}
}
