// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerControls : IInputActionCollection
{
    private InputActionAsset asset;
    public PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""e26ade35-fa3b-462c-a856-19cedea34a61"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""c2e15d6c-9e43-4b25-86a5-fdfb0f4d208f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""a04330b6-5e3c-42ca-a294-3bfb107f1028"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""087b4d36-ee07-4b0f-a2fc-c5342734492b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""32979a68-d18d-4493-b5c3-49a9237084d3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UseAbility"",
                    ""type"": ""Button"",
                    ""id"": ""dcdee802-1531-4cc5-937a-f05798b39aee"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""OpenInventory"",
                    ""type"": ""Button"",
                    ""id"": ""ae72bcb8-32b1-4907-bef7-7cb1ec102755"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UseActive"",
                    ""type"": ""Button"",
                    ""id"": ""e80adab4-84d8-4f34-9f85-3e3b720262a8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapWeapons"",
                    ""type"": ""Button"",
                    ""id"": ""525ff357-afed-45b0-9103-52fcbd4dbf0a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""ef458cbf-4d09-40a9-806e-891459fd321f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d10c6070-e2c3-4227-91cc-b9b96dd5ca7f"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c88124d8-89f8-4ee2-a78b-af914ad641d3"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""583b490c-f320-403a-888f-195a596435e1"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21f84aae-0948-4050-a049-bc48b523211d"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8ffa0ec3-2888-40d1-8ff5-e8eefe90bb3c"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95a0fa01-ed3d-471a-afd7-340536362d98"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c258762-375f-43a7-a472-d8b01809a568"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseActive"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f87af54d-8839-4f25-a08d-fe9cae567991"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapWeapons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5aebaa09-50b3-4e4a-998c-a41b066fa8ad"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Inventory"",
            ""id"": ""5f15e7b5-1a15-4a8c-840b-650587bf79c6"",
            ""actions"": [
                {
                    ""name"": ""CycleLeft"",
                    ""type"": ""Button"",
                    ""id"": ""517bc54b-dbf7-40da-8d27-6653def4dadf"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Button"",
                    ""id"": ""8390ea40-1312-4751-b35e-e19889bb41d9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Close"",
                    ""type"": ""Button"",
                    ""id"": ""d6f27193-ab4b-4b1d-9272-50d7db544b14"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CycleRight"",
                    ""type"": ""Button"",
                    ""id"": ""419a2f66-ac44-4918-b42a-b585f877587c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""994496ae-ca8b-44f2-bd94-08537411b620"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CycleLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b48dac3a-584c-4d5e-a979-c3a76354670e"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""658fbd3e-6573-42f5-a562-cbf5c4d5e5a8"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Close"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d91d237-2fd2-46aa-b4e9-fc10b50afe3d"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CycleRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_Pause = m_Gameplay.GetAction("Pause");
        m_Gameplay_Move = m_Gameplay.GetAction("Move");
        m_Gameplay_Aim = m_Gameplay.GetAction("Aim");
        m_Gameplay_Attack = m_Gameplay.GetAction("Attack");
        m_Gameplay_UseAbility = m_Gameplay.GetAction("UseAbility");
        m_Gameplay_OpenInventory = m_Gameplay.GetAction("OpenInventory");
        m_Gameplay_UseActive = m_Gameplay.GetAction("UseActive");
        m_Gameplay_SwapWeapons = m_Gameplay.GetAction("SwapWeapons");
        m_Gameplay_Interact = m_Gameplay.GetAction("Interact");
        // Inventory
        m_Inventory = asset.GetActionMap("Inventory");
        m_Inventory_CycleLeft = m_Inventory.GetAction("CycleLeft");
        m_Inventory_Drop = m_Inventory.GetAction("Drop");
        m_Inventory_Close = m_Inventory.GetAction("Close");
        m_Inventory_CycleRight = m_Inventory.GetAction("CycleRight");
    }

    ~PlayerControls()
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Pause;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Aim;
    private readonly InputAction m_Gameplay_Attack;
    private readonly InputAction m_Gameplay_UseAbility;
    private readonly InputAction m_Gameplay_OpenInventory;
    private readonly InputAction m_Gameplay_UseActive;
    private readonly InputAction m_Gameplay_SwapWeapons;
    private readonly InputAction m_Gameplay_Interact;
    public struct GameplayActions
    {
        private PlayerControls m_Wrapper;
        public GameplayActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_Gameplay_Pause;
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Aim => m_Wrapper.m_Gameplay_Aim;
        public InputAction @Attack => m_Wrapper.m_Gameplay_Attack;
        public InputAction @UseAbility => m_Wrapper.m_Gameplay_UseAbility;
        public InputAction @OpenInventory => m_Wrapper.m_Gameplay_OpenInventory;
        public InputAction @UseActive => m_Wrapper.m_Gameplay_UseActive;
        public InputAction @SwapWeapons => m_Wrapper.m_Gameplay_SwapWeapons;
        public InputAction @Interact => m_Wrapper.m_Gameplay_Interact;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Pause.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Pause.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Pause.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Aim.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Aim.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Aim.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Attack.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                Attack.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                Attack.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                UseAbility.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseAbility;
                UseAbility.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseAbility;
                UseAbility.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseAbility;
                OpenInventory.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnOpenInventory;
                OpenInventory.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnOpenInventory;
                OpenInventory.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnOpenInventory;
                UseActive.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseActive;
                UseActive.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseActive;
                UseActive.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseActive;
                SwapWeapons.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSwapWeapons;
                SwapWeapons.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSwapWeapons;
                SwapWeapons.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSwapWeapons;
                Interact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                Interact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                Interact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Pause.started += instance.OnPause;
                Pause.performed += instance.OnPause;
                Pause.canceled += instance.OnPause;
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Aim.started += instance.OnAim;
                Aim.performed += instance.OnAim;
                Aim.canceled += instance.OnAim;
                Attack.started += instance.OnAttack;
                Attack.performed += instance.OnAttack;
                Attack.canceled += instance.OnAttack;
                UseAbility.started += instance.OnUseAbility;
                UseAbility.performed += instance.OnUseAbility;
                UseAbility.canceled += instance.OnUseAbility;
                OpenInventory.started += instance.OnOpenInventory;
                OpenInventory.performed += instance.OnOpenInventory;
                OpenInventory.canceled += instance.OnOpenInventory;
                UseActive.started += instance.OnUseActive;
                UseActive.performed += instance.OnUseActive;
                UseActive.canceled += instance.OnUseActive;
                SwapWeapons.started += instance.OnSwapWeapons;
                SwapWeapons.performed += instance.OnSwapWeapons;
                SwapWeapons.canceled += instance.OnSwapWeapons;
                Interact.started += instance.OnInteract;
                Interact.performed += instance.OnInteract;
                Interact.canceled += instance.OnInteract;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Inventory
    private readonly InputActionMap m_Inventory;
    private IInventoryActions m_InventoryActionsCallbackInterface;
    private readonly InputAction m_Inventory_CycleLeft;
    private readonly InputAction m_Inventory_Drop;
    private readonly InputAction m_Inventory_Close;
    private readonly InputAction m_Inventory_CycleRight;
    public struct InventoryActions
    {
        private PlayerControls m_Wrapper;
        public InventoryActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @CycleLeft => m_Wrapper.m_Inventory_CycleLeft;
        public InputAction @Drop => m_Wrapper.m_Inventory_Drop;
        public InputAction @Close => m_Wrapper.m_Inventory_Close;
        public InputAction @CycleRight => m_Wrapper.m_Inventory_CycleRight;
        public InputActionMap Get() { return m_Wrapper.m_Inventory; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InventoryActions set) { return set.Get(); }
        public void SetCallbacks(IInventoryActions instance)
        {
            if (m_Wrapper.m_InventoryActionsCallbackInterface != null)
            {
                CycleLeft.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnCycleLeft;
                CycleLeft.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnCycleLeft;
                CycleLeft.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnCycleLeft;
                Drop.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnDrop;
                Drop.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnDrop;
                Drop.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnDrop;
                Close.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnClose;
                Close.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnClose;
                Close.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnClose;
                CycleRight.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnCycleRight;
                CycleRight.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnCycleRight;
                CycleRight.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnCycleRight;
            }
            m_Wrapper.m_InventoryActionsCallbackInterface = instance;
            if (instance != null)
            {
                CycleLeft.started += instance.OnCycleLeft;
                CycleLeft.performed += instance.OnCycleLeft;
                CycleLeft.canceled += instance.OnCycleLeft;
                Drop.started += instance.OnDrop;
                Drop.performed += instance.OnDrop;
                Drop.canceled += instance.OnDrop;
                Close.started += instance.OnClose;
                Close.performed += instance.OnClose;
                Close.canceled += instance.OnClose;
                CycleRight.started += instance.OnCycleRight;
                CycleRight.performed += instance.OnCycleRight;
                CycleRight.canceled += instance.OnCycleRight;
            }
        }
    }
    public InventoryActions @Inventory => new InventoryActions(this);
    public interface IGameplayActions
    {
        void OnPause(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnUseAbility(InputAction.CallbackContext context);
        void OnOpenInventory(InputAction.CallbackContext context);
        void OnUseActive(InputAction.CallbackContext context);
        void OnSwapWeapons(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
    public interface IInventoryActions
    {
        void OnCycleLeft(InputAction.CallbackContext context);
        void OnDrop(InputAction.CallbackContext context);
        void OnClose(InputAction.CallbackContext context);
        void OnCycleRight(InputAction.CallbackContext context);
    }
}
