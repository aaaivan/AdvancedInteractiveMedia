using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeMenu : MonoBehaviour
{
	[SerializeField]
	List<CafeMenuItem> menuItems = new List<CafeMenuItem>();

	[HideInInspector]
	public Dictionary<string, CafeMenuItem>[] menuItemsMap = new Dictionary<string, CafeMenuItem>[(int)CafeMenuItem.MenuItemType.MAX_ITEM_TYPES];

	static CafeMenu instance;
	static public CafeMenu Instance { get { return instance; } }

	private void OnDestroy()
	{
		instance = null;
	}

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;

			for (int t = 0; t < (int)CafeMenuItem.MenuItemType.MAX_ITEM_TYPES; ++t)
			{
				menuItemsMap[t] = new Dictionary<string, CafeMenuItem>();
			}
			for (int i = 0; i < menuItems.Count; ++i)
			{
				CafeMenuItem item = menuItems[i];
				string key = item.itemName;
				CafeMenuItem.MenuItemType type = item.type;

				if (!menuItemsMap[(int)type].ContainsKey(key))
				{
					menuItemsMap[(int)type].Add(key, item);
				}
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
