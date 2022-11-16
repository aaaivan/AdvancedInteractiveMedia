using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CancelOrderButton : MonoBehaviour
{
	[SerializeField]
	UI_OrderItemsList listHolderObject;
	[SerializeField]
	MenuNavigationManager menuNavigation;
	[SerializeField]
	UI_StartOrderScreen startOrderScreen;

	public void CancelOrder()
	{
		listHolderObject.ClearOrder();
		menuNavigation.ShowScreen("options");
		startOrderScreen.CancelOrder();
	}
}
