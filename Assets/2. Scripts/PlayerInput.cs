// GENERATED AUTOMATICALLY FROM 'Assets/2. Scripts/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Bet"",
            ""id"": ""e8e28040-f2d7-4d8e-981e-198852a1ab1f"",
            ""actions"": [
                {
                    ""name"": ""AddBet"",
                    ""type"": ""Button"",
                    ""id"": ""9a8ea155-a9b0-46b7-9b27-998bc139e03c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ConfirmBet"",
                    ""type"": ""Button"",
                    ""id"": ""a3d354fd-88f1-4a22-83bd-97f0f6a318f2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelectGreen"",
                    ""type"": ""Button"",
                    ""id"": ""e0d9a3fc-83b7-4bf6-9d7d-ebe457341c7f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelectRed"",
                    ""type"": ""Button"",
                    ""id"": ""06335f83-c381-495d-a177-a7d8438451e7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""713c11b9-737f-4fab-95ad-fbef760ab7eb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AddBet"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5fa9db0b-10f0-4f45-a406-8a1a65abadb3"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ConfirmBet"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4e63819-7b57-4bfc-9cc1-441c85564ec9"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectGreen"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d4fd95e-f76e-48dd-9953-b4ef41a32159"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectRed"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerMovement"",
            ""id"": ""a8b43b91-652e-42f2-9bdc-057a48c018a5"",
            ""actions"": [
                {
                    ""name"": ""Camera"",
                    ""type"": ""PassThrough"",
                    ""id"": ""893f2299-5591-4a4a-81a1-661139a981ac"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""481e7bd3-8f9c-4200-803d-4f60087ee81d"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Bet
        m_Bet = asset.FindActionMap("Bet", throwIfNotFound: true);
        m_Bet_AddBet = m_Bet.FindAction("AddBet", throwIfNotFound: true);
        m_Bet_ConfirmBet = m_Bet.FindAction("ConfirmBet", throwIfNotFound: true);
        m_Bet_SelectGreen = m_Bet.FindAction("SelectGreen", throwIfNotFound: true);
        m_Bet_SelectRed = m_Bet.FindAction("SelectRed", throwIfNotFound: true);
        // PlayerMovement
        m_PlayerMovement = asset.FindActionMap("PlayerMovement", throwIfNotFound: true);
        m_PlayerMovement_Camera = m_PlayerMovement.FindAction("Camera", throwIfNotFound: true);
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

    // Bet
    private readonly InputActionMap m_Bet;
    private IBetActions m_BetActionsCallbackInterface;
    private readonly InputAction m_Bet_AddBet;
    private readonly InputAction m_Bet_ConfirmBet;
    private readonly InputAction m_Bet_SelectGreen;
    private readonly InputAction m_Bet_SelectRed;
    public struct BetActions
    {
        private @PlayerInput m_Wrapper;
        public BetActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @AddBet => m_Wrapper.m_Bet_AddBet;
        public InputAction @ConfirmBet => m_Wrapper.m_Bet_ConfirmBet;
        public InputAction @SelectGreen => m_Wrapper.m_Bet_SelectGreen;
        public InputAction @SelectRed => m_Wrapper.m_Bet_SelectRed;
        public InputActionMap Get() { return m_Wrapper.m_Bet; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BetActions set) { return set.Get(); }
        public void SetCallbacks(IBetActions instance)
        {
            if (m_Wrapper.m_BetActionsCallbackInterface != null)
            {
                @AddBet.started -= m_Wrapper.m_BetActionsCallbackInterface.OnAddBet;
                @AddBet.performed -= m_Wrapper.m_BetActionsCallbackInterface.OnAddBet;
                @AddBet.canceled -= m_Wrapper.m_BetActionsCallbackInterface.OnAddBet;
                @ConfirmBet.started -= m_Wrapper.m_BetActionsCallbackInterface.OnConfirmBet;
                @ConfirmBet.performed -= m_Wrapper.m_BetActionsCallbackInterface.OnConfirmBet;
                @ConfirmBet.canceled -= m_Wrapper.m_BetActionsCallbackInterface.OnConfirmBet;
                @SelectGreen.started -= m_Wrapper.m_BetActionsCallbackInterface.OnSelectGreen;
                @SelectGreen.performed -= m_Wrapper.m_BetActionsCallbackInterface.OnSelectGreen;
                @SelectGreen.canceled -= m_Wrapper.m_BetActionsCallbackInterface.OnSelectGreen;
                @SelectRed.started -= m_Wrapper.m_BetActionsCallbackInterface.OnSelectRed;
                @SelectRed.performed -= m_Wrapper.m_BetActionsCallbackInterface.OnSelectRed;
                @SelectRed.canceled -= m_Wrapper.m_BetActionsCallbackInterface.OnSelectRed;
            }
            m_Wrapper.m_BetActionsCallbackInterface = instance;
            if (instance != null)
            {
                @AddBet.started += instance.OnAddBet;
                @AddBet.performed += instance.OnAddBet;
                @AddBet.canceled += instance.OnAddBet;
                @ConfirmBet.started += instance.OnConfirmBet;
                @ConfirmBet.performed += instance.OnConfirmBet;
                @ConfirmBet.canceled += instance.OnConfirmBet;
                @SelectGreen.started += instance.OnSelectGreen;
                @SelectGreen.performed += instance.OnSelectGreen;
                @SelectGreen.canceled += instance.OnSelectGreen;
                @SelectRed.started += instance.OnSelectRed;
                @SelectRed.performed += instance.OnSelectRed;
                @SelectRed.canceled += instance.OnSelectRed;
            }
        }
    }
    public BetActions @Bet => new BetActions(this);

    // PlayerMovement
    private readonly InputActionMap m_PlayerMovement;
    private IPlayerMovementActions m_PlayerMovementActionsCallbackInterface;
    private readonly InputAction m_PlayerMovement_Camera;
    public struct PlayerMovementActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerMovementActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Camera => m_Wrapper.m_PlayerMovement_Camera;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMovementActions instance)
        {
            if (m_Wrapper.m_PlayerMovementActionsCallbackInterface != null)
            {
                @Camera.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCamera;
            }
            m_Wrapper.m_PlayerMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
            }
        }
    }
    public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);
    public interface IBetActions
    {
        void OnAddBet(InputAction.CallbackContext context);
        void OnConfirmBet(InputAction.CallbackContext context);
        void OnSelectGreen(InputAction.CallbackContext context);
        void OnSelectRed(InputAction.CallbackContext context);
    }
    public interface IPlayerMovementActions
    {
        void OnCamera(InputAction.CallbackContext context);
    }
}
