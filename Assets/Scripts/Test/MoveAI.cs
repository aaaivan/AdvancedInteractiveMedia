using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAI : MonoBehaviour
{
	public Transform destination;
	NavMeshAgent agent;
	Animator animator;

	Vector2 speed;
	Vector2 smoothDeltaPosition;
	public float walkToIdleSpeedTrashhold = 0.1f;

    // Start is called before the first frame update
    void Awake()
    {
        agent= GetComponent<NavMeshAgent>();
		agent.SetDestination(destination.position);

		animator= GetComponent<Animator>();
		animator.applyRootMotion= true;
		agent.updatePosition = false;
		agent.updateRotation = true;
    }

	private void OnAnimatorMove()
	{
		Vector3 rootPos = animator.rootPosition;
		rootPos.y = agent.nextPosition.y;
		transform.position = rootPos;
		agent.nextPosition= rootPos;
	}
	
	void SyncAnimationWithAgent()
	{
		Vector3 deltaPos = agent.nextPosition - transform.position;
		deltaPos.y = 0;

		float dr = Vector3.Dot(deltaPos, transform.right);
		float df = Vector3.Dot(deltaPos, transform.forward);
		Vector2 localDeltaPos = new Vector2(dr, df);

		float smooth = Mathf.Min(1f, Time.deltaTime / 0.1f);
		smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, localDeltaPos, smooth);

		speed = smoothDeltaPosition / Time.deltaTime;
		if(agent.remainingDistance <= agent.stoppingDistance)
		{
			speed = Vector2.Lerp(
				Vector2.zero,
				speed,
				agent.remainingDistance/agent.stoppingDistance
			);
		}

		bool shouldMove = speed.magnitude > walkToIdleSpeedTrashhold &&
			agent.remainingDistance > agent.stoppingDistance;

		animator.SetBool("IsWalking", shouldMove);
		animator.SetFloat("Velocity", speed.magnitude);

		if (deltaPos.magnitude > agent.radius / 2f)
		{
			transform.position = Vector3.Lerp(
				animator.rootPosition,
				agent.nextPosition,
				smooth);
		}
	}

	// Update is called once per frame
	void Update()
	{
		SyncAnimationWithAgent();
		//if (agent.remainingDistance < 0.05)
		//{
		//	Quaternion targetRotation = Quaternion.LookRotation(destination.forward, destination.up);
		//	transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 10 * Time.deltaTime);
		//}
	}
}
