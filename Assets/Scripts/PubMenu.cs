using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PubMenu : MonoBehaviour
{
	[SerializeField]
	List<PubMenuItemData> menuItems = new List<PubMenuItemData>();

	[HideInInspector]
	public List<PubMenuItemData>[] menuItemsMap = new List<PubMenuItemData>[(int)PubMenuItemData.MenuItemType.MAX_ITEM_TYPES];

	static PubMenu instance;
	static public PubMenu Instance { get { return instance; } }

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

			for (int t = 0; t < (int)PubMenuItemData.MenuItemType.MAX_ITEM_TYPES; ++t)
			{
				menuItemsMap[t] = new List<PubMenuItemData>();
			}
			for (int i = 0; i < menuItems.Count; ++i)
			{
				PubMenuItemData item = menuItems[i];
				PubMenuItemData.MenuItemType type = item.type;
				menuItemsMap[(int)type].Add(item);
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public PubMenuItemData GetRandomItem(PubMenuItemData.MenuItemType type)
	{
		int index = Random.Range(0, menuItemsMap[(int)type].Count);
		return menuItemsMap[(int)type][index];
	}

	public PubMenuItemData GetItem(PubMenuItemData.MenuItemType type, PubMenuItemData.MenuItemEnum item)
	{
		List<PubMenuItemData> itemsOfType = menuItemsMap[(int)type];
		int index = itemsOfType.FindIndex(i => i.item == item);
		if (index == -1)
			return null;

		return itemsOfType[index];
	}
}
