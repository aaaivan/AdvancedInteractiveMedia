using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOrderDialog : FluentScript
{
	CustomerAI customer;
	private new void Awake()
	{
		customer = GetComponent<CustomerAI>();
	}

	public override FluentNode Create()
	{
		return
			Show() * 
			Write("Test Test Test Test").WaitForButton() * 
			Do(()=>customer.LeaveQueue()) *
			Hide();
	}
}
