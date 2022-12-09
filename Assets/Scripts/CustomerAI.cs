using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAI : MonoBehaviour
{
	public Transform chair;
	public Transform exit;

	CustomerMovementController movementController;
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

	private void Awake()
	{
		movementController = GetComponent<CustomerMovementController>();
		state = CustomerState.None;
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
		QueueManager.Instance.RemoveFromQueue(this, chair, CustomerState.MovingToTable);
	}

	private void Update()
	{
		if(Input.GetKeyUp(KeyCode.L) && state == CustomerState.FrontOfTheQueue) { LeaveQueue(); }
	}
}
