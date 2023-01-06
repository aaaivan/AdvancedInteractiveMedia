using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingLevelManager : MonoBehaviour
{

	[SerializeField]
	Transform farFarAway;
	public Transform FarFarAway { get { return farFarAway; } }


	[SerializeField]
	Transform playerInventory;
	public Transform PlayerInventory { get { return playerInventory; } }

	[SerializeField]
	DrinksMat drinksMat;
	public DrinksMat DrinksMat { get { return drinksMat; } }

	[SerializeField]
	ExitTabletView exitTablet;
	public ExitTabletView ExitTablet { get { return exitTablet; } }

	[SerializeField]
	UI_OrderItemsList orderItemsList;
	public UI_OrderItemsList OrderItemsList { get { return orderItemsList; } }

	[SerializeField]
	TrainingTable table;
	public TrainingTable Table { get { return table; } }




	static TrainingLevelManager instance;
	public static TrainingLevelManager Instance { get { return instance; } }

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
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
