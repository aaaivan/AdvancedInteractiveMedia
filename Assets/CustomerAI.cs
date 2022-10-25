using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
	Vector3 startPosition;
	public Vector3 StartPoition{ get { return startPosition; } }
	NavMeshAgent agent;
	public enum QueuingState
	{
		None,
		MovingToQueue,
		Queuing,
		LeavingQueue
	}
	QueuingState state;
	public QueuingState State
	{
		get { return state; }
		set { state = value; }
	}


	private void Awake()
	{
		startPosition = transform.position;
		agent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		QueueManager.Instance.QueuUp(this);
	}

	private void Update()
	{
		QueueManager qm = QueueManager.Instance;
		if(agent.hasPath)
		{
			if ( state == QueuingState.MovingToQueue && agent.remainingDistance < qm.Spacing * 0.25f )
			{
				qm.OnQueueLocationReached(this);
			}
			else if (state == QueuingState.Queuing && agent.remainingDistance < qm.Spacing * 0.1f)
			{
				agent.isStopped = true;
			}
			else if (state == QueuingState.LeavingQueue && agent.remainingDistance < 0.1f)
			{
				state = QueuingState.None;
			}
		}
	}

	public void LeaveQueue()
	{
		QueueManager qm = QueueManager.Instance;
		qm.RemoveFromQueue(this);
	}

}
