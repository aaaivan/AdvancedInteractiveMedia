using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_MenuItem : MonoBehaviour
{
	[SerializeField]
	string displayText = "{0} (£{1})";
	[SerializeField]
	TMP_Text text;

	CafeMenuItem item;

	public delegate void AddToOrder(CafeMenuItem i);
	static public event AddToOrder OnAddToOrder;

	public void Initialise(CafeMenuItem _item)
	{
		item = _item;
		text.text = string.Format(displayText, item.itemName, item.price.ToString("0.00"));
	}

	public void AddItemToOrder()
	{
		if(OnAddToOrder != null)
		{
			OnAddToOrder.Invoke(item);
		}
	}
}
