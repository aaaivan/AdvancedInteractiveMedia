using Fluent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour, InteractableObject
{
   public void DoInteraction()
	{
		DialogueManager.Instance.ExecuteAction(GetComponentInChildren<FluentScript>());
	}
}
