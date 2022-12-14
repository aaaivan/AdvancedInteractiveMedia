using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NewOrderDialog : FluentScript, InteractableObject
{
	CustomerAI customer;
	Animator animator;
	CustomerOptions orderOptions;

	bool isReadyToPay = false;
	bool isOrderCorrect = false;
	bool hasPaid = false;

	bool wasDrinkGiven = false;
	bool isDrinkCorrect = false;
	GameObject drinkGO = null;

	string totalToPay = string.Empty;

	public delegate void OrderPaidHandler();
	public static event OrderPaidHandler OnOrderPaid;

	const string speakingAnim = "IsTalking";
	const string paymentAnim = "DoPayment";

	private void OnEnable()
	{
		UI_OrderItemsList.OnPaymentReady += PaymentHandler;
		DrinksMat.OnDrinkPutOnMat += DrinkPickUpHandler;
	}

	private void OnDisable()
	{
		UI_OrderItemsList.OnPaymentReady -= PaymentHandler;
		DrinksMat.OnDrinkPutOnMat -= DrinkPickUpHandler;
	}

	private new void Awake()
	{
		customer = GetComponent<CustomerAI>();
		animator = GetComponent<Animator>();
		orderOptions= GetComponent<CustomerOptions>();
	}

	public override void OnStart()
	{
		base.OnStart();
		isReadyToPay = false;
		isOrderCorrect = false;
		hasPaid = false;
	}

	void PaymentHandler(List<PubMenuItemData> items, int tableNum, string total)
	{
		if (!FluentManager.Instance.GetActiveDialogs().Contains(this))
			return;

		isReadyToPay = true;
		isOrderCorrect = orderOptions.IsOrderMatching(items, tableNum);
		totalToPay = total;
	}

	void DrinkPickUpHandler(GameObject drink)
	{
		if (!FluentManager.Instance.GetActiveDialogs().Contains(this))
			return;

		wasDrinkGiven = true;
		isDrinkCorrect = false;
		PubMenuItem menuItem = drink.GetComponent<PubMenuItem>();
		if (menuItem == null)
			return;

		if(menuItem.ItemData.item == orderOptions.GetDrink().item)
		{
			isDrinkCorrect = true;
			drinkGO = drink;
		}
	}

	public void DoInteraction(bool primary)
	{
		if (customer.State != CustomerAI.CustomerState.FrontOfTheQueue)
			return;

		FluentManager.Instance.ExecuteAction(this);
	}

	public void OnAnimationButtonPressed()
	{
		hasPaid = true;
	}

	public override FluentNode Create()
	{
		return
			Show() *
			Do(() => DialogManager.Instance.SetSpeaker("You")) *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Write("Hi, what's your table number?").WaitForButton() *

			Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write("Table " + orderOptions.TableNumber.ToString() + ".").WaitForButton() *

			Do(() => DialogManager.Instance.SetSpeaker("You")) *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Write("What can I get you?").WaitForButton() *

			Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write(Eval(() => "Can I have " + orderOptions.GetOrderString() + ", please?")).WaitForButton() *

			Hide() *
			Do(() => animator.SetBool(speakingAnim, false)) *

			While(() => !isOrderCorrect,
				ContinueWhen(() => isReadyToPay) *
				Show() *

				Do(() => DialogManager.Instance.SetSpeaker("You")) *
				Do(() => animator.SetBool(speakingAnim, false)) *
				Write(Eval(() => "It's £" + totalToPay)).WaitForButton() *

				If(() => !isOrderCorrect,
					Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
					Do(() => animator.SetBool(speakingAnim, true)) *
					Write("The order is wrong.").WaitForButton() *
					Write(Eval(() => "Can I have " + orderOptions.GetOrderString() + " at table " + orderOptions.TableNumber.ToString() + ", please?")).WaitForButton()
				) *

				Do(() => isReadyToPay = false) *
				Hide() *
				Do(() => animator.SetBool(speakingAnim, false))
			) *

			Do(() => animator.SetTrigger(paymentAnim)) *
			Do(() => GameObject.Find("Tablet").GetComponentInChildren<ExitTabletView>().DoInteraction(true)) *
			ContinueWhen(() => hasPaid) *
			If(() => OnOrderPaid != null, Do(() => OnOrderPaid.Invoke())) *

			If(() => orderOptions.HasDrink(),
				If(() => !wasDrinkGiven,
					Show() *
					Do(() => DialogManager.Instance.SetSpeaker("You")) *
					Do(() => animator.SetBool(speakingAnim, false)) *
					Write("Let me get your drink.").WaitForButton() *
					Hide()
				) *

				While(() => !isDrinkCorrect,
					ContinueWhen(() => wasDrinkGiven) *
					Show() *

					If(() => !isDrinkCorrect,
						Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
						Do(() => animator.SetBool(speakingAnim, true)) *
						Write("This is the wrong drink.").WaitForButton() *
						Write(Eval(() => "Can I have some " + orderOptions.GetDrink().itemName + ", please?")).WaitForButton()
					) *

					Do(() => wasDrinkGiven = false) *
					Hide() *
					Do(() => animator.SetBool(speakingAnim, false))
				)
			) *
			// TODO: make drink not interactable anymore
			// TODO: add pickup gesture anymation and continue dialog at the end of gesture
			Do(() => { drinkGO.transform.parent = transform; drinkGO.SetActive(false); }) *

			Show() *
			Do(() => DialogManager.Instance.SetSpeaker("Customer")) *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write(2.0f, "Thank you.") *
			
			Hide() *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Do(() => customer.LeaveQueue());
	}
}
