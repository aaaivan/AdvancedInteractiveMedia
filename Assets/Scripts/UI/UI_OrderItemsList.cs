using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OrderItemsList : MonoBehaviour
{
	[SerializeField]
	UI_OrderItem orderEntryPrefab;

	Dictionary<string, UI_OrderItem> itemsInList = new Dictionary<string, UI_OrderItem>();

	private void OnEnable()
	{
		UI_MenuItem.OnAddToOrder += AddItem;
		UI_OrderItem.OnRemoveFromOrder += RemoveItem;
	}

	private void OnDisable()
	{
		UI_MenuItem.OnAddToOrder -= AddItem;
		UI_OrderItem.OnRemoveFromOrder -= RemoveItem;
	}

	private void AddItem(CafeMenuItem _item)
	{
		if(itemsInList.ContainsKey(_item.itemName))
		{
			itemsInList[_item.itemName].IncreaseQuantity();
		}
		else
		{
			UI_OrderItem orderItem = Instantiate(orderEntryPrefab.gameObject, transform).GetComponent<UI_OrderItem>();
			itemsInList.Add(_item.itemName, orderItem);
			orderItem.Initialise(_item);
		}
	}

	private void RemoveItem(string _item)
	{
		if(itemsInList.ContainsKey(_item))
		{
			Destroy(itemsInList[_item].gameObject);
			itemsInList.Remove(_item);
		}
	}
}
