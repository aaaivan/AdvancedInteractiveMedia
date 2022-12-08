using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

public class CustomerMovementController : MonoBehaviour
{
	Transform destination = null;
	NavMeshAgent agent = null;
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
			Vector3 rootPos = animator.rootPosition;
			rootPos.y = agent.nextPosition.y;
			transform.position = rootPos;
			agent.nextPosition = rootPos;
		}
		else if(isRotating)
		{
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

		while(!agent.hasPath)
		{
			yield return null;
		}

		while (isMoving)
		{
			Vector3 deltaPos = agent.nextPosition - transform.position;
			deltaPos.y = 0;

			float dr = Vector3.Dot(deltaPos, transform.right);
			float df = Vector3.Dot(deltaPos, transform.forward);
			Vector2 localDeltaPos = new Vector2(dr, df);

			float smooth = Mathf.Min(1f, Time.deltaTime / 0.1f);
			smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, localDeltaPos, smooth);

			speed = smoothDeltaPosition / Time.deltaTime;
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

			animator.SetBool("IsWalking", shouldMove);
			animator.SetFloat("Velocity", speed.magnitude);

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
				agent.ResetPath();
				agent.updateRotation = false;
				isMoving = false;
			}
			yield return null;
		}
		StartCoroutine(RotateCoroutine());
		StopCoroutine(MovementCoroutine());
		yield return null;
	}

	IEnumerator RotateCoroutine()
	{
		isRotating = true;
		smoothDeltaAngle = 0;

		while (isRotating)
		{
			// calculate the angle between the current orientation and the target orientation
			float angle = Vector3.Angle(transform.forward, destination.forward);
			int sign = Vector3.Cross(transform.forward, destination.forward).y > 0 ? 1 : -1;

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

		isRotating = false;
		smoothDeltaAngle = 0;
		destination = null;

		yield return null;
	}

	public void SetDestination(Transform _destination)
	{
		destination = _destination;
		agent.SetDestination(destination.position);
		StartCoroutine(MovementCoroutine());
	}

	void Update()
	{

	}
}
