using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOrderDialog : FluentScript
{
	public override FluentNode Create()
	{
		return Show() * Write("Test Test Test Test").WaitForButton() * Hide();
	}
}
