using Fluent;
using System;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class CustomerDialogue : FluentScript
{
    [SerializeField]
    GameObject canvas;
	[SerializeField]
	TMP_Text speakerBox;

	CustomerOptions customerOptions;

	private void DisablePlayerMovement()
	{
		FirstPersonController fpc = InputsManager.Instance.firstPersonController;
		GameObject player = fpc.gameObject;
		StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
		if (Input.mousePresent)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			inputs.cursorLocked = false;
			inputs.cursorInputForLook = false;
		}

		fpc.DisableGameInputs();
	}

	private void EnablePlayerMovement()
	{
		FirstPersonController fpc = InputsManager.Instance.firstPersonController;
		GameObject player = fpc.gameObject;
		StarterAssetsInputs inputs = player.GetComponent<StarterAssetsInputs>();
		if (Input.mousePresent)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			inputs.cursorLocked = true;
			inputs.cursorInputForLook = true;
		}

		fpc.EnableGameInputs();
	}

	private void SetCustomerSpeaking()
	{
		speakerBox.text = "Customer";
	}

	private void SetWaiterSpeaking()
	{
		speakerBox.text = "You";
	}

	public override void OnStart()
    {
		customerOptions = GetComponent<CustomerOptions>();
		DisablePlayerMovement();
        base.OnStart();
    }

    public override void OnFinish()
    {
		EnablePlayerMovement();
        base.OnFinish();
    }

    public override FluentNode Create()
    {
		return
			Show(canvas) *
			Do(() => SetWaiterSpeaking()) *
			Write("Hi, would you like to order?").WaitForButton() *
			If(() => customerOptions.WantsToOrder(),
				If(() => customerOptions.FoodNameGet() == "",
					Do(() => SetCustomerSpeaking()) *
					Write("I don't really feel like eating anything. Can I just have some " + customerOptions.DrinkNameGet()).WaitForButton() *
					Do(() => SetWaiterSpeaking()) *
					Write("Sure!").WaitForButton()) *
				If(() => customerOptions.FoodNameGet() != "",
					Do(() => SetCustomerSpeaking()) *
					Write("Yes, can I have a " + customerOptions.FoodNameGet() + ", please?").WaitForButton() *
					Do(() => SetWaiterSpeaking()) *
					Write("Sure! Would you like a drink with your " + customerOptions.FoodNameGet() + "?").WaitForButton() *
					If(() => customerOptions.DrinkNameGet() != "",
						Do(() => SetCustomerSpeaking()) *
						Write(customerOptions.DrinkNameGet() + ", please.").WaitForButton() *
						Do(() => SetWaiterSpeaking()) *
						Write("No problem!").WaitForButton()
					)
				)*
				Do(() => SetWaiterSpeaking()) *
				Write(0f, "") *
				Show() *
				Options(
					Option("> Here is your order.") *
						Write("Here is your order.").WaitForButton()*
						Do(() => SetCustomerSpeaking()) *
						Write("Thank you.").WaitForButton() *
						End() *
					Option("> Sorry, we ran out of " + (customerOptions.FoodNameGet(true) == "" ? customerOptions.DrinkNameGet() : customerOptions.FoodNameGet(true)) + ".") *
						Write("Sorry, we ran out of " + (customerOptions.FoodNameGet(true) == "" ? customerOptions.DrinkNameGet() : customerOptions.FoodNameGet(true)) + ".").WaitForButton() *
						Do(() => SetCustomerSpeaking()) *
						Write("This is ridicolous!").WaitForButton() *
						End()
				) *
				Hide()
			) *
			If(() => !customerOptions.WantsToOrder(),
				Do(() => SetCustomerSpeaking()) *
				Write("No, thanks. I was just looking at the menu.").WaitForButton()
			) *
			Do(() => SetCustomerSpeaking()) *
			Write("Goodbye!").WaitForButton() *
			Do(() => SetWaiterSpeaking()) *
			Write("Bye!").WaitForButton() *
			Hide(canvas)
			;
	}
}
