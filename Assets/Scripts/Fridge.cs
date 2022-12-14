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
			PlayerInventory.Instance.AddToInventory(PubMenu.Instance.GetItem(PubMenuItemData.MenuItemType.Drink, drink));
		}
		else
		{
			PlayerInventory.Instance.RemoveFromInventory(PubMenu.Instance.GetItem(PubMenuItemData.MenuItemType.Drink, drink));
		}
	}
}
