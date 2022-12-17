using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PubMenuItemData
{
	public enum MenuItemEnum
	{
		Burger,
		HotDog,
		Pizza,
		Steak,

		Salad,
		FrenchFries,
		Nachos,

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