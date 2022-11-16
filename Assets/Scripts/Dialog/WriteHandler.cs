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

        bool isTyping = false;
        WriteNode currentNode;
        TextMeshProUGUI currentTextUI;

        private IEnumerator TypeText()
        {
            // Get the text component we are using to write 
            TextMeshProUGUI textTextUI = currentTextUI;
            List<string> tagStack = new List<string>();

            int currentPosition = 0;
            string allText = currentNode.Text;

            textTextUI.text = allText;

            if (allText.Length > 0)
            {
                isTyping = true;
				while (true)
				{
					if (CharacterPauseSeconds > 0)
					{ 
						yield return new WaitForSeconds(CharacterPauseSeconds);
					}

                    currentPosition++;
                    textTextUI.maxVisibleCharacters = currentPosition;
                    if (currentPosition >= allText.Length)
                    {
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

            if (currentNode.WaitForButtonPress && !buttonRequestedButNotSpecified)
            {
                ShowButton();
            }
			else if (currentNode.WaitForButtonPress)
			{
			}
			else
			{
				StartCoroutine("Pause");
			}

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

        public void StopTyping()
        {
            // When we stop for the first time we just write out all the text
            if (isTyping)
            {
                isTyping = false;
                StopCoroutine("TypeText");
                currentTextUI.GetComponent<TextMeshProUGUI>().maxVisibleCharacters = currentTextUI.text.Length;

                if (!currentNode.WaitForButtonPress)
                    StartCoroutine("Pause");
                else if (Button != null)
                    ShowButton();

                return;
            }

            // The player needs to press a button to continue
            if (currentNode.WaitForButtonPress && Button != null)
			{
				return;
			}

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

            // Set the text component to be the selected component
            EventSystem.current.SetSelectedGameObject(textTextUI.gameObject);

            StartCoroutine("TypeText");
        }

        public override void Interrupt(FluentNode fluentNode)
        {
            Debug.Log("Interrupt write");
            // 

        }
    }
}
