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

	[SerializeField]
	RectTransform inventoryOptionsUI;
	[SerializeField]
	RectTransform inventoryOptionsPanel;
	[SerializeField]
	TMP_Text optionPrefab;
	[SerializeField]
	Transform player;
	Transform inventory;

	public delegate void ItemRemovedHandler(PubMenuItem i);

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
			inventory = player.Find("Inventory");
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public bool AddToInventory(PubMenuItem item)
	{
		if (inventory.childCount >= maxCapacity)
			return false;

		item.transform.SetParent(inventory, false);
		return true;
	}

	bool RemoveFromInventory(PubMenuItem item, Transform newParent)
	{
		int index = -1;
		foreach(Transform t in inventory)
		{
			++index;
			if (t.GetChild(index) == item.transform)
				break;
		}

		if (index == inventory.childCount)
			return false;

		if(newParent != null)
			item.transform.SetParent(newParent, false);
		else
			Destroy(item.gameObject);

		return true;
	}

	public void RemoveFromInventoryByCondition(Func<PubMenuItem, bool> condition, Transform newParent, ItemRemovedHandler callback)
	{
		List<PubMenuItem> matchingItems = new List<PubMenuItem>();
		foreach (Transform t in inventory)
		{
			PubMenuItem item = t.GetComponent<PubMenuItem>();
			if (item != null && condition(item))
				matchingItems.Add(item);
		}

		if (matchingItems.Count == 0)
			return;

		if(matchingItems.Count == 1)
		{
			RemoveFromInventory(matchingItems[0], newParent);
			if(callback!= null)
				callback(matchingItems[0]);

			return;
		}

		inventoryOptionsUI.gameObject.SetActive(true);
		InputsManager.Instance.EnableInputsByType(InputsManager.InputsType.Inventory);

		foreach (Transform child in inventoryOptionsPanel.transform)
		{
			Destroy(child.gameObject);
		}
		foreach (PubMenuItem item in matchingItems)
		{
			GameObject option = Instantiate(optionPrefab.gameObject, inventoryOptionsPanel.transform);
			TMP_Text text = option.GetComponent<TMP_Text>();
			text.text = "Put down the " + item.ItemData.itemName;
			Button button = option.GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				InputsManager.Instance.DisableInputsByType(InputsManager.InputsType.Inventory);
				RemoveFromInventory(item, newParent);
				if (callback != null)
					callback(item);
				inventoryOptionsUI.gameObject.SetActive(false);
			});
		}
	}

}
