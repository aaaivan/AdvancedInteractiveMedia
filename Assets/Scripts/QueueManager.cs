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
	const float alignmentDist = 0.5f;
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
			customers.Add(c);
			c.State = CustomerAI.QueuingState.MovingToQueue;
			NavMeshAgent agent = c.GetComponent<NavMeshAgent>();
			agent.SetDestination(NextPositionGet());
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
		NavMeshAgent agent = c.GetComponent<NavMeshAgent>();
		Vector3 newDest = agent.destination + transform.forward * spacing * alignmentDist;
		queuePositions.Add(newDest);
		UpdateQueuePriorities();

		for (int i = queuePositions.Count; i < customers.Count; ++i)
		{
			NavMeshAgent ag = customers[i].GetComponent<NavMeshAgent>();
			ag.SetDestination(NextPositionGet());
			ag.isStopped = false;
		}
	}
	
	public void LookForward(CustomerAI c)
	{
		NavMeshAgent agent = c.GetComponent<NavMeshAgent>();
		int position = customers.FindIndex(x => x == c);
		agent.SetDestination(queuePositions[position]);
		agent.isStopped = false;
		c.State = CustomerAI.QueuingState.FacingForward;
	}

	public void RemoveFromQueue(CustomerAI c)
	{
		StartCoroutine(RemoveFromQueueCoroutine(c));
	}

	IEnumerator RemoveFromQueueCoroutine(CustomerAI c)
	{
		if (customers.Contains(c))
		{
			int position = customers.FindIndex(x => x == c);
			customers.Remove(c);
			c.State = CustomerAI.QueuingState.LeavingQueue;

			NavMeshAgent agent = c.GetComponent<NavMeshAgent>();

			bool goRight = c.StartPosition.z > 0;
			agent.SetDestination(c.transform.right * 2 * spacing * (goRight ? 1 : -1));

			agent.isStopped = false;

			if (position < queuePositions.Count)
			{
				queuePositions.RemoveAt(queuePositions.Count - 1);
			}
			UpdateQueuePriorities();
			yield return new WaitForSeconds(3);

			for (int i = position; i < queuePositions.Count; ++i)
			{
				NavMeshAgent ag = customers[i].GetComponent<NavMeshAgent>();
				ag.SetDestination(queuePositions[i]);
				ag.isStopped = false;
			}
			for (int i = queuePositions.Count; i < customers.Count; ++i)
			{
				NavMeshAgent ag = customers[i].GetComponent<NavMeshAgent>();
				ag.SetDestination(NextPositionGet());
				ag.isStopped = false;
			}
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

	private Vector3 NextPositionGet()
	{
		Vector3 startPosition = transform.position;
		Vector3 targetPosition = startPosition - transform.forward * queuePositions.Count * spacing - transform.forward * spacing * alignmentDist;
		return targetPosition;
	}
}
