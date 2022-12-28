using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OrderPanel : MonoBehaviour
{
	[SerializeField]
	TMP_Text tableNumber;
	string tableString = "T{0}";
	[SerializeField]
	Image[] images;

	CustomerOptions order;

	private void Awake()
	{
		order = transform.parent.GetComponent<CustomerOptions>();
	}

	public void ShowOrder()
	{
		gameObject.SetActive(true);
		tableNumber.text = string.Format(tableString, order.TableNumber.ToString());
		tableNumber.gameObject.SetActive(true);

		for (int i = 0; i < order.Order.Count; ++i)
		{
			images[i].sprite = order.Order[i].image;
			images[i].gameObject.SetActive(true);
		}
	}

	public void HideTableNumber()
	{
		tableNumber.gameObject.SetActive(false);
	}

	public void HideFood(PubMenuItemData.MenuItemEnum item)
	{
		int activeItems = 0;

		for (int i = 0; i < order.Order.Count; ++i)
		{
			if (order.Order[i].item == item)
				images[i].gameObject.SetActive(false);
			else if(images[i].gameObject.activeSelf)
				activeItems++;
		}

		if(activeItems == 0)
		{
			gameObject.SetActive(false);
		}
	}

}
