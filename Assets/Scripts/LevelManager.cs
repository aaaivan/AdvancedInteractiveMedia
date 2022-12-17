using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[SerializeField]
	DrinksMat drinksMat;
	[SerializeField]
	Transform entrance;
	[SerializeField]
	Transform exit;
	[SerializeField]
	List<Table> tables;
	[SerializeField]
	List<GameObject> customerPrefabs;
	int nextCustomer = 0;
	float lastSpawnTime = -30;
	const float spawnTimeInterval = 30;
	public DrinksMat DrinksMat { get { return drinksMat; } }

	static LevelManager instance;
	public static LevelManager Instance { get { return instance; } }

	private void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

			// shuffle the custome prefabs so they get spawn in a random order every time
			for (int i = 0; i < customerPrefabs.Count; ++i)
			{
				var temp = customerPrefabs[i];
				int j = UnityEngine.Random.Range(0, customerPrefabs.Count);
				customerPrefabs[i] = customerPrefabs[j];
				customerPrefabs[j] = temp;
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Update()
	{
		if (Time.time > lastSpawnTime + spawnTimeInterval)
		{
			lastSpawnTime = Time.time;
			int numCustomers = UnityEngine.Random.Range(1, 3);
			numCustomers = Math.Min(numCustomers, customerPrefabs.Count - nextCustomer);

			Table table = null;
			foreach(Table t in tables)
			{
				if (t.IsEmpty() && t.GetNumberOfFreeChairs() >= numCustomers)
				{
					table = t;
					break;
				}
			}
			if (table == null)
			{
				foreach (Table t in tables)
				{
					if (t.GetNumberOfFreeChairs() >= numCustomers)
					{
						table = t;
						break;
					}
				}
			}

			if (table == null)
				return;

			StartCoroutine(SpawnCustomersCoroutine(table, numCustomers));
		}
	}

	IEnumerator SpawnCustomersCoroutine(Table table, int numberOfCustomers)
	{
		for (int i = 0; i < numberOfCustomers; ++i)
		{
			Vector3 spawnPosition = entrance.position - i * entrance.forward;
			GameObject go = Instantiate(customerPrefabs[nextCustomer++], spawnPosition, entrance.rotation);
			CustomerAI customer = go.GetComponent<CustomerAI>();
			Chair chair = table.GetFreeChair();
			customer.SetChair(chair);
			yield return new WaitForSeconds(2.0f);
		}
		yield return null;
	}
}
