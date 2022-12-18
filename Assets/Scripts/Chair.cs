using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Chair : MonoBehaviour
{
	[SerializeField]
	FoodOnTableManager foodOntable;
	public FoodOnTableManager FoodOnTable { get { return foodOntable; } }

	Table table;
	public Table Table { get { return table; } }

	Vector3 farPosition; // position when the chair is far from the table
	Vector3 closePosition; // position when the chair is close to the table
	Vector3 halfwayPosition = Vector3.zero;
	Transform pivot;

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
		farPosition = transform.position;
		closePosition = transform.Find("EndPos").position;
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

		// move the chair under the customer's butt
		Vector3 startPos = pivot.position;
		Transform target = customer.transform.Find("ChairPos");
		float endTime = Time.time + duration;
		while (true)
		{
			float t = (duration + Time.time - endTime) / duration;
			if (t > 1.0f)
				break;

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
		while (true)
		{
			float t = (duration + Time.time - endTime) / duration;
			if (t > 1.0f)
				break;

			pivot.position = Vector3.Lerp(startPos, closePosition, t);
			if ((pivot.position - closePosition).sqrMagnitude < 0.0001f)
			{
				pivot.position = closePosition;
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
		Vector3 initPos = pivot.position;
		float endTime = Time.time + duration;
		while(true)
		{
			float t = (duration + Time.time - endTime) / duration;
			if (t > 1.0f)
				break;

			pivot.position = Vector3.Lerp(initPos, halfwayPosition,t);
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
		yield return new WaitForSeconds(2.5f);

		// notify the chair that the customer is sitting on it
		SetIsCustomerSitting(false);

		// move the chair off the customer's butt
		duration = 1f;
		initPos = pivot.position;
		endTime = Time.time + duration;
		while (true)
		{
			float t = (duration + Time.time - endTime) / duration;
			if (t > 1.0f)
				break;

			pivot.position = Vector3.Lerp(initPos, farPosition, t);
			if ((pivot.position - farPosition).sqrMagnitude < 0.0001f)
			{
				pivot.position = farPosition;
				break;
			}
			yield return null;
		}
		
		yield return null;
	}
}
