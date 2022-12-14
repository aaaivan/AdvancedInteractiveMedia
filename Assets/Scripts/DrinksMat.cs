using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinksMat : MonoBehaviour, InteractableObject
{
	public delegate void DrinkOnMatHandler(GameObject drink);
	public static event DrinkOnMatHandler OnDrinkPutOnMat;

	public GameObject CurrentDrink
	{
		get
		{ 
			return transform.childCount > 0 ? 
				transform.GetChild(0).gameObject :
				null;
		}
	}

	public void DoInteraction(bool primary)
	{
		if (transform.childCount > 0)
			return;

		Func<PubMenuItem, bool> condition = (item) => { return item.ItemData.type == PubMenuItemData.MenuItemType.Drink; };
		PlayerInventory.Instance.RemoveFromInventoryByCondition(condition, transform, AddDrinkToMat);
	}

	void AddDrinkToMat(PubMenuItem drink)
	{
		if(OnDrinkPutOnMat != null)
		{
			OnDrinkPutOnMat.Invoke(drink.gameObject);
		}
	}
}

