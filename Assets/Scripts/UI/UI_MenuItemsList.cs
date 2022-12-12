using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MenuItemsList : MonoBehaviour
{
	[SerializeField]
	CafeMenuItem.MenuItemType type;
	[SerializeField]
	GameObject itemButton;

	private void Start()
	{
		foreach(var i in CafeMenu.Instance.menuItemsMap[(int)type])
		{
			UI_MenuItem go = Instantiate(itemButton, gameObject.transform).GetComponent<UI_MenuItem>();
			go.Initialise(i);
		}
	}
}
