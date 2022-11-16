using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProcessingStringAnimation : MonoBehaviour
{
	[SerializeField]
	string processingText = "Processing Payment {0}";
	TMP_Text textObj;
	int dotsNum = 0;

	IEnumerator coroutine = null;

	private void Awake()
	{
		textObj = GetComponent<TMP_Text>();
	}

	private void OnEnable()
	{
		coroutine = DotsAnimation();
		StartCoroutine(coroutine);
	}
	private void OnDisable()
	{
		StopCoroutine(coroutine);
		coroutine = null;
	}

	IEnumerator DotsAnimation()
	{
		float timeIncrements = 0.2f;
		while (true)
		{
			dotsNum = (dotsNum + 1) % 6;
			string dots = "";
			for (int i = 0; i < dotsNum; i++)
				dots += ".";
			textObj.text = string.Format(processingText, dots);
			yield return new WaitForSeconds(timeIncrements);
		}
	}
}
