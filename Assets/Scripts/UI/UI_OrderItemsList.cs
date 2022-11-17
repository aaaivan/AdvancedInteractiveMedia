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

	public delegate void ProcessOrder( List<CafeMenuItem> i, int table);
	static public event ProcessOrder OnOrderSubmitted;

	IEnumerator coroutine = null;
	Dictionary<string, UI_OrderItem> itemsInList = new Dictionary<string, UI_OrderItem>();

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
		coroutine = PaymentProcessingCoroutine();
		StartCoroutine(coroutine);
	}

	public void CancelPayment()
	{
		orderActionsPanel.gameObject.SetActive(true);
		processPaymentPanel.gameObject.SetActive(false);
		StopCoroutine(coroutine);
		coroutine = null;
		clearOrderButton.gameObject.SetActive(true);
	}

	IEnumerator PaymentProcessingCoroutine()
	{
		float waitTime = 3f;
		yield return new WaitForSeconds(waitTime);
		SubmitOrder();
	}

	private void SubmitOrder()
	{
		List<CafeMenuItem> orderItems = new List<CafeMenuItem>();
		foreach (var orderItem in itemsInList)
		{
			for (int i = 0; i < orderItem.Value.Quantity; i++)
			{
				CafeMenuItem item = orderItem.Value.Item;
				if(item.type != CafeMenuItem.MenuItemType.Drink)
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
