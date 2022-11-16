using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigationManager : MonoBehaviour
{
	[SerializeField]
	RectTransform[] screens = new RectTransform[(int)CafeMenuItem.MenuItemType.MAX_ITEM_TYPES + 1];
	[SerializeField]
	RectTransform backButton;

	static public Dictionary<string, CafeMenuItem.MenuItemType> menuNames = new Dictionary<string, CafeMenuItem.MenuItemType>{
		{"mains", CafeMenuItem.MenuItemType.Main},
		{"sides", CafeMenuItem.MenuItemType.Side},
		{"drinks", CafeMenuItem.MenuItemType.Drink},
		{"options", CafeMenuItem.MenuItemType.MAX_ITEM_TYPES},
	};

	private void Awake()
	{
		ShowScreen(CafeMenuItem.MenuItemType.MAX_ITEM_TYPES);
	}

	public void ShowScreen(CafeMenuItem.MenuItemType screen)
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
		backButton.gameObject.SetActive(screen != CafeMenuItem.MenuItemType.MAX_ITEM_TYPES);
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
