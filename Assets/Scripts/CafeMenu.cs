using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CafeMenu : MonoBehaviour
{
	[SerializeField]
	List<CafeMenuItem> menuItems = new List<CafeMenuItem>();

	[HideInInspector]
	public List<CafeMenuItem>[] menuItemsMap = new List<CafeMenuItem>[(int)CafeMenuItem.MenuItemType.MAX_ITEM_TYPES];

	static CafeMenu instance;
	static public CafeMenu Instance { get { return instance; } }

	private void OnDestroy()
	{
		if(instance == this)
			instance = null;
	}

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;

			for (int t = 0; t < (int)CafeMenuItem.MenuItemType.MAX_ITEM_TYPES; ++t)
			{
				menuItemsMap[t] = new List<CafeMenuItem>();
			}
			for (int i = 0; i < menuItems.Count; ++i)
			{
				CafeMenuItem item = menuItems[i];
				CafeMenuItem.MenuItemType type = item.type;
				menuItemsMap[(int)type].Add(item);
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public CafeMenuItem GetRandomItem(CafeMenuItem.MenuItemType type)
	{
		int index = Random.Range(0, menuItemsMap[(int)type].Count);
		return menuItemsMap[(int)type][index];
	}
}
