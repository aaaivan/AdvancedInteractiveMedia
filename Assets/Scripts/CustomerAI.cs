using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : MonoBehaviour
{
	[SerializeField]
	Chair chair; // chair game object
	public Chair Chair { get { return chair; } }
	ChairTranslation chairPivot; // pivot of the chair that will be translated to move the chair
	Transform chairStandingPos; // location where the customer should be standing before sitting on the chair

	CustomerMovementController movementController;
	Animator animator;

	public CustomerMovementController MovementController { get { return movementController; } }

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
		GetComponent<CustomerMovementController>().OnFacingForward += OnTableReached;
	}

	private void OnDisable()
	{
		GetComponent<CustomerMovementController>().OnFacingForward -= OnTableReached;
	}

	private void Awake()
	{
		movementController = GetComponent<CustomerMovementController>();
		animator= GetComponent<Animator>();
		state = CustomerState.None;

		chairPivot = chair.transform.Find("Pivot").GetComponent<ChairTranslation>();
		chairStandingPos = chairPivot.transform.Find("StandingPos");
		chair.SetCustomer(transform);
	}

	private void Start()
	{
		QueueUp();
	}

	public void QueueUp()
	{
		QueueManager.Instance.AddToQueue(this);
	}

	public void LeaveQueue()
	{
		QueueManager.Instance.RemoveFromQueue(this, chairStandingPos, CustomerState.MovingToTable);
	}

	private void OnTableReached(CustomerMovementController controller)
	{
		if (state != CustomerState.MovingToTable)
			return;

		animator.SetTrigger("DoSitDown");
		chairPivot.TranslateToPosition(this, 4f);
	}

	private void Update()
	{
		if(Input.GetKeyUp(KeyCode.L) && state == CustomerState.FrontOfTheQueue) { LeaveQueue(); }
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
}
