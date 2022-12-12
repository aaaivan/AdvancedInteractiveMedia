using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class QueueManager : MonoBehaviour
{
	[SerializeField]
	Transform[] queuePositions;
	[SerializeField]
	Transform rightQueueExitWaypoint = null;
	[SerializeField]
	Transform leftQueueExitWaypoint = null;

	List<CustomerAI> queuingCustomers = new List<CustomerAI>();
	int queueLength = 0;

	static QueueManager instance;
	public static QueueManager Instance
	{
		get
		{
			if (instance == null)
				throw new UnityException("You need to add a QueueManager to your scene");
			return instance;
		}
	}

	private void OnEnable()
	{
		CustomerMovementController.OnDestinationReached += OnQueueLocationReached;
	}

	private void OnDisable()
	{
		CustomerMovementController.OnDestinationReached -= OnQueueLocationReached;
	}

	private void OnDestroy()
	{
		if(instance == this)
			instance = null;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			queueLength = 0;
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}

	public void AddToQueue(CustomerAI customer)
	{
		if (queuingCustomers.Count >= queuePositions.Length)
			return;

		if (!queuingCustomers.Contains(customer) && customer.MovementController != null)
		{
			// register this customer as moving to the queue and set their destination
			queuingCustomers.Add(customer);
			customer.State = CustomerAI.CustomerState.MovingToQueue;
			Transform nextPosition = NextPositionGet();
			customer.MovementController.SetDestination(nextPosition.position, nextPosition.forward);
		}
	}

	private void OnQueueLocationReached(CustomerMovementController controller)
	{
		int position = queuingCustomers.FindIndex(x => x.MovementController == controller);

		if (position == -1)
			return;

		CustomerAI customer = queuingCustomers[position];
		// set the correct state for the customer
		if (customer.State == CustomerAI.CustomerState.MovingToQueue)
		{
			if (queueLength == 0)
				customer.State = CustomerAI.CustomerState.FrontOfTheQueue;
			else
				customer.State = CustomerAI.CustomerState.Queuing;
		}
		else if (customer.State == CustomerAI.CustomerState.Queuing && position == 0)
		{
			customer.State = CustomerAI.CustomerState.FrontOfTheQueue;
			return;
		}
		else
		{
			return;
		}

		// do a swap so that the customer who's just arrived is at the queueLength-th position
		CustomerAI temp = queuingCustomers[queueLength];
		queuingCustomers[queueLength] = queuingCustomers[position];
		queuingCustomers[position] = temp;
	
		// update the length of the queue
		++queueLength;

		// update the priorities of the navmesh agents
		UpdateQueuePriorities();

		// update the destination of the customers still walking to reach the queue
		for (int i = queueLength; i < queuingCustomers.Count; ++i)
		{
			Transform nextPosition = NextPositionGet();
			queuingCustomers[i].MovementController.SetDestination(nextPosition.position, nextPosition.forward);
		}
	}

	public void RemoveFromQueue(CustomerAI customer, Transform nextDestination, CustomerAI.CustomerState newState = CustomerAI.CustomerState.None)
	{
		int position = queuingCustomers.FindIndex(x => x);
		if (position == -1)
			return;

		// remove customer from the queue
		queuingCustomers.RemoveAt(position);
		--queueLength;

		// set the new state
		customer.State = newState;

		// move the custome leaving the queue to its new destination, via the queue exit waypoint
		Vector3 newDestDirection = nextDestination.position - customer.transform.position;
		newDestDirection.y = 0;
		bool goRight = Vector3.Cross(customer.transform.forward, newDestDirection).y > 0;
		Transform waypoint = goRight ? rightQueueExitWaypoint : leftQueueExitWaypoint;
		customer.MovementController.SetDestination(waypoint.position + customer.transform.position - queuePositions[0].position, waypoint.forward);
		customer.MovementController.SetNextDestination(nextDestination.position, nextDestination.forward);

		// advance customers behind the customer who left the queue
		StartCoroutine(QueueAdvancementCoroutine(position));
	}

	IEnumerator QueueAdvancementCoroutine(int removedPosition)
	{
		UpdateQueuePriorities();
		yield return new WaitForSeconds(2);

		// advance the customers in the queue from th eempty spot onwards
		int i = removedPosition;
		for ( ; i < queueLength; ++i)
		{
			CustomerMovementController controller = queuingCustomers[i].MovementController;
			controller.SetDestination(queuePositions[i].position, queuePositions[i].forward);
		}
		// update the destination of the customer walking towards the queue
		for ( ; i < queuingCustomers.Count; ++i)
		{
			CustomerMovementController controller = queuingCustomers[i].MovementController;
			Transform nextPosition = NextPositionGet();
			controller.SetDestination(nextPosition.position, nextPosition.forward);
		}
	}

	private void UpdateQueuePriorities()
	{
		int i = 0;
		// customers in the queue have decreasing priority values going from the front to the back of the queue
		// NOTE: lower priority value means higher importance (see NavMeshAgent.avoidancePriority documentation)
		for ( ; i < queueLength; ++i)
		{
			queuingCustomers[i].MovementController.Agent.avoidancePriority = queueLength - 1 - i;
		}
		// customers walking towards the queue have higher priority values than the ones in the queue
		for (; i < queuingCustomers.Count; ++i)
		{
			queuingCustomers[i].MovementController.Agent.avoidancePriority = 99 - queuingCustomers.Count - 1 + i;
		}
	}

	private Transform NextPositionGet()
	{
		if(queueLength >= queuePositions.Length)
			return null;

		return queuePositions[queueLength];
	}
}
