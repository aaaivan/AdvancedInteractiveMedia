using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOptions : MonoBehaviour
{
	public int TableNumber
	{
		get
		{
			CustomerAI c = GetComponent<CustomerAI>();
			return c.Chair.Table.TableNumber;
		}
	}

	List<PubMenuItemData> order = new List<PubMenuItemData>();

	private void Start()
	{
		order.Add(PubMenu.Instance.GetRandomItem(PubMenuItemData.MenuItemType.Drink));
		int rand = Random.Range(0, 10);

		if(rand % 3 == 0)
		{
			order.Add(PubMenu.Instance.GetRandomItem(PubMenuItemData.MenuItemType.Side));
			if(rand == 3)
			{
				order.Add(PubMenu.Instance.GetRandomItem(PubMenuItemData.MenuItemType.Main));
			}
		}
		else
		{
			order.Add(PubMenu.Instance.GetRandomItem(PubMenuItemData.MenuItemType.Main));
		}

		order.Sort((a, b) => (a.item.CompareTo(b.item)));

		float cost = 0;
		foreach(PubMenuItemData item in order)
		{
			cost += item.price;
		}
		GetComponent<TipCalculator>().SetBaseTip(cost * 0.2f);
	}

	public bool IsOrderMatching(List<PubMenuItemData> _items, int _tableNum)
	{
		if (_tableNum != TableNumber)
			return false;

		return IsOrderMatching(_items);
	}

	public bool IsOrderMatching(List<PubMenuItemData> _items)
	{
		if (_items.Count != order.Count)
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

	public bool HasDrink()
	{
		for (int i = 0; i < order.Count; ++i)
		{
			if (order[i].type == PubMenuItemData.MenuItemType.Drink)
				return true;
		}
		return false;
	}

	public PubMenuItemData GetDrink()
	{
		for (int i = 0; i < order.Count; ++i)
		{
			if (order[i].type == PubMenuItemData.MenuItemType.Drink)
				return order[i];
		}
		return null;
	}

	public bool OrderHasItem(PubMenuItemData item)
	{
		return order.Contains(item);
	}

	public int GetItemsCount()
	{
		return order.Count; 
	}
}
