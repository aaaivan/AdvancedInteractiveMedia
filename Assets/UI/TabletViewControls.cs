//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/UI/TabletViewControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @TabletViewControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @TabletViewControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""TabletViewControls"",
    ""maps"": [
        {
            ""name"": ""TabletView"",
            ""id"": ""fafa8e63-1f27-4bca-9d72-d07f44d97e71"",
            ""actions"": [
                {
                    ""name"": ""Exit"",
                    ""type"": ""Button"",
                    ""id"": ""30de03b1-fb89-404e-b8a8-2b3e2286d182"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""0b20f346-1350-4f27-a014-d01acdeb02b3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ceab8f7d-71a9-49fe-a6bd-5637481e0292"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Exit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""304fe884-f50e-4324-b070-ea4adf61e7f7"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // TabletView
        m_TabletView = asset.FindActionMap("TabletView", throwIfNotFound: true);
        m_TabletView_Exit = m_TabletView.FindAction("Exit", throwIfNotFound: true);
        m_TabletView_Click = m_TabletView.FindAction("Click", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // TabletView
    private readonly InputActionMap m_TabletView;
    private ITabletViewActions m_TabletViewActionsCallbackInterface;
    private readonly InputAction m_TabletView_Exit;
    private readonly InputAction m_TabletView_Click;
    public struct TabletViewActions
    {
        private @TabletViewControls m_Wrapper;
        public TabletViewActions(@TabletViewControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Exit => m_Wrapper.m_TabletView_Exit;
        public InputAction @Click => m_Wrapper.m_TabletView_Click;
        public InputActionMap Get() { return m_Wrapper.m_TabletView; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TabletViewActions set) { return set.Get(); }
        public void SetCallbacks(ITabletViewActions instance)
        {
            if (m_Wrapper.m_TabletViewActionsCallbackInterface != null)
            {
                @Exit.started -= m_Wrapper.m_TabletViewActionsCallbackInterface.OnExit;
                @Exit.performed -= m_Wrapper.m_TabletViewActionsCallbackInterface.OnExit;
                @Exit.canceled -= m_Wrapper.m_TabletViewActionsCallbackInterface.OnExit;
                @Click.started -= m_Wrapper.m_TabletViewActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_TabletViewActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_TabletViewActionsCallbackInterface.OnClick;
            }
            m_Wrapper.m_TabletViewActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Exit.started += instance.OnExit;
                @Exit.performed += instance.OnExit;
                @Exit.canceled += instance.OnExit;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
            }
        }
    }
    public TabletViewActions @TabletView => new TabletViewActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface ITabletViewActions
    {
        void OnExit(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
    }
}
