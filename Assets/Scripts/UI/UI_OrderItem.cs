using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_OrderItem : MonoBehaviour
{
	[SerializeField]
	string displayText = "{0}x {1}";
	[SerializeField]
	TMP_Text text;

	CafeMenuItem item;
	public CafeMenuItem Item { get {return item;} }
	int quantity;
	public int Quantity { get { return quantity; } }

	public delegate void RemoveFromOrder(CafeMenuItem.MenuItemEnum i);
	static public event RemoveFromOrder OnRemoveFromOrder;

	private void Awake()
	{
		quantity = 1;
	}

	public void Initialise(CafeMenuItem _item)
	{
		item = _item;
		text.text = string.Format(displayText, quantity, item.itemName);
	}

	public void IncreaseQuantity()
	{
		quantity++;
		text.text = string.Format(displayText, quantity, item.itemName);
	}

	public void DecreaseQuantity()
	{
		quantity--;
		if(quantity == 0)
		{
			if(OnRemoveFromOrder != null)
			{
				OnRemoveFromOrder.Invoke(item.item);
			}
		}
		else
		{
			text.text = string.Format(displayText, quantity, item.itemName);
		}
	}
}
