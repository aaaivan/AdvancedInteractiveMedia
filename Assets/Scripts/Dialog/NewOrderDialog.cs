using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewOrderDialog : FluentScript, InteractableObject
{
	CustomerAI customer;
	Animator animator;
	CustomerOptions orderOptions;

	bool isReadyToPay = false;
	bool isOrderCorrect = false;
	bool hasPaid = false;

	string totalToPay = string.Empty;

	public delegate void OrderPaidHandler();
	public static event OrderPaidHandler OnOrderPaid;

	private void OnEnable()
	{
		UI_OrderItemsList.OnPaymentReady += PaymentHandler;
	}

	private void OnDisable()
	{
		UI_OrderItemsList.OnPaymentReady -= PaymentHandler;
	}

	private new void Awake()
	{
		customer = GetComponent<CustomerAI>();
		animator = GetComponent<Animator>();
		orderOptions= GetComponent<CustomerOptions>();
	}

	void PaymentHandler(List<CafeMenuItem> items, int tableNum, string total)
	{
		if (!FluentManager.Instance.GetActiveDialogs().Contains(this))
			return;

		isReadyToPay = true;
		isOrderCorrect = orderOptions.IsOrderMatching(items, tableNum);
		totalToPay = total;
	}

	public void DoInteraction()
	{
		if (customer.State != CustomerAI.CustomerState.FrontOfTheQueue)
			return;

		FluentManager.Instance.ExecuteAction(this);
	}

	public override FluentNode Create()
	{
		return
			Show() *
			Do(()=>DialogManager.Instance.SetSpeaker("You")) *
			Write("Hi, what's your table number?").WaitForButton() *

			Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
			Write("Table " + orderOptions.TableNumber.ToString() + ".").WaitForButton() *

			Do(() => DialogManager.Instance.SetSpeaker("You")) *
			Write("What can I get you?").WaitForButton() *

			Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
			Write(Eval(() => "Can I have " + orderOptions.GetOrderString() + ", please?")).WaitForButton() *

			Hide() *

			While(()=>!isOrderCorrect,
				ContinueWhen(() => isReadyToPay) *
				Show() *

				Do(() => DialogManager.Instance.SetSpeaker("You")) *
				Write(Eval(()=>"It's £" + totalToPay)).WaitForButton() *

				If(() => !isOrderCorrect,
					Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
					Write("The order is wrong.").WaitForButton() *
					Write(Eval(() => "Can I have " + orderOptions.GetOrderString() + " at table " + orderOptions.TableNumber.ToString() + ", please?")).WaitForButton()
				) *

				Do(() => isReadyToPay = false) *
				Hide()
			) *

			ContinueWhen(() => hasPaid) *
			If(() => OnOrderPaid != null, Do(() => OnOrderPaid.Invoke())) *

			Show() *
			Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
			Write(1.0f, "Thank you.") *
			
			Hide();
	}
}
