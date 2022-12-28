using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, InteractableObject
{
	[SerializeField]
	PubMenuItemData.MenuItemEnum drink;

	public void DoInteraction(bool primary)
	{
		if(primary)
		{
			PubMenuItemData drinkData = PubMenu.Instance.GetItem(PubMenuItemData.MenuItemType.Drink, drink);
			GameObject go = PubMenuItem.InstatiateItem(drinkData, LevelManager.Instance.FarFarAway);
			if(!PlayerInventory.Instance.AddToInventory(go.GetComponent<PubMenuItem>()))
			{
				Destroy(go);
			}
		}
		else
		{
			Func<PubMenuItem, bool> condition = (item) => { return item.ItemData.item == drink; };
			PlayerInventory.Instance.RemoveFromInventoryByCondition(condition, LevelManager.Instance.FarFarAway, DestroyDrink);
		}
	}

	void DestroyDrink(PubMenuItem drink)
	{
		Destroy(drink.gameObject);
	}
}
