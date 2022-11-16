using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fluent
{
	public class DialogueManager : MonoBehaviour
	{
		[AddComponentMenu("Fluent/Custom Fluent Manager")]

		List<FluentScript> possibleDialogues = new List<FluentScript>();
		FluentScript activeDialogue = null;


		static DialogueManager instance;
		public static DialogueManager Instance
		{
			get
			{
				if (instance == null)
					throw new UnityException("You need to add an FluentManager to your scene");
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

		public bool AddScript(FluentScript fluentScript)
		{
			// Dont add any game actions that are not allowed to be added while active
			if (activeDialogue != null)
				return false;

			if (fluentScript == null)
			{
				Debug.LogWarning("You are trying to add a null action to the action manager", this);
				return false;
			}

			if (possibleDialogues.Contains(fluentScript))
				return true;

			// set the initiator
			possibleDialogues.Add(fluentScript);

			return true;
		}

		public void RemoveScript(FluentScript fluentScript)
		{
			possibleDialogues.Remove(fluentScript);
		}

		public void ExecuteAction(FluentScript gameAction)
		{
			if (activeDialogue != null)
			{
				Debug.LogWarning("This FluentScipt is already active " + gameAction.GetType().Name, gameAction);
				return;
			}
			else/* if (possibleDialogues.Contains(gameAction))*/
			{
				gameAction.SetDoneCallback(ActionCompleted);
				activeDialogue = gameAction;

				gameAction.Run();
			}
		}

		private void ActionCompleted(FluentScript fluentScript)
		{
			activeDialogue = null;
		}

		public FluentScript ActiveDialogueGet()
		{
			return activeDialogue;
		}
	}
}
