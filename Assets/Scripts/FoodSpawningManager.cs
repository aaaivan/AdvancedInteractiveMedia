using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawningManager : MonoBehaviour
{
	[SerializeField]
	List<Transform> spawnLocations = new List<Transform>();

	static FoodSpawningManager instance;
	static public FoodSpawningManager Instance
	{
		get { return instance; }
	}

	private void OnDestroy()
	{
		if(instance == this)
		{
			instance = null;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public bool TrySpawnFood(CafeMenuItem food, int tableNum)
	{
		Transform t = GetNextFreeSpot();
		if(t == null)
			return false;

		GameObject go = Instantiate(food.prefab, t);
		FoodDrinkItem fdi = go.GetComponent<FoodDrinkItem>();
		fdi.Initialise(tableNum);
		return true;
	}

	Transform GetNextFreeSpot()
	{
		foreach(var t in spawnLocations)
		{
			if(t.childCount == 0)
			{
				return t;
			}
		}
		return null;
	}
}
