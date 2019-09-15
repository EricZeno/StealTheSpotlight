using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour {
    #region Constant Variables
    // The names of the action maps referenced by this script 
    private const string INVENTORY = "Inventory";
    private const string GAMEPLAY = "Gameplay";
    #endregion

    #region Private Variables
    // Player Data Struct goes here

    // The player's inventory
    private Inventory p_Inventory;
    #endregion

    #region Cached Components
    // A reference to the Player Input component to swap action maps
    private PlayerInput cc_Input;
    #endregion

    #region Initialization
    private void Awake() {
        // Number of max passives subject to change based on item balancing
        p_Inventory = new Inventory(10);
        cc_Input = GetComponent<PlayerInput>();
    }
    #endregion

    #region Input Receivers
    private void OnCycleLeft() {
        p_Inventory.CycleLeft();
    }

    private void OnCycleRight() {
        p_Inventory.CycleRight();
    }

    private void OnOpenInventory() {
        cc_Input.SwitchCurrentActionMap(INVENTORY);
        p_Inventory.OpenInventory();
    }

    private void OnCloseInventory() {
        p_Inventory.CloseInventory();
        cc_Input.SwitchCurrentActionMap(GAMEPLAY);
    }

    private void OnSwitchWeapons() {
        p_Inventory.SwitchWeapons();
    }

    private void OnDropItem() {
        p_Inventory.DropSelectedItem();
    }

    private void OnDropWeapon() {
        p_Inventory.DropCurrentWeapon();
    }
    #endregion
}
