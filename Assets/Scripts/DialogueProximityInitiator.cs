using UnityEngine;

namespace Fluent
{
	[RequireComponent(typeof(Collider), typeof(FluentScript))]
	public class DialogueProximityInitiator : MonoBehaviour
	{
		void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.tag == "Player")
			{
				DialogueManager.Instance.AddScript(GetComponent<FluentScript>());
			}
		}

		void OnTriggerExit(Collider collider)
		{
			if (collider.gameObject.tag == "Player")
			{
				DialogueManager.Instance.RemoveScript(GetComponent<FluentScript>());
			}
		}
	}
}
