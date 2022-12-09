using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Apple;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public class CustomerMovementController : MonoBehaviour
{
	List<Tuple<Vector3, Vector3>> destinationsList = new List<Tuple<Vector3, Vector3>>();
	// Tuple.Item1 = position
	// Tuple.Item2 = forwardVector
	Tuple<Vector3, Vector3> destination = null;

	NavMeshAgent agent = null;
	public NavMeshAgent Agent { get { return agent; } }
	Animator animator = null;

	Vector2 speed = new Vector2(0, 0);
	Vector2 smoothDeltaPosition = new Vector2(0, 0);
	public float walkToIdleSpeedTrashhold = 0.1f;
	public float maxDistanceError = 0.25f;
	bool isMoving = false;

	float angularSpeed = 0f;
	float smoothDeltaAngle = 0;
	public float rotationSpeedMultiplier = 10f;
	public float rotateToIdleSpeedTrashhold = 0.1f;
	bool isRotating = false;

	public delegate void DestinationReachedHandler(CustomerMovementController c);
	public static event DestinationReachedHandler OnDestinationReached;

	void Awake()
    {
        agent= GetComponent<NavMeshAgent>();
		agent.updatePosition = false;
		agent.updateRotation = false;

		animator = GetComponent<Animator>();
		animator.applyRootMotion= true;
    }

	private void OnAnimatorMove()
	{
		if (isMoving && !agent.isStopped)
		{
			// apply the root motion to the NavMeshAgent
			Vector3 rootPos = animator.rootPosition;
			rootPos.y = agent.nextPosition.y;
			transform.position = rootPos;
			agent.nextPosition = rootPos;
		}
		else if(isRotating)
		{
			// Apply the rotation to the transform
			transform.rotation = animator.rootRotation;
			transform.position = animator.rootPosition;
		}
	}
	
	IEnumerator MovementCoroutine()
	{
		agent.updateRotation = true;
		speed = new Vector2(0, 0);
		smoothDeltaPosition = new Vector2(0, 0);
		isMoving = true;
		isRotating= false;

		while (isMoving)
		{
			while (!agent.hasPath)
			{
				// the path has not been calculated yet
				yield return null;
			}

			Vector3 deltaPos = agent.nextPosition - transform.position;
			deltaPos.y = 0;

			// calculate the delta position
			float dr = Vector3.Dot(deltaPos, transform.right);
			float df = Vector3.Dot(deltaPos, transform.forward);
			Vector2 localDeltaPos = new Vector2(dr, df);

			// smooth over time to prevent sudden changes
			float smooth = Mathf.Min(1f, Time.deltaTime / 0.1f);
			smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, localDeltaPos, smooth);

			// calculate the movement speed
			speed = smoothDeltaPosition / Time.deltaTime;

			// slow down if we have walked past the stopping distance
			if (agent.remainingDistance <= agent.stoppingDistance)
			{
				float decay = agent.remainingDistance / agent.stoppingDistance;
				speed = Vector2.Lerp(
					Vector2.zero,
					speed,
					decay * decay
				);
			}

			bool shouldMove = speed.magnitude > walkToIdleSpeedTrashhold &&
				agent.remainingDistance > agent.stoppingDistance;

			// set animator parameters
			animator.SetBool("IsWalking", shouldMove);
			animator.SetFloat("Velocity", speed.magnitude);

			// if the position error between the game object and the NavMeshAgent simulation
			// is too high, move the gameObject closer to the NavMeshAgent
			if (deltaPos.magnitude > agent.radius * maxDistanceError)
			{
				shouldMove = true;
				transform.position = Vector3.Lerp(
					animator.rootPosition,
					agent.nextPosition,
					smooth);
			}

			if (!shouldMove && agent.remainingDistance < agent.stoppingDistance)
			{
				// we have reached the destination
				if (destinationsList.Count > 1)
				{
					// move towards the next destination
					destinationsList.Remove(destination);
					destination = destinationsList[0];
					agent.SetDestination(destination.Item1);
				}
				else
				{
					// stop moving and rotate to face the forward vector
					agent.ResetPath();
					agent.updateRotation = false;
					isMoving = false;
					if(OnDestinationReached != null)
						OnDestinationReached.Invoke(this);
				}
			}
			yield return null;
		}

		animator.SetBool("IsWalking", false);
		animator.SetFloat("Velocity", 0.0f);
		StartCoroutine(RotateCoroutine());

		yield return null;
	}

	IEnumerator RotateCoroutine()
	{
		isRotating = true;
		smoothDeltaAngle = 0;

		while (isRotating)
		{
			// calculate the angle between the current orientation and the target orientation
			float angle = Vector3.Angle(transform.forward, destination.Item2);
			int sign = Vector3.Cross(transform.forward, destination.Item2).y > 0 ? 1 : -1;

			// caclulate the delta angle
			float dt = Mathf.Min(1f, Time.deltaTime * rotationSpeedMultiplier);
			float deltaRotation = Mathf.Lerp(0, sign * angle, dt);

			// smooth the delta angle
			float smooth = Mathf.Min(1f, Time.deltaTime / 0.1f);
			smoothDeltaAngle = Mathf.Lerp(smoothDeltaAngle, deltaRotation, smooth);

			// prevent overshooting
			smoothDeltaAngle = Mathf.Clamp(smoothDeltaAngle, -angle, angle);

			// calculate the angular speed
			angularSpeed = smoothDeltaAngle/Time.deltaTime;
			isRotating = Mathf.Abs(angularSpeed) > rotateToIdleSpeedTrashhold;

			// set animator parameters
			animator.SetBool("IsRotating", isRotating);
			animator.SetFloat("RotationSpeed", angularSpeed);

			yield return null;
		}

		// set animator parameters
		animator.SetBool("IsRotating", false);
		animator.SetFloat("RotationSpeed", 0.0f);
		smoothDeltaAngle = 0;

		yield return null;
	}

	public void SetDestination(Vector3 _destination, Vector3 _forwardVect)
	{
		destinationsList.Clear();
		destinationsList.Add(new Tuple<Vector3, Vector3>(_destination, _forwardVect));

		destination = destinationsList[0];
		agent.SetDestination(destination.Item1);
		StartCoroutine(MovementCoroutine());
	}

	public void SetNextDestination(Vector3 _destination, Vector3 _forwardVect)
	{
		if(destinationsList.Count == 0)
			SetDestination(_destination, _forwardVect);
		else
			destinationsList.Add(new Tuple<Vector3, Vector3>(_destination, _forwardVect));
	}
}
