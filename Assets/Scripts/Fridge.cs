using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Fridge : MonoBehaviour, InteractableObject
{
	[SerializeField]
	PubMenuItemData.MenuItemEnum drink;

	public void DoInteraction(bool primary)
	{
		if(primary)
		{
			PubMenuItemData drinkData = PubMenu.Instance.GetItem(PubMenuItemData.MenuItemType.Drink, drink);
			GameObject go = PubMenuItem.InstatiateItem(drinkData, transform);
			PlayerInventory.Instance.AddToInventory(go.GetComponent<PubMenuItem>());
		}
		else
		{
			Func<PubMenuItem, bool> condition = (item) => { return item.ItemData.item == drink; };
			PlayerInventory.Instance.RemoveFromInventoryByCondition(condition, null, null);
		}
	}
}
