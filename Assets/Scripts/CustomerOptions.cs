using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOptions : MonoBehaviour
{
	int tableNumber = 1;
	public int TableNumber {
		get { return tableNumber; }
		set { tableNumber = value; }
	}

	List<CafeMenuItem> order = new List<CafeMenuItem>();

	private void Start()
	{
		order.Add(CafeMenu.Instance.GetRandomItem(CafeMenuItem.MenuItemType.Drink));
		int rand = Random.Range(0, 10);

		if(rand % 3 == 0)
		{
			order.Add(CafeMenu.Instance.GetRandomItem(CafeMenuItem.MenuItemType.Side));
			if(rand == 3)
			{
				order.Add(CafeMenu.Instance.GetRandomItem(CafeMenuItem.MenuItemType.Main));
			}
		}
		else
		{
			order.Add(CafeMenu.Instance.GetRandomItem(CafeMenuItem.MenuItemType.Main));
		}

		order.Sort((a, b) => (a.item.CompareTo(b.item)));
	}

	public bool IsOrderMatching(List<CafeMenuItem> _items, int _tableNum)
	{
		if (_tableNum != tableNumber)
			return false;
		if(_items.Count != order.Count)
			return false;

		order.Sort((a, b) => (a.item.CompareTo(b.item)));
		_items.Sort((a, b) => (a.item.CompareTo(b.item)));

		for(int i = 0; i < order.Count; ++i)
		{
			if (order[i] != _items[i])
				return false;
		}
		return true;
	}

	public string GetOrderString()
	{
		if(order.Count == 0)
			return string.Empty;

		string result = order[0].itemName;
		int i = 1;
		for (; i < order.Count - 1; ++i)
		{
			result += ", ";
			result += order[i].itemName;
		}
		if (i == order.Count - 1)
		{
			result += " and ";
			result += order[i].itemName;
		}
		return result;
	}
}
