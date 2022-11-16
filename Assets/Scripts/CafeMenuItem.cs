using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CafeMenuItem
{
	public enum MenuItemType
	{
		Main,
		Side,
		Drink,

		MAX_ITEM_TYPES
	}
	public MenuItemType type;
	public string itemName;
	public float price;
	public float prepTime;
	public GameObject prefab;
}
