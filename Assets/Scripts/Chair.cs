using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Chair : MonoBehaviour
{
	[SerializeField]
	FoodOnTableManager foodOntable;
	public FoodOnTableManager FoodOnTable { get { return foodOntable; } }

	Table table;
	public Table Table { get { return table; } }

	Transform startPosition; // position when the chair is far from the table
	Transform endPosition; // position when the chair is close to the table
	Transform pivot;

	public delegate void SitDownHandler(CustomerAI c);
	public event SitDownHandler OnSitDown;

	Transform seatedCustomer = null;
	public Transform SeatedCustomer
	{ 
		get { return seatedCustomer; }
	}
	bool isCustomerSitting = false;
	public bool HasCustomerArrived
	{
		get { return isCustomerSitting; }
	}

	private void Awake()
	{
		startPosition = transform;
		endPosition = transform.Find("EndPos");
		pivot = transform.Find("Pivot");
		table = transform.parent.GetComponent<Table>();
	}

	public void SetCustomer(Transform _customer)
	{
		if(seatedCustomer == null && _customer != null)
		{
			isCustomerSitting = false;
			seatedCustomer = _customer;
		}
	}

	public void SetIsCustomerSitting(bool sitting)
	{
		if (isCustomerSitting == sitting)
			return;

		isCustomerSitting = sitting;
		if (isCustomerSitting && OnSitDown != null)
			OnSitDown.Invoke(seatedCustomer.GetComponent<CustomerAI>());
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
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f;)
		{
			Vector3 pos = target.position;
			pos.y = pivot.position.y;
			pivot.position = Vector3.Lerp(pivot.position, pos, t * t * t * t);
			if ((pivot.position - pos).sqrMagnitude < 0.0001f)
			{
				pivot.position = pos;
				break;
			}

			yield return null;
		}

		// parent the customer to the chair
		pivot.position = new Vector3(target.position.x, pivot.position.y, target.position.z);
		customer.transform.SetParent(pivot);


		// move the chair and the customer under the table
		endTime = Time.time + duration;
		Vector3 pos2 = endPosition.position;
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f;)
		{
			pivot.position = Vector3.Lerp(pivot.position, pos2, t * t * t);
			yield return null;
		}

		// notify the chair that the customer is sitting on it
		SetIsCustomerSitting(true);

		yield return null;
	}
}
