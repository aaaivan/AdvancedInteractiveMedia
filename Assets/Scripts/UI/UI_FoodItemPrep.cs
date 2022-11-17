using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FoodItemPrep : MonoBehaviour
{
	[SerializeField]
	Slider slider;
	[SerializeField]
	TMP_Text itemName;
	[SerializeField]
	string itemString = "T{0} - {1}";

	CafeMenuItem foodItem;
	float prepStartTime = 0;
	bool isReady = true;
	int tableNumber = 0;
	public int TableNumber { get { return tableNumber; } }

	public delegate void FoodReady(GameObject foodItem);
	static public event FoodReady OnFoodReady;

	private void Awake()
	{
		slider.value = 0;
	}

	public void Initialise(CafeMenuItem _foodItem, int _tableNum)
	{
		foodItem = _foodItem;
		tableNumber = _tableNum;
		itemName.text = string.Format(itemString, tableNumber, foodItem.itemName);
		prepStartTime = Time.time;
		isReady = false;
	}

	private void Update()
	{
		if(!isReady)
		{
			float progress = Time.time - prepStartTime;
			if (progress >= foodItem.prepTime && OnFoodReady != null)
			{
				OnFoodReady.Invoke(gameObject);
				isReady = true;
			}

			if(isReady)
			{
				slider.value = 1;
			}
			else
			{
				slider.value = progress / foodItem.prepTime;
			}
		}
	}
}
