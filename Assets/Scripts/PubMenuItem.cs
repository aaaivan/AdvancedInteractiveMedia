using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PubMenuItem : MonoBehaviour, InteractableObject
{
	[SerializeField]
	TMP_Text destTable;
	Canvas tableUI;

	bool interactable = true;
	public bool Interactable 
	{
		get { return interactable; }
		set { interactable = value; }
	}

	PubMenuItemData itemData;
	public PubMenuItemData ItemData { get { return itemData; } }


	static public GameObject InstatiateItem(PubMenuItemData item, Transform parent)
	{
		GameObject go = Instantiate(item.prefab, parent);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.GetComponent<PubMenuItem>().Initialise(item);
		return go;
	}

	public void DoInteraction(bool primary)
	{
		if (!interactable)
			return;

		PlayerInventory.Instance.AddToInventory(this);
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
