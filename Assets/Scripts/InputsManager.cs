using Fluent;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsManager : MonoBehaviour
{
	public DialogueInputManager dialogueInputManger;
	public FirstPersonController firstPersonController;


	static InputsManager instance;
	public static InputsManager Instance
	{
		get
		{
			if (instance == null)
				throw new UnityException("You need to add an InputsManager to your scene");
			return instance;
		}
	}

	private void OnDestroy()
	{
		instance = null; 
	}

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != null)
		{
			Destroy(gameObject);
		}
	}
}
