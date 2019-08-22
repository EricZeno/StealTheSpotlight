/* 
 * The inventory struct
 * 
 * Edited by: Tom Bar Ezer Ayalon
 * Last edited by: Tom Bar Ezer Ayalon
 * Last edited on: July 05, 2019
 */

using System.Collections.Generic;
using UnityEngine;


public interface InventoryInterace {
    void OpenInventory();
    void CloseInventory();
    void CycleLeft();
    void CycleRight();

    int DropSelectedItem();
    int DropCurrentWeapon();
    void PickupItem(int itemID);
    int ReplaceCurrentWeapon(int itemID);
    int ReplaceCurrentActive(int itemID);

    int GetSelectedItem();
    int[] GetXNeighborsOfSelectedItem(int x);
    int GetCurrentWeapon();
    int GetSecondaryWeapon();
    int GetCurrentActive();

    void SwitchWeapons();
    void RemoveActive();
}

[System.Serializable]
public struct Inventory : InventoryInterace {
    #region Delegates
    private delegate bool CheckerDelegate(int index);
    #endregion

    #region Private Variables
    // All of the items in the inventory. The items are stored using their IDs.
    // Under the hood, the zeroth position will usually be for the current
    // weapon, the first position will be for the secondary weapon, the second
    // position will be for the active item and the rest of the positions are
    // for all of the passive items.
    private List<int> p_Inventory;

    // The current number of passive items being carried
    private int p_NumPassives;

    // The maximum number of passive items allowed to be carried
    private int p_MaxPassives;

    // If the inventory is open, this is set to true. Certain things will not
    // work unless the inventory is open (cycling).
    private bool p_IsOpen;

    // When the inventory is open, this is the position in the inventory array
    // of the currently selected item.
    private int p_CurrentlySelectedItem;
    #endregion

    #region Constructor
    public Inventory(int maxPassives) {
        p_Inventory = new List<int>();
        p_NumPassives = 0;
        p_MaxPassives = maxPassives;
        p_IsOpen = false;
        p_CurrentlySelectedItem = 0;
    }
    #endregion

    #region Getters
    private int GetItem(int index, CheckerDelegate isTypeOfChecker = null) {
        if (p_Inventory.Count <= index || 
            (isTypeOfChecker != null && !isTypeOfChecker(index))) {
            return -1;
        }
        return p_Inventory[index];
    }
    #endregion

    #region Checkers
    private bool IsEmpty() {
        return p_Inventory.Count == 0;
    }

    private bool IsWeapon(int index) {
        throw new System.NotImplementedException();
    }

    private bool IsActive(int index) {
        throw new System.NotImplementedException();
    }

    private bool IsPassive(int index) {
        throw new System.NotImplementedException();
    }

    private bool HasActive() {
        if (p_Inventory.Count > 2 &&
            (IsActive(0) || IsActive(1) || IsActive(2))) {
            return true;
        }
        if (p_Inventory.Count > 1 &&
            (IsActive(0) || IsActive(2))) {
            return true;
        }
        if (p_Inventory.Count > 0 && IsActive(0)) {
            return true;
        }
        return false;
    }
    #endregion

    #region Picking up and Dropping
    private int DropItem(int index) {
        if (p_Inventory.Count <= index) {
            return -1;
        }
        if (IsPassive(index)) {
            p_NumPassives--;
        }
        int itemID = p_Inventory[index];
        p_Inventory.RemoveAt(index);
        return itemID;
    }

    private void PickupItem(int itemID, int index) {
        if (index < 0) {
            index = 0;
        }
        else if (index > p_Inventory.Count) {
            index = p_Inventory.Count;
        }
        p_Inventory.Insert(index, itemID);
        if (IsPassive(index)) {
            p_NumPassives++;
        }
    }
    #endregion

    /*----------------------------*
     * Interface required methods *
     *----------------------------*/

    #region Inventory Control
    public void OpenInventory() {
        p_IsOpen = true;
    }

    public void CloseInventory() {
        p_IsOpen = false;
    }

    public void CycleLeft() {
        if (IsEmpty() || !p_IsOpen) {
            return;
        }
        p_CurrentlySelectedItem--;
        if (p_CurrentlySelectedItem <= -1) {
            p_CurrentlySelectedItem = p_Inventory.Count - 1;
        }
    }

    public void CycleRight() {
        if (IsEmpty() || !p_IsOpen) {
            return;
        }
        p_CurrentlySelectedItem++;
        if (p_CurrentlySelectedItem >= p_Inventory.Count) {
            p_CurrentlySelectedItem = 0;
        }
    }
    #endregion

    #region Pickup and Drop
    public int DropCurrentWeapon() {
        if (IsWeapon(0)) {
            return DropItem(0);
        }
        return -1;
    }

    public int DropSelectedItem() {
        return DropItem(p_CurrentlySelectedItem);
    }

    public void PickupItem(int itemID) {
        throw new System.NotImplementedException();
        /*
        if is a weapon,
            if zeroth is not a weapon,
                put in zero
            else if one is not a weapon,
                put in one
            return
        if is an active,
            if active is in 0, 1, or 2,
                return
            put in 2
            return
        if passive count < max passive count
            find position for passive
            put in position
         */
    }

    public int ReplaceCurrentActive(int itemID) {
        int activeInd = -1;
        if (p_Inventory.Count > 2 && IsActive(2)) {
            activeInd = 2;
        }
        else if (p_Inventory.Count > 1 && IsActive(1)) {
            activeInd = 1;
        }
        else if (p_Inventory.Count > 0 && IsActive(0)) {
            activeInd = 0;
        }
        int currentActiveID = DropItem(activeInd);
        PickupItem(itemID, activeInd);
        return currentActiveID;
    }

    public int ReplaceCurrentWeapon(int itemID) {
        int currentWeaponID = DropCurrentWeapon();
        PickupItem(itemID, 0);
        return currentWeaponID;
    }
    #endregion

    #region Getters
    public int GetCurrentWeapon() {
        return GetItem(0, IsWeapon);
    }

    public int GetSecondaryWeapon() {
        return GetItem(1, IsWeapon);
    }

    public int GetCurrentActive() {
        return GetItem(2, IsActive);
    }

    public int GetSelectedItem() {
        if (!p_IsOpen) {
            return -1;
        }
        return GetItem(p_CurrentlySelectedItem);
    }

    public int[] GetXNeighborsOfSelectedItem(int x) {
        int[] neighbors = new int[2 * x];
        for (int i = 0; i < 2 * x; i++) {
            neighbors[i] = -1;
        }
        if (IsEmpty() || !p_IsOpen) {
            return neighbors;
        }

        int inventoryInd = p_CurrentlySelectedItem - 1;
        int neighborsInd = x - 1;
        while (neighborsInd >= 0) {
            if (inventoryInd == -1) {
                inventoryInd = p_Inventory.Count - 1;
            }
            if (inventoryInd == p_CurrentlySelectedItem) {
                break;
            }
            neighbors[neighborsInd--] = p_Inventory[inventoryInd--];
        }
        inventoryInd = p_CurrentlySelectedItem + 1;
        neighborsInd = x + 1;
        while (neighborsInd < 2 * x) {
            if (inventoryInd == p_Inventory.Count) {
                inventoryInd = 0;
            }
            if (inventoryInd == p_CurrentlySelectedItem) {
                break;
            }
            neighbors[neighborsInd++] = p_Inventory[inventoryInd++];
        }
        return neighbors;
    }
    #endregion

    #region Misc
    public void SwitchWeapons() {
        if (p_IsOpen) {
            return;
        }
        if (p_Inventory.Count < 2) {
            return;
        }
        if (IsWeapon(0) && IsWeapon(1)) {
            int temp = p_Inventory[0];
            p_Inventory[0] = p_Inventory[1];
            p_Inventory[1] = temp;
        }
    }

    public void RemoveActive() {
        if (!HasActive()) {
            return;
        }
        if (IsActive(0)) {
            p_Inventory.RemoveAt(0);
        }
        else if (IsActive(1)) {
            p_Inventory.RemoveAt(1);
        }
        else {
            p_Inventory.RemoveAt(2);
        }
    }
    #endregion
}
