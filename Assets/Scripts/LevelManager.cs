using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[SerializeField]
	float spawnTimeInterval = 60;
	int nextCustomer = 0;
	float lastSpawnTime;

	[SerializeField]
	DrinksMat drinksMat;
	public DrinksMat DrinksMat { get { return drinksMat; } }
	[SerializeField]
	ExitTabletView exitTablet;
	public ExitTabletView ExitTablet { get { return exitTablet; } }
	[SerializeField]
	Transform entrance;
	[SerializeField]
	Transform exit;
	public Transform Exit { get { return exit; } }
	[SerializeField]
	List<Table> tables;
	[SerializeField]
	List<GameObject> customerPrefabs;

	float score = 0f;

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

			lastSpawnTime = float.MinValue;
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
					if (table == null || t.GetSize() < table.GetSize())
						table = t;
				}
			}
			if (table == null)
			{
				foreach (Table t in tables)
				{
					if (t.GetNumberOfFreeChairs() >= numCustomers)
					{
						if(table == null || t.GetNumberOfFreeChairs() < table.GetNumberOfFreeChairs())
							table = t;
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

	public void AddScore(float amount)
	{
		score += amount;
		Debug.Log(score.ToString());
	}
}
