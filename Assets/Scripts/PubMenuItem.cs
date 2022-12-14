using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PubMenuItem : MonoBehaviour
{
	[SerializeField]
	TMP_Text destTable;
	Canvas tableUI;

	PubMenuItemData itemData;
	public PubMenuItemData ItemData { get { return itemData; } }


	static public GameObject InstatiateItem(PubMenuItemData item, Transform parent)
	{
		GameObject go = Instantiate(item.prefab, parent);
		go.GetComponent<PubMenuItem>().Initialise(item);
		return go;
	}

	void Initialise(PubMenuItemData item)
	{
		itemData = item;
	}

	private void Awake()
	{
		tableUI = GetComponentInChildren<Canvas>(true);
		tableUI.gameObject.SetActive(false);
	}

	public void ShowTableUI(int tableNum)
	{
		destTable.text = tableNum.ToString();
		tableUI.gameObject.SetActive(true);
	}

	public void HideTableUI()
	{
		tableUI.gameObject.SetActive(false);
	}
}
