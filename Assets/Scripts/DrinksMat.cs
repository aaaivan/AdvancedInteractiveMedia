using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinksMat : MonoBehaviour, InteractableObject
{
	Transform drinkHolder;

	public delegate void DrinkOnMatHandler(GameObject drink);
	public static event DrinkOnMatHandler OnDrinkPutOnMat;

	private void Awake()
	{
		drinkHolder = transform.Find("Inventory");
	}

	public GameObject CurrentDrink
	{
		get
		{ 
			return drinkHolder.childCount > 0 ?
				drinkHolder.GetChild(0).gameObject :
				null;
		}
	}

	public void DoInteraction(bool primary)
	{
		if (drinkHolder.childCount > 0)
			return;

		Func<PubMenuItem, bool> condition = (item) => { return item.ItemData.type == PubMenuItemData.MenuItemType.Drink; };
		PlayerInventory.Instance.RemoveFromInventoryByCondition(condition, drinkHolder, AddDrinkToMat);
	}

	void AddDrinkToMat(PubMenuItem drink)
	{
		if(OnDrinkPutOnMat != null)
		{
			OnDrinkPutOnMat.Invoke(drink.gameObject);
		}
	}
}

