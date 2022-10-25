using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ShowUiTrigger : MonoBehaviour
{
    Rigidbody rb;
    
    [SerializeField]
    Canvas canvas;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HideUi(other.gameObject.GetComponent<StarterAssetsInputs>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (Input.GetMouseButton((int)MouseButton.Left))
            {
                ShowUi(other.gameObject.GetComponent<StarterAssetsInputs>());
            }
        }
    }

    private void ShowUi(StarterAssetsInputs inputs)
    {
        canvas.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        inputs.cursorLocked = false;
        inputs.cursorInputForLook = false;
    }

    private void HideUi(StarterAssetsInputs inputs)
    {
        canvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputs.cursorLocked = true;
        inputs.cursorInputForLook = true;
    }
}
