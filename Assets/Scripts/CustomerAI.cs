using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : MonoBehaviour
{
	Chair chair; // chair game object
	public Chair Chair{ get { return chair; } }
	Transform chairStandingPos; // location where the customer should be standing before sitting on the chair

	CustomerMovementController movementController;
	public CustomerMovementController MovementController { get { return movementController; } }

	Animator animator;

	float startQueuingTime = 0;
	public float StartQueuingTime
	{
		get { return startQueuingTime; }
		set { startQueuingTime = value; }
	}
	float sitAtTableTime = 0;
	public float SitAtTableTime { get { return sitAtTableTime; } }


	public enum CustomerState
	{
		None,
		MovingToQueue,
		Queuing,
		FrontOfTheQueue,
		MovingToTable,
		AtTheTable,
		MovingToExit
	}
	CustomerState state;
	public CustomerState State
	{
		get { return state; }
		set { state = value; }
	}

	private void OnEnable()
	{
		movementController.OnFacingForward += OnTableReached;
		movementController.OnDestinationReached += OnExitReached;
	}

	private void OnDisable()
	{
		movementController.OnFacingForward -= OnTableReached;
		movementController.OnDestinationReached -= OnExitReached;
	}

	private void Awake()
	{
		movementController = GetComponent<CustomerMovementController>();
		state = CustomerState.None;
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		QueueUp();
	}
	private void Update()
	{
		if(animator.GetBool("IsTalking") && animator.GetBool("IsSitting"))
		{
			int prob = (int)(30/Time.deltaTime);
			if (Random.Range(0, prob) == 0)
				animator.SetTrigger("DoLaugh");
		}
	}

	public void SetChair(Chair c)
	{
		if (chair != null && c != null)
			return;

		chair = c;
		chair.SetCustomer(transform);
		chairStandingPos = chair.transform.Find("Pivot/StandingPos");
		chair.OnStandUp += OnLeftChair;
	}

	void UnsetChair()
	{
		chair.OnStandUp -= OnLeftChair;
		chair.SetCustomer(null);
		chair = null;
	}

	public void QueueUp()
	{
		if (state != CustomerState.None)
			return;

		QueueManager.Instance.AddToQueue(this);
	}

	public void LeaveQueue()
	{
		if (state != CustomerState.Queuing && state != CustomerState.FrontOfTheQueue)
			return;

		QueueManager.Instance.RemoveFromQueue(this, chairStandingPos, CustomerState.MovingToTable);
		GetComponent<TipCalculator>().queuingTime = Time.time - startQueuingTime;
	}

	public void LeaveRestaurant()
	{
		if (state == CustomerState.AtTheTable)
		{
			state = CustomerState.MovingToExit;

			chair.MoveChairOffTheTable(this, 1f);
			TipCalculator tip = GetComponent<TipCalculator>();
			LevelManager.Instance.AddScore(tip.CalculateTip());
		}
		else if (state != CustomerState.MovingToExit)
		{
			UnsetChair();
			movementController.SetDestination(LevelManager.Instance.Exit.position, LevelManager.Instance.Exit.forward);
		}
	}

	void OnTableReached(CustomerMovementController controller)
	{
		if (state != CustomerState.MovingToTable)
			return;

		sitAtTableTime = Time.time;
		state = CustomerState.AtTheTable;
		chair.MoveChairCloseToTable(this, 2.5f);
	}

	public void OnAnimationSitDown()
	{
		Transform inventory = transform.Find("Inventory");
		for (int i = inventory.childCount - 1; i >= 0; --i)
		{
			PubMenuItem item = inventory.GetChild(i).GetComponent<PubMenuItem>();
			if(item != null)
				chair.FoodOnTable.AddFood(item);
		}
	}

	public void OnLeftChair(CustomerAI customer)
	{
		UnsetChair();
		movementController.SetDestination(LevelManager.Instance.Exit.position, LevelManager.Instance.Exit.forward);
	}

	public void OnExitReached(CustomerMovementController controller)
	{
		if(state != CustomerState.MovingToExit)
			return;

		Destroy(gameObject);
	}
}
