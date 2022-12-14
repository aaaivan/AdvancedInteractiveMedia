using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigationManager : MonoBehaviour
{
	[SerializeField]
	RectTransform[] screens = new RectTransform[(int)PubMenuItemData.MenuItemType.MAX_ITEM_TYPES + 1];
	[SerializeField]
	RectTransform backButton;

	static public Dictionary<string, PubMenuItemData.MenuItemType> menuNames = new Dictionary<string, PubMenuItemData.MenuItemType>{
		{"mains", PubMenuItemData.MenuItemType.Main},
		{"sides", PubMenuItemData.MenuItemType.Side},
		{"drinks", PubMenuItemData.MenuItemType.Drink},
		{"options", PubMenuItemData.MenuItemType.MAX_ITEM_TYPES},
	};

	private void Awake()
	{
		ShowScreen(PubMenuItemData.MenuItemType.MAX_ITEM_TYPES);
	}

	public void ShowScreen(PubMenuItemData.MenuItemType screen)
	{
		for( int i= 0; i < screens.Length; ++i)
		{
			if((int)screen == i)
			{
				screens[i].gameObject.SetActive(true);
			}
			else
			{
				screens[i].gameObject.SetActive(false);
			}
		}
		backButton.gameObject.SetActive(screen != PubMenuItemData.MenuItemType.MAX_ITEM_TYPES);
	}

	public void ShowScreen(string screen)
	{
		if(menuNames.ContainsKey(screen))
		{
			ShowScreen(menuNames[screen]);
		}
		else
		{
			Debug.LogWarning(screen + " is not a valid name for the till menus!");
		}
	}
}
