using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
	[SerializeField]
	RectTransform dialogUi;
	public RectTransform DialogUi { get { return dialogUi; } }
	[SerializeField]
	RectTransform dialogWritePanel;
	public RectTransform DialogWritePanel { get { return dialogWritePanel; } }
	[SerializeField]
	RectTransform dialogOptionsPanel;
	public RectTransform DialogOptionsPanel { get { return dialogOptionsPanel; } }
	[SerializeField]
	RectTransform dialogSpeakerField;
	public RectTransform DialogSpeakerField { get { return dialogSpeakerField; } }

	static DialogManager instance;
	public static DialogManager Instance { get { return instance; } }

	private void OnDestroy()
	{
		if (instance == this)
			instance= null;
	}

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void SetSpeaker(string name)
	{
		dialogSpeakerField.GetComponentInChildren<TMP_Text>().text = name;
	}
}
