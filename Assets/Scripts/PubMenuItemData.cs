using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PubMenuItemData
{
	public enum MenuItemEnum
	{
		Burger,
		Wrap,
		HotDog,
		Pizza,
		Steak,

		Salad,
		FrenchFries,
		OnionRings,

		Water,
		Coke,
		Lemonade,

		None,
	};

	public enum MenuItemType
	{
		Main,
		Side,
		Drink,

		MAX_ITEM_TYPES
	}

	public MenuItemType type;
	public MenuItemEnum item;
	public string itemName;
	public float price;
	public float prepTime;
	public float timeToEat;
	public GameObject prefab;
}