using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
	Vector3 halfwayPosition = Vector3.zero;

	public delegate void SitDownHandler(CustomerAI c);
	public event SitDownHandler OnSitDown;
	public event SitDownHandler OnStandUp;

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
		else if(seatedCustomer != null && _customer == null)
		{
			isCustomerSitting = false;
			seatedCustomer = null;
		}
	}

	private void SetIsCustomerSitting(bool sitting)
	{
		if (isCustomerSitting == sitting)
			return;

		isCustomerSitting = sitting;
		if (isCustomerSitting && OnSitDown != null)
			OnSitDown.Invoke(seatedCustomer.GetComponent<CustomerAI>());
		else if (!isCustomerSitting && OnStandUp != null)
			OnStandUp.Invoke(seatedCustomer.GetComponent<CustomerAI>());
	}

	public void MoveChairCloseToTable(CustomerAI customer, float duration)
	{
		StartCoroutine(MoveChairCloseToTableCoroutine(customer, duration));
	}

	IEnumerator MoveChairCloseToTableCoroutine(CustomerAI customer, float duration)
	{
		Animator anim = customer.GetComponent<Animator>();
		if(anim != null)
			anim.SetBool("IsSitting", true);

		Vector3 startPos = pivot.position;
		float endTime = Time.time + duration;
		Transform target = customer.transform.Find("ChairPos");

		// move the chair under the customer's butt
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f;)
		{
			Vector3 pos = target.position;
			pos.y = pivot.position.y;
			pivot.position = Vector3.Lerp(startPos, pos, t);
			if ((pivot.position - pos).sqrMagnitude < 0.0001f)
			{
				pivot.position = pos;
				break;
			}

			yield return null;
		}

		// parent the customer to the chair
		customer.transform.SetParent(pivot);
		halfwayPosition = pivot.position;

		// notify the chair that the customer is sitting on it
		SetIsCustomerSitting(true);

		// move the chair and the customer under the table
		duration = 1f;
		startPos = pivot.position;
		endTime = Time.time + duration;
		Vector3 pos2 = endPosition.position;
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f;)
		{
			pivot.position = Vector3.Lerp(startPos, pos2, t);
			if ((pivot.position - pos2).sqrMagnitude < 0.0001f)
			{
				pivot.position = pos2;
				break;
			}
			yield return null;
		}

		yield return null;
	}

	public void MoveChairOffTheTable(CustomerAI customer, float duration)
	{
		StartCoroutine(MoveChairOffTheTableCoroutine(customer, duration));
	}

	IEnumerator MoveChairOffTheTableCoroutine(CustomerAI customer, float duration)
	{
		// move the chair and the customer off the table
		Vector3 startPosition = pivot.position;
		float endTime = Time.time + duration;
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f;)
		{
			pivot.position = Vector3.Lerp(startPosition, halfwayPosition,t);
			if ((pivot.position - halfwayPosition).sqrMagnitude < 0.0001f)
			{
				pivot.position = halfwayPosition;
				break;
			}
			yield return null;
		}
		
		// unparent the customer from the chair
		customer.transform.SetParent(null);
		
		// trigger stand up animation
		Animator anim = customer.GetComponent<Animator>();
		if (anim != null)
			anim.SetBool("IsSitting", false);

		// move the chair off the customer's butt
		startPosition = pivot.position;
		endTime = Time.time + duration;
		for (float t; (t = (duration + Time.time - endTime) / duration) < 1.0f;)
		{
			Vector3 pos = transform.position;
			pos.y = pivot.position.y;
			pivot.position = Vector3.Lerp(startPosition, pos, Mathf.Sqrt(t));
			if ((pivot.position - pos).sqrMagnitude < 0.0001f)
			{
				pivot.position = pos;
				break;
			}
			yield return null;
		}

		// notify the chair that the customer is sitting on it
		SetIsCustomerSitting(false);
		
		yield return null;
	}
}
