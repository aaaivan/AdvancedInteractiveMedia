using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WrongOrderDialog : FluentScript
{
	Table table;
	bool singleCustomer;
	PubMenuItemData wrongFood;

	private new void Awake()
	{
		table = GetComponent<Table>();
	}

	public override void OnStart()
	{
		base.OnStart();
		singleCustomer = table.GetNumberOfCustomers() == 1;
	}

	public void SetWrongFood(PubMenuItemData f)
	{
		wrongFood = f;
	}

	public override FluentNode Create()
	{
		return
			Show() *
			Do(() => DialogManager.Instance.SetSpeaker(singleCustomer ? "Customer" : "Customers")) *
			Write(Eval(() => (singleCustomer ? "I" : "We") + " did not order " + wrongFood.itemName + ".")).WaitForButton() *
			Hide();
	}
}
