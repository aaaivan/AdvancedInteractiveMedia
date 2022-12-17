using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PubMenuItem : MonoBehaviour, InteractableObject
{
	[SerializeField]
	TMP_Text destTable;
	GameObject tableUI;

	[SerializeField]
	Transform progressBar;
	GameObject progressUI;


	bool interactable = true;
	public bool Interactable 
	{
		get { return interactable; }
		set { interactable = value; }
	}

	PubMenuItemData itemData;
	public PubMenuItemData ItemData { get { return itemData; } }

	float consumptionProgression = 1.0f;
	public float ConsumptionProgression
	{
		get { return consumptionProgression; }
		set { consumptionProgression = value; }
	}

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
		tableUI = transform.Find("TableNumber").gameObject;
		tableUI.SetActive(false);
		progressUI = transform.Find("ProgressBar").gameObject;
		progressUI.SetActive(false);
	}

	public void ShowTableUI(int tableNum)
	{
		destTable.text = tableNum.ToString();
		tableUI.SetActive(true);
	}

	public void HideTableUI()
	{
		tableUI.SetActive(false);
	}

	public void ShowProgressUI()
	{
		progressUI.SetActive(true);
	}

	public void HideProgressUI()
	{
		progressUI.SetActive(false);
	}

	private void Update()
	{
		progressBar.transform.localScale = new Vector3(consumptionProgression, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
	}
}
