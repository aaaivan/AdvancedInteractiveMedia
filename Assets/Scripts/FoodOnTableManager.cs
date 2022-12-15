using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOnTableManager : MonoBehaviour
{
	[SerializeField]
	Transform[] locations = new Transform[(int)PubMenuItemData.MenuItemType.MAX_ITEM_TYPES];
	
	public bool IsSpotFreeForItemType(PubMenuItemData.MenuItemType type)
	{
		return locations[(int)type].childCount == 0;
	}
	public void AddFood(PubMenuItem item)
	{
		item.transform.SetParent(locations[(int)item.ItemData.type], false);
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
		item.Interactable = false;
		item.HideTableUI();
	}
}
