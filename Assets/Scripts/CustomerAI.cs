using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class CustomerAI : MonoBehaviour
{
	Vector3 startPosition;
	public Vector3 StartPosition{ get { return startPosition; } }
	
	NavMeshAgent agent;
	ThirdPersonCharacter character;

	public enum QueuingState
	{
		None,
		MovingToQueue,
		Queuing,
		FacingForward,
		LeavingQueue,
		ReturningToTable
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
		character = GetComponent<ThirdPersonCharacter>();
	}

	private void Start()
	{
		agent.updateRotation = false;

		// for now have all custome queue up as soon as the game starts
		QueueManager.Instance.QueuUp(this);
	}

	private void Update()
	{
		if(agent.hasPath)
		{
			QueueManager qm = QueueManager.Instance;
			if ( state == QueuingState.MovingToQueue && agent.remainingDistance < qm.Spacing * 0.5f )
			{
				qm.OnQueueLocationReached(this);
			}
			else if (state == QueuingState.Queuing && agent.remainingDistance < qm.Spacing * 0.2f)
			{
				qm.LookForward(this);
			}
			else if (state == QueuingState.FacingForward && agent.remainingDistance < qm.Spacing * 0.01f)
			{
				agent.isStopped = true;
			}
			else if (state == QueuingState.LeavingQueue && agent.remainingDistance < qm.Spacing)
			{
				state = QueuingState.ReturningToTable;
				agent.SetDestination(StartPosition);
			}
			else if (state == QueuingState.ReturningToTable && agent.remainingDistance < 0.1f)
			{
				state = QueuingState.None;
			}

			if (agent.remainingDistance > agent.stoppingDistance && !agent.isStopped)
			{
				character.Move(agent.desiredVelocity, false, false);
			}
			else
			{
				character.Move(Vector3.zero, false, false);
			}
		}
	}

	public void LeaveQueue()
	{
		QueueManager qm = QueueManager.Instance;
		qm.RemoveFromQueue(this);
	}

}
