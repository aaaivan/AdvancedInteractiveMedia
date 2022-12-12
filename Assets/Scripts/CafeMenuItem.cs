using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CafeMenuItem
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
	public GameObject prefab;
}