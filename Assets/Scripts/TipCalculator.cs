using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipCalculator : MonoBehaviour
{
	CustomerOptions order;
	float baseTip = 0;
	float tipMultiplier = 1f;
	[HideInInspector]
	public float queuingTime = 0f;
	[HideInInspector]
	public float foodWaitTime = 0f;
	[HideInInspector]
	public int mistakes = 0;

	private void Awake()
	{
		order = GetComponent<CustomerOptions>();
	}

	public void SetBaseTip(float t)
	{
		baseTip = t;
	}

	public float CalculateTip()
	{
		// mistakes
		tipMultiplier -= 10 / 100 * mistakes;

		// queueing
		tipMultiplier -= 0.5f / 100 * Mathf.Max(0f, queuingTime - 60f);

		// waiting
		tipMultiplier -= 0.5f / 100  * Mathf.Max(0f, foodWaitTime - 60f);

		tipMultiplier = Mathf.Clamp01(tipMultiplier);

		return baseTip * tipMultiplier;
	}
}
