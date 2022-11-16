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

	[SerializeField]
	RectTransform orderActionsPanel;
	[SerializeField]
	RectTransform processPaymentPanel;

	IEnumerator coroutine = null;
	Dictionary<string, UI_OrderItem> itemsInList = new Dictionary<string, UI_OrderItem>();

	float totalCost = 0;
	public float TotalCost { get { return totalCost; } }

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
		orderActionsPanel.gameObject.SetActive(false);
		processPaymentPanel.gameObject.SetActive(true);
		coroutine = PaymentProcessingCoroutine();
		StartCoroutine(coroutine);
	}

	public void CancelPayment()
	{
		orderActionsPanel.gameObject.SetActive(true);
		processPaymentPanel.gameObject.SetActive(false);
		StopCoroutine(coroutine);
		coroutine = null;
	}

	IEnumerator PaymentProcessingCoroutine()
	{
		float waitTime = 3f;
		yield return new WaitForSeconds(waitTime);
		SubmitOrder();
	}

	private void SubmitOrder()
	{
		Debug.Log("Order submitted!");
		ClearOrder();
		orderActionsPanel.gameObject.SetActive(true);
		processPaymentPanel.gameObject.SetActive(false);

	}

}
