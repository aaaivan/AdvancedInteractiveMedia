#define IVAN_OVERRIDE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using TMPro;

namespace Fluent
{
    [AddComponentMenu("Fluent / Write Handler")] 
    [RequireComponent(typeof(OptionsPresenter))]
    public class WriteHandler : FluentNodeHandler
    {
        public TextMeshProUGUI TextUI;
        public float CharacterPauseSeconds = 0.03f;
		public Button Button;
		public Button CompleteTextButton;

		bool isTyping = false;
        WriteNode currentNode;
        TextMeshProUGUI currentTextUI;

#if IVAN_OVERRIDE //07-12-2022
		private void Start()
		{
			if (TextUI == null)
			{
				GameObject go = DialogManager.Instance.DialogWritePanel.gameObject;
				TextUI = go.GetComponent<TextMeshProUGUI>();
			}
			if (Button == null)
			{
				GameObject go = DialogManager.Instance.SkipButton.gameObject;
				Button = go.GetComponent<Button>();
			}
			if(CompleteTextButton == null)
			{
				GameObject go = DialogManager.Instance.CompleteTextButton.gameObject;
				CompleteTextButton = go.GetComponent<Button>();
			}
		}
#endif
		private IEnumerator TypeText()
        {
            // Get the text component we are using to write 
            TextMeshProUGUI textTextUI = currentTextUI;
            List<string> tagStack = new List<string>();

            int currentPosition = 0;
            string allText = currentNode.Text;

            textTextUI.text = allText;

#if IVAN_OVERRIDE //07-12-2022 implement instant write
			if(CharacterPauseSeconds <= 0)
			{
				currentPosition = allText.Length;
				textTextUI.maxVisibleCharacters = currentPosition;
				RemoveSkipListener();
			}
			else
#endif
			if (allText.Length > 0)
            {
                isTyping = true;
                while (true)
                {
                    yield return new WaitForSeconds(CharacterPauseSeconds);

                    currentPosition++;
                    textTextUI.maxVisibleCharacters = currentPosition;
                    if (currentPosition >= allText.Length)
                    {
						RemoveSkipListener();
						break;
                    }
                }
            }
            else
            {
                textTextUI.text = "";
            }

            isTyping = false;

            bool buttonRequestedButNotSpecified = currentNode.WaitForButtonPress && (Button == null);
			if (buttonRequestedButNotSpecified)
            {
                Debug.Log("You are trying to show a button for a Write() but you did not specify the Button UI element", gameObject);
            }
			if (!currentNode.WaitForButtonPress || buttonRequestedButNotSpecified)
            {
                StartCoroutine("Pause");
            }
            else
            {
                ShowButton();
            }
		}

		private void RemoveSkipListener()
        {
#if IVAN_OVERRIDE //18-12-2022
			CompleteTextButton.onClick.RemoveAllListeners();
#else
			currentTextUI.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
#endif
		}

		private void ShowButton()
        {
            // Show the button
            Button.gameObject.SetActive(true);

            // Give it focus
            EventSystem.current.SetSelectedGameObject(Button.gameObject);

            // Hookup the event handlers
            Button.onClick.AddListener(() =>
            {
                // Hide the button
                Button.gameObject.SetActive(false);

                // Disconnect the event
                Button.onClick.RemoveAllListeners();

                //
                currentNode.Done();
            });
        }


        private IEnumerator Pause()
        {
            yield return new WaitForSeconds(currentNode.SecondsToPause);
            currentNode.Done();
        }

		void StopTyping()
        {
            // When we stop for the first time we just write out all the text
            if (isTyping)
            {
                isTyping = false;
                StopCoroutine("TypeText");
                currentTextUI.GetComponent<TextMeshProUGUI>().maxVisibleCharacters = currentTextUI.text.Length;
				RemoveSkipListener();

				if (!currentNode.WaitForButtonPress)
                    StartCoroutine("Pause");
                else
					ShowButton();

                return;
            }

            // The player needs to press a button to continue
            if (currentNode.WaitForButtonPress)
                return;

            // If the node is stopped again we stop the pausing
            StopCoroutine("Pause");
            currentNode.Done();
        }

        public override void HandleFluentNode(FluentNode fluentNode)
        {
            // Store current node
            currentNode = fluentNode as WriteNode;

            // Check if the UI element is defined on the node itself
            if (currentNode.TextUIElement != null)
                currentTextUI = currentNode.TextUIElement;
            else
                currentTextUI = TextUI;

            // Get the text component we are using to write the text
            TextMeshProUGUI textTextUI = currentTextUI;
            textTextUI.maxVisibleCharacters = 0;

            if (!(currentTextUI.gameObject).activeSelf)
            {
                Debug.LogError("Did you forget to call Show() before Write() in your node chain ? The Write Node needs the element on to which text is written to be visible", this);
                return;
            }

#if IVAN_OVERRIDE //18-12-2022
			// Add the button listener so that text can be skipped
			CompleteTextButton.onClick.RemoveAllListeners();
			CompleteTextButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
			{
				// Do cleanup
				isTyping = false;
				StopCoroutine("TypeText");
				StopCoroutine("Pause");
				string nodeText = textTextUI.text;
				textTextUI.text = nodeText;
				textTextUI.maxVisibleCharacters = nodeText.Length;
				RemoveSkipListener();

				// Write's that require a button press to continue cannot be interrupted
				if (currentNode.WaitForButtonPress)
				{
					ShowButton();
					return;
				}

				FluentNode prevNode = currentNode;
				currentNode.Done();
				prevNode.IWasInterrupted();
			}));
#else
			// Add a button to the text if it doesnt have one
			if (currentTextUI.GetComponentInChildren<Button>() == null)
                currentTextUI.gameObject.AddComponent<Button>();

            // Add the button listener so that text can be skipped
            currentTextUI.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            currentTextUI.GetComponentInChildren<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                // Do cleanup
                isTyping = false;
                StopCoroutine("TypeText");
                StopCoroutine("Pause");
                string nodeText = textTextUI.text;
                textTextUI.text = nodeText;
                textTextUI.maxVisibleCharacters = nodeText.Length;
                RemoveSkipListener();

                // Write's that require a button press to continue cannot be interrupted
                if (currentNode.WaitForButtonPress)
                {
                    ShowButton();
                    return;
                }

                FluentNode prevNode = currentNode;
                currentNode.Done();
                prevNode.IWasInterrupted();
            }));
#endif
			// Set the text component to be the selected component
			EventSystem.current.SetSelectedGameObject(textTextUI.gameObject);
#if !IVAN_OVERRIDE //07-12-2022 prevent text from disappearing after it's been written. Remove it on next click!
            // Check if this is an instant write
            if (CharacterPauseSeconds == 0)
            {
                string nodeText = currentNode.Text;
                textTextUI.maxVisibleCharacters = nodeText.Length;
                textTextUI.text = currentNode.Text;
                if (currentNode.SecondsToPause != 0)
                    StartCoroutine("Pause");
                else
                    currentNode.Done();

                return;
            }
#endif
			StartCoroutine("TypeText");
        }

        public override void Interrupt(FluentNode fluentNode)
        {
            Debug.Log("Interrupt write");
        }
    }
}
