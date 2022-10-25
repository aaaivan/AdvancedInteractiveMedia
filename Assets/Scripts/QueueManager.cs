using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class QueueManager : MonoBehaviour
{
	List<Vector3> queuePositions = new List<Vector3>();
	List<CustomerAI> customers = new List<CustomerAI>();
	[SerializeField]
	float spacing = 1;
	public float Spacing
	{
		get { return spacing; }
	}

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

	public void Awake()
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

	public void QueuUp( CustomerAI c )
	{
		if (!customers.Contains(c) && c.GetComponent<NavMeshAgent>() != null)
		{
			Vector3 startPosition = transform.position;
			Vector3 targetPosition = startPosition - transform.forward * queuePositions.Count * spacing;
			customers.Add(c);
			c.State = CustomerAI.QueuingState.MovingToQueue;
			NavMeshAgent agent = c.GetComponent<NavMeshAgent>();
			agent.SetDestination(targetPosition);
			agent.isStopped = false;
		}
	}

	public void OnQueueLocationReached(CustomerAI c)
	{
		int position = customers.FindIndex(x => x == c);

		if(position > queuePositions.Count)
		{
			CustomerAI temp = customers[queuePositions.Count];
			customers[queuePositions.Count] = customers[position];
			customers[position] = temp;
		}

		c.State = CustomerAI.QueuingState.Queuing;
		queuePositions.Add(c.GetComponent<NavMeshAgent>().destination);
		UpdateQueuePriorities();

		Vector3 startPosition = transform.position;
		Vector3 targetPosition = startPosition - transform.forward * queuePositions.Count * spacing;
		for (int i = queuePositions.Count; i < customers.Count; ++i)
		{
			NavMeshAgent ag = customers[i].GetComponent<NavMeshAgent>();
			ag.SetDestination(targetPosition);
			ag.isStopped = false;
		}
	}
	
	public void RemoveFromQueue(CustomerAI c)
	{
		if (customers.Contains(c))
		{
			int position = customers.FindIndex(x => x == c);
			customers.Remove(c);
			c.State = CustomerAI.QueuingState.LeavingQueue;

			if (position < queuePositions.Count)
			{
				queuePositions.RemoveAt(queuePositions.Count - 1);

				for (int i = position; i < queuePositions.Count; ++i)
				{
					NavMeshAgent ag = customers[i].GetComponent<NavMeshAgent>();
					ag.SetDestination(queuePositions[i]);
					ag.isStopped = false;
				}

				Vector3 startPosition = transform.position;
				Vector3 targetPosition = startPosition - transform.forward * queuePositions.Count * spacing;
				for (int i = queuePositions.Count; i < customers.Count; ++i)
				{
					NavMeshAgent ag = customers[i].GetComponent<NavMeshAgent>();
					ag.SetDestination(targetPosition);
					ag.isStopped = false;
				}
			}
			UpdateQueuePriorities();
			NavMeshAgent agent = c.GetComponent<NavMeshAgent>();
			agent.SetDestination(c.StartPoition);
			agent.isStopped = false;
		}
	}

	private void UpdateQueuePriorities()
	{
		int i = 0;
		for ( ; i < queuePositions.Count; ++i)
		{
			customers[i].GetComponent<NavMeshAgent>().avoidancePriority = queuePositions.Count - 1 - i;
		}
		for (; i < customers.Count; ++i)
		{
			customers[i].GetComponent<NavMeshAgent>().avoidancePriority = 99;
		}
	}
}
