using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_OrderItemsList : MonoBehaviour
{
	[SerializeField]
	RectTransform listHolderObject;
	[SerializeField]
	TMP_Text totalCostText;
	[SerializeField]
	string totalCostString = "Total: £{0}";
	[SerializeField]
	UI_OrderItem orderEntryPrefab;

	Dictionary<string, UI_OrderItem> itemsInList = new Dictionary<string, UI_OrderItem>();

	float totalCost = 0;

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

	private void Awake()
	{
		UpdateCostText();
	}

	private void AddItem(CafeMenuItem _item)
	{
		if(itemsInList.ContainsKey(_item.itemName))
		{
			itemsInList[_item.itemName].IncreaseQuantity();
		}
		else
		{
			UI_OrderItem orderItem = Instantiate(orderEntryPrefab.gameObject, listHolderObject).GetComponent<UI_OrderItem>();
			itemsInList.Add(_item.itemName, orderItem);
			orderItem.Initialise(_item);
		}
		totalCost += _item.price;
		UpdateCostText();
	}

	private void RemoveItem(string _item)
	{
		if(itemsInList.ContainsKey(_item))
		{
			totalCost -= itemsInList[_item].Item.price;
			Destroy(itemsInList[_item].gameObject);
			itemsInList.Remove(_item);
		}
		UpdateCostText();
	}

	private void UpdateCostText()
	{
		totalCostText.text = string.Format(totalCostString, totalCost.ToString("0.00"));
	}
}
