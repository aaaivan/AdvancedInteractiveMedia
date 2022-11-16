using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StartOrderScreen : MonoBehaviour
{
	[SerializeField]
	TMP_Text tableNumber;
	[SerializeField]
	UI_OrderItemsList orderList;
	[SerializeField]
	RectTransform orderScreen;

	public string TableNumber { get { return tableNumber.text; } }

	private void Awake()
	{
		tableNumber.text = "";
	}

	public void AddDigit(RectTransform t)
	{
		if (t == null || tableNumber.text.Length >=2)
			return;

		Transform textObj = t.GetChild(0);
		if (textObj == null)
			return;

		TMP_Text text = textObj.GetComponent<TMP_Text>();
		if(text == null)
			return;

		tableNumber.text += text.text;
	}

	public void RemoveLastDigit()
	{
		tableNumber.text = tableNumber.text.Substring(0, tableNumber.text.Length - 1);
	}

	public void ClearTableNumber()
	{
		tableNumber.text = "";
	}

	public void StartOrder()
	{
		if (tableNumber.text.Length == 0)
			return;

		if(Int32.TryParse(tableNumber.text, out int number))
		{
			if(number == 0)
			{
				ClearTableNumber();
			}
			else
			{
				orderList.TableNumber = number;
				tableNumber.text = "";
				orderScreen.gameObject.SetActive(true);
				gameObject.SetActive(false);
			}
		}
	}

	public void CancelOrder()
	{
		orderScreen.gameObject.SetActive(false);
		gameObject.SetActive(true);
	}
}
