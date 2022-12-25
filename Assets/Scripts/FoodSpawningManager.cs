using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawningManager : MonoBehaviour
{
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
			foreach(Transform t in transform)
			{
				spawnLocations.Add(t);
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public bool TrySpawnFood(PubMenuItemData food, int tableNum)
	{
		Transform t = GetNextFreeSpot();
		if(t == null)
			return false;

		GameObject go = PubMenuItem.InstatiateItem(food, t);
		PubMenuItem fdi = go.GetComponent<PubMenuItem>();
		fdi.ShowTableUI(tableNum);
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
