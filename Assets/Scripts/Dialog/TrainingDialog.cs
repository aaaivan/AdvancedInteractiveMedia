using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingDialog : FluentScript, InteractableObject
{
	Animator animator;
	bool sayAgain = false;

	const string speakingAnim = "IsTalking";

	enum Objectives
	{
		GetWaterFromFridge,
		PutWaterOnMat,
		PutWaterBack,
		OrderSalad,
		BringFoodToTable,

		Done
	}

	Objectives currentObjective = Objectives.Done;
	bool foodOrdered = false;

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
		animator = GetComponent<Animator>();
	}

	public override void OnFinish()
	{
		base.OnFinish();
		SceneManager.LoadScene(1);
	}

	public void DoInteraction(bool primary)
	{
		if(!FluentManager.Instance.GetActiveDialogs().Contains(this))
		{
			FluentManager.Instance.ExecuteAction(this);
		}
		else
		{
			sayAgain = true;
		}
	}

	bool IsCurrentObjectiveCompleted()
	{
		switch(currentObjective)
		{
			case Objectives.GetWaterFromFridge:
				return IsWaterInPlayerInventory();
			case Objectives.PutWaterOnMat:
				return IsWaterOnMat();
			case Objectives.PutWaterBack:
				return !(IsWaterInPlayerInventory() || IsWaterOnMat());
			case Objectives.OrderSalad:
				return foodOrdered;
			case Objectives.BringFoodToTable:
				return IsFoodOnTable();
			default:
				return false;
		}
	}

	bool IsWaterInPlayerInventory()
	{
		if (TrainingLevelManager.Instance.PlayerInventory.childCount > 0)
		{
			foreach (Transform t in TrainingLevelManager.Instance.PlayerInventory)
			{
				if (t.GetComponent<PubMenuItem>().ItemData.item == PubMenuItemData.MenuItemEnum.Water)
					return true;
			}
		}
		return false;
	}

	bool IsWaterOnMat()
	{
		DrinksMat mat = TrainingLevelManager.Instance.DrinksMat;
		GameObject itemGameObject = mat.CurrentDrink;

		if (itemGameObject == null)
			return false;

		PubMenuItem drink = itemGameObject.GetComponent<PubMenuItem>();
		if (drink == null)
			return false;

		if (drink.ItemData.item == PubMenuItemData.MenuItemEnum.Water)
			return true;

		return false;
	}

	void PaymentHandler(List<PubMenuItemData> items, int tableNum, string total)
	{
		if (!FluentManager.Instance.GetActiveDialogs().Contains(this))
			return;

		if (currentObjective != Objectives.OrderSalad)
			return;

		if(tableNum == 1 && items.Count == 1 && items[0].item == PubMenuItemData.MenuItemEnum.Salad)
		{
			foodOrdered = true;
			TrainingLevelManager.Instance.OrderItemsList.SubmitOrder();
		}
		else
		{
			sayAgain = true;
		}
	}

	bool IsFoodOnTable()
	{
		return TrainingLevelManager.Instance.Table.IsFoodOnTable();
	}

	public override FluentNode Create()
	{
		return
			Show() *
			Do(() => DialogManager.Instance.SetSpeaker("Manager")) *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write("Hello, I am the manager. I am here to help you get started with working in this restaurant").WaitForButton() *
			Write("Let's start simple. You can pick up a drink by getting close to a fridge and using the LEFT mouse button while aiming at it. You can put the drink back by using the RIGHT mouse button.").WaitForButton() *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Hide() *

			// take water from the fridge
			Do(() => currentObjective = Objectives.GetWaterFromFridge) *

			While(() => !IsCurrentObjectiveCompleted(),
				Show() *
				Do(() => animator.SetBool(speakingAnim, true)) *
				Write("Try to pick up a bottle of water. It will show up in the upper left corner of the screen. Come back to me when you have it.").WaitForButton() *
				Do(() => animator.SetBool(speakingAnim, false)) *
				Hide() *

				Do(() => sayAgain = false) *
				ContinueWhen(() => sayAgain) *
				Do(() => sayAgain = false)
			) *

			Show() *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write("Great! When a customer orders a drink, you can place it on the mat by the till.").WaitForButton() *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Hide() *

			// put water on the counter
			Do(() => currentObjective = Objectives.PutWaterOnMat) *

			While(() => !IsCurrentObjectiveCompleted(),
				Show() *
				Do(() => animator.SetBool(speakingAnim, true)) *
				Write("Put the water down on the mat by aiming at it and clicking with the mouse.").WaitForButton() *
				Do(() => animator.SetBool(speakingAnim, false)) *
				Hide() *

				Do(() => sayAgain = false) *
				ContinueWhen(() => sayAgain) *
				Do(() => sayAgain = false)
			) *

			Show() *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write("Good job! The customer will grab the drink from the mat if that's the one they ordered.").WaitForButton() *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Hide() *

			// put water back in the fridge
			Do(() => currentObjective = Objectives.PutWaterBack) *

			While(() => !IsCurrentObjectiveCompleted(),
				Show() *
				Do(() => animator.SetBool(speakingAnim, true)) *
				Write("Now try to put the water bottle from the mat back in the fridge. Remeber, LEFT click to pick up drinks from the fridge, RIGHT click to put them back.").WaitForButton() *
				Do(() => animator.SetBool(speakingAnim, false)) *
				Hide() *

				Do(() => sayAgain = false) *
				ContinueWhen(() => sayAgain) *
				Do(() => sayAgain = false)
			) *

			Show() *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write("Nicely done! When the restaurant opens you will need to place the order for the customers using the till.").WaitForButton() *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Hide() *

			// order salad
			Do(() => currentObjective = Objectives.OrderSalad) *

			While(() => !IsCurrentObjectiveCompleted(),
				Show() *
				Do(() => animator.SetBool(speakingAnim, true)) *
				Write("Try to order a SALAD at table 1. Enter the table first, then go to SIDES > SALAD and finally CONFIRM. You can exit the till view by clicking outside of the monitor.").WaitForButton() *
				Do(() => animator.SetBool(speakingAnim, false)) *
				Hide() *

				Do(() => sayAgain = false) *
				ContinueWhen(() => sayAgain) *
				Do(() => sayAgain = false)
			) *

			Show() *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write("Good! We are almost done. When the food is ready it will appear on the kitchen counter.").WaitForButton() *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Hide() *

			// bring salad to table
			Do(() => currentObjective = Objectives.BringFoodToTable) *

			While(() => !IsCurrentObjectiveCompleted(),
				Show() *
				Do(() => animator.SetBool(speakingAnim, true)) *
				Write("Pick up the salad and bring it to table one. Use mouse click to pick up / put down the food.").WaitForButton() *
				Do(() => animator.SetBool(speakingAnim, false)) *
				Hide() *

				Do(() => sayAgain = false) *
				ContinueWhen(() => sayAgain) *
				Do(() => sayAgain = false)
			) *

			Show() *
			Do(() => animator.SetBool(speakingAnim, true)) *
			Write("That's your induction completed, WELL DONE!").WaitForButton() *
			Write("Be aware that customers will tip you based on queuing time, time they waited for their food and accuracy of the order brought to them.").WaitForButton() *
			Write("Look, it's already time to open. Good luck!").WaitForButton() *
			Do(() => animator.SetBool(speakingAnim, false)) *
			Hide();
	}
}
