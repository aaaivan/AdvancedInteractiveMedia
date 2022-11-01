using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersManager : MonoBehaviour
{
	OrdersManager instance;
	public OrdersManager Instance
	{
		get
		{
			if (instance == null)
				throw new UnityException("You need to add a SceneTransitionManager to your scene");
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}

	public void AddToOrder(string food)
	{
		Debug.Log(food);
	}
}
