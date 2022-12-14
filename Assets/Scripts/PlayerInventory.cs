using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
	[SerializeField]
	int maxCapacity = 2;
	List<PubMenuItemData> items = new List<PubMenuItemData>();

	[SerializeField]
	RectTransform inventoryOptionsUI;
	[SerializeField]
	RectTransform inventoryOptionsPanel;
	[SerializeField]
	TMP_Text optionPrefab;

	public delegate void ItemRemovedHandler(PubMenuItemData i);

	static PlayerInventory instance;
	public static PlayerInventory Instance { get { return instance; } }

	private void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public bool AddToInventory(PubMenuItemData item)
	{
		if (items.Count >= maxCapacity)
			return false;

		items.Add(item);
		return true;
	}

	public bool RemoveFromInventory(PubMenuItemData item)
	{
		int index = items.FindIndex(i => i == item);

		if (index == -1)
			return false;

		items.RemoveAt(index);
		return true;
	}

	public void RemoveFromInventoryByType(bool drinkType, ItemRemovedHandler callback)
	{
		Func<PubMenuItemData, bool> cmp = (i) => { return (drinkType && i.type == PubMenuItemData.MenuItemType.Drink) || (!drinkType && i.type != PubMenuItemData.MenuItemType.Drink); };
		List<PubMenuItemData> matchingItems = new List<PubMenuItemData>();
		foreach (PubMenuItemData item in items)
		{
			if (cmp(item))
				matchingItems.Add(item);
		}

		if (matchingItems.Count == 0)
			return;

		if(matchingItems.Count == 1)
		{
			RemoveFromInventory(matchingItems[0]);
			callback(matchingItems[0]);
			return;
		}

		inventoryOptionsUI.gameObject.SetActive(true);
		InputsManager.Instance.DisableInputsByType(InputsManager.InputsType.Gameplay);

		foreach (Transform child in inventoryOptionsPanel.transform)
		{
			Destroy(child.gameObject);
		}
		foreach (PubMenuItemData item in matchingItems)
		{
			GameObject option = Instantiate(optionPrefab.gameObject, inventoryOptionsPanel.transform);
			TMP_Text text = option.GetComponent<TMP_Text>();
			text.text = item.itemName;
			Button button = option.GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				RemoveFromInventory(item);
				callback(item);
				inventoryOptionsUI.gameObject.SetActive(false);
				InputsManager.Instance.EnableInputsByType(InputsManager.InputsType.Gameplay);
			});
		}
	}

}
