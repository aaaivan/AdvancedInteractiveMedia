using Newtonsoft.Json.Linq;
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
	TMP_Text tableNumberText;
	[SerializeField]
	string tableNumberString = "Table Number: {0}";
	[SerializeField]
	UI_OrderItem orderEntryPrefab;
	[SerializeField]
	RectTransform orderActionsPanel;
	[SerializeField]
	RectTransform processPaymentPanel;
	[SerializeField]
	RectTransform clearOrderButton;
	[SerializeField]
	UI_CancelOrderButton cancelOrderButton;

	public delegate void ProcessOrder(List<PubMenuItemData> i, int table);
	static public event ProcessOrder OnOrderSubmitted;

	public delegate void PaymentHandler(List<PubMenuItemData> i, int table, string total);
	static public event PaymentHandler OnPaymentReady;

	Dictionary<PubMenuItemData.MenuItemEnum, UI_OrderItem> itemsInList = new Dictionary<PubMenuItemData.MenuItemEnum, UI_OrderItem>();

	float totalCost = 0;
	public float TotalCost { get { return totalCost; }}
	int tableNumber = 0;
	public int TableNumber
	{ 
		get { return tableNumber; }
		set { tableNumber = value; tableNumberText.text = string.Format(tableNumberString, tableNumber); }
	}

	private void OnEnable()
	{
		NewOrderDialog.OnOrderPaid += SubmitOrder;
		UI_MenuItem.OnAddToOrder += AddItem;
		UI_OrderItem.OnRemoveFromOrder += RemoveItem;
	}

	private void OnDisable()
	{
		NewOrderDialog.OnOrderPaid -= SubmitOrder;
		UI_MenuItem.OnAddToOrder -= AddItem;
		UI_OrderItem.OnRemoveFromOrder -= RemoveItem;
	}

	private void Awake()
	{
		UpdateCostText();
	}

	private void AddItem(PubMenuItemData _item)
	{
		if(itemsInList.ContainsKey(_item.item))
		{
			itemsInList[_item.item].IncreaseQuantity();
		}
		else
		{
			UI_OrderItem orderItem = Instantiate(orderEntryPrefab.gameObject, listHolderObject).GetComponent<UI_OrderItem>();
			itemsInList.Add(_item.item, orderItem);
			orderItem.Initialise(_item);
		}
		totalCost += _item.price;
		UpdateCostText();
	}

	private void RemoveItem(PubMenuItemData.MenuItemEnum _item)
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

	public void ClearOrder()
	{
		foreach (var item in itemsInList)
		{
			Destroy(item.Value.gameObject);
		}
		itemsInList.Clear();
		totalCost = 0;
		UpdateCostText();
	}

	public void ConfirmOrder()
	{
		if (itemsInList.Count == 0)
			return;

		orderActionsPanel.gameObject.SetActive(false);
		processPaymentPanel.gameObject.SetActive(true);
		clearOrderButton.gameObject.SetActive(false);

		if(OnPaymentReady != null)
		{
			List<PubMenuItemData> orderItems = new List<PubMenuItemData>();
			foreach (var orderItem in itemsInList)
			{
				for (int i = 0; i < orderItem.Value.Quantity; i++)
				{
					PubMenuItemData item = orderItem.Value.Item;
					orderItems.Add(item);
				}
			}

			OnPaymentReady.Invoke(orderItems, tableNumber, totalCost.ToString("0.00"));
		}
	}

	public void CancelPayment()
	{
		orderActionsPanel.gameObject.SetActive(true);
		processPaymentPanel.gameObject.SetActive(false);
		clearOrderButton.gameObject.SetActive(true);
	}

	public void SubmitOrder()
	{
		List<PubMenuItemData> orderItems = new List<PubMenuItemData>();
		foreach (var orderItem in itemsInList)
		{
			for (int i = 0; i < orderItem.Value.Quantity; i++)
			{
				PubMenuItemData item = orderItem.Value.Item;
				if(item.type != PubMenuItemData.MenuItemType.Drink)
				{
					orderItems.Add(item);
				}
			}
		}
		cancelOrderButton.CancelOrder();
		CancelPayment();

		if (OnOrderSubmitted != null)
		{
			OnOrderSubmitted.Invoke(orderItems, tableNumber);
		}
	}
}
