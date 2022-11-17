using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodDrinkItem : MonoBehaviour
{
	[SerializeField]
	TMP_Text destTable;

	public void Initialise(int tableNum)
	{
		destTable.text = tableNum.ToString();
	}
}
