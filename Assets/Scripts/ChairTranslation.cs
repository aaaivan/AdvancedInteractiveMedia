using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairTranslation : MonoBehaviour
{
	Transform startPosition; // position when the chair is far from the table
	Transform endPosition; // position when the chair is close to the table

	private void Awake()
	{
		startPosition = transform.parent;
		endPosition = transform.parent.Find("EndPos");
	}

	public void TranslateToPosition(CustomerAI customer, float duration)
	{
		StartCoroutine(MoveChairCoroutine(customer, duration));
	}

	IEnumerator MoveChairCoroutine(CustomerAI customer, float duration)
	{
		float endTime = Time.time + duration;
		Transform target = customer.transform.Find("ChairPos");

		// move the chair under the customer's butt
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f; )
		{
			Vector3 pos = target.position;
			pos.y = transform.position.y;
			transform.position = Vector3.Lerp(transform.position, pos, t * t * t * t);
			yield return null;
		}

		// parent the customer to the chair
		transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
		customer.transform.SetParent(transform);


		// move the chair and the customer under the table
		endTime = Time.time + duration;
		Vector3 pos2 = endPosition.position;
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f;)
		{
			transform.position = Vector3.Lerp(transform.position, pos2, t * t * t);
			yield return null;
		}

		// notify the chair that the customer is sitting on it
		transform.parent.GetComponent<Chair>().SetIsCustomerSitting(true);

		yield return null;
	}
}
