using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuItemsList : MonoBehaviour
{
	[SerializeField]
	PubMenuItemData.MenuItemType type;
	[SerializeField]
	GameObject itemButton;

	private void Start()
	{
		foreach(var i in PubMenu.Instance.menuItemsMap[(int)type])
		{
			UI_MenuItem go = Instantiate(itemButton, gameObject.transform).GetComponent<UI_MenuItem>();
			go.Initialise(i);
		}
	}
}
