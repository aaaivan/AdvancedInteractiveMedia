using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOptions : MonoBehaviour
{
	[SerializeField]
	bool randomFood = false;
	[SerializeField]
	bool randomDrink = false;

	// Drinks
	List<string> drinkNames = new List<string>{"Water", "Coke", "Lemonade", "Milkshake"};
	public enum Drink
	{
		Water,
		Coke,
		Lemonade,
		Milkshake,

		NONE
	};
	[SerializeField]
	Drink drink = Drink.Water;
	public Drink DrinkItem => drink;

	// Food
	List<string> foodNames = new List<string> { "Burger", "Hot Dog", "Pizza", "Salad" };
	public enum Food
    {
        Burger,
        HotDog,
        Pizza,
        Salad,

        NONE
    };
    [SerializeField]
    Food food = Food.Burger;
    public Food FoodItem => food;

	private void Awake()
	{
		if(randomFood)
		{
			food = (Food)Random.Range(0, (int)Food.NONE);
		}
		if(randomDrink)
		{
			drink = (Drink)Random.Range(0, (int)Drink.NONE);
		}
	}

	public bool WantsToOrder()
    {
        return DrinkItem != Drink.NONE || FoodItem != Food.NONE;
    }

	public string FoodNameGet(bool plural = false)
	{
		string result = food != Food.NONE ? foodNames[(int)food] : "";
		if(result != "" && plural)
		{
			result = result + "s";
		}
		return result;
	}

	public string DrinkNameGet()
	{
		return drink != Drink.NONE ? drinkNames[(int)drink] : "";
	}

}
