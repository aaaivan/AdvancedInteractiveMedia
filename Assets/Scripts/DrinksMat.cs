using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinksMat : MonoBehaviour, InteractableObject
{
	public delegate void DrinkOnMatHandler(GameObject drink);
	public static event DrinkOnMatHandler OnDrinkPutOnMat;

	public GameObject CurrentDrink
	{
		get
		{ 
			return transform.childCount > 0 ? 
				transform.GetChild(0).gameObject :
				null;
		}
	}

	public void DoInteraction(bool primary)
	{
		if (!primary)
		{
			if (transform.childCount > 0)
				return;

			PlayerInventory.Instance.RemoveFromInventoryByType(true, AddDrinkToMat);
		}
	}

	void AddDrinkToMat(PubMenuItemData drink)
	{
		GameObject go = PubMenuItem.InstatiateItem(drink, transform);
		if(OnDrinkPutOnMat != null)
		{
			OnDrinkPutOnMat.Invoke(go);
		}
	}
}

