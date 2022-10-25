using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOptions : MonoBehaviour
{
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

    public bool WantsToOrder()
    {
        return DrinkItem != Drink.NONE || FoodItem != Food.NONE;
    }

	public string FoodNameGet()
	{
		return food != Food.NONE ? foodNames[(int)food] : "";
	}

	public string DrinkNameGet()
	{
		return drink != Drink.NONE ? drinkNames[(int)drink] : "";
	}

}
