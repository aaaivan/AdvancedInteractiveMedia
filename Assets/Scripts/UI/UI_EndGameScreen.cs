using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_EndGameScreen : MonoBehaviour
{
	[SerializeField]
	TMP_Text scoreText;
	string scoreString = "Your Score: £{0}";

	private void OnEnable()
	{
		InputsManager.Instance.EnableInputsByType(InputsManager.InputsType.UserInterface);
		scoreText.text = string.Format(scoreString, LevelManager.Instance.Score.ToString("0.00"));
	}
}
