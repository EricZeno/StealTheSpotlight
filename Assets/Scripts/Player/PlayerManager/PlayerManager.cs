using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;

public class PlayerManager : MonoBehaviour
{
    #region Structs and Variables

    // Player Data Struct goes here

    // The player's inventory
    private Inventory p_Inventory;

    // A reference to the Player Input component to swap action maps
    private PlayerInput cc_Input;

    #endregion

    #region Startup

    // Start is called before the first frame update
    void Start()
    {
        // Number of max passives subject to change based on item balancing
        p_Inventory = new Inventory(10);
        cc_Input = GetComponent<PlayerInput>();
    }

    #endregion

    #region Input Receivers
    private void OnCycleLeft()
    {
        p_Inventory.CycleLeft();
    }

    private void OnCycleRight()
    {
        p_Inventory.CycleRight();
    }

    private void OnOpenInventory()
    {
        cc_Input.SwitchCurrentActionMap("Inventory");
        p_Inventory.OpenInventory();
    }

    private void OnCloseInventory()
    {
        p_Inventory.CloseInventory();
        cc_Input.SwitchCurrentActionMap("Gameplay");
    }

    private void OnSwitchWeapons()
    {
        p_Inventory.SwitchWeapons();
    }

    private void OnDropItem()
    {
        p_Inventory.DropSelectedItem();
    }

    private void OnDropWeapon()
    {
        p_Inventory.DropCurrentWeapon();
    }

    #endregion

}
