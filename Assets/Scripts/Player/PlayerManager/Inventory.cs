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
    int PickupItem(int itemID);

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
    
    #region Checkers
    private bool IsEmpty() {
        return p_Inventory.Count == 0;
    }

    private bool HasPassiveRoom() {
        return p_NumPassives < p_MaxPassives;
    }

    private bool HasActive() {
        if (p_Inventory.Count > 0) {
            if (ItemManager.IsActiveItem(p_Inventory[0])) {
                return true;
            }
            else if (p_Inventory.Count > 1) {
                if (ItemManager.IsActiveItem(p_Inventory[1])) {
                    return true;
                }
                else if (p_Inventory.Count > 2 &&
                    ItemManager.IsActiveItem(p_Inventory[2])) {
                    return true;
                }
            }
        }
        return false;
    }

    private bool HasTwoWeapons() {
        return p_Inventory.Count >= 2 &&
            ItemManager.IsWeaponItem(p_Inventory[0]) &&
            ItemManager.IsWeaponItem(p_Inventory[1]);
    }
    #endregion

    #region Picking up and Dropping
    private int DropItemAtIndex(int index) {
        if (index < 0) {
            throw new System.IndexOutOfRangeException($"Tried to drop item " +
                $"in inventory position {index}. This is illegal.");
        }
        if (p_Inventory.Count <= index) {
            throw new System.IndexOutOfRangeException($"Tried to drop item " +
                $"in inventory position {index} but inventory only has " +
                $"{p_Inventory.Count} items.");
        }

        int itemID = p_Inventory[index];
        p_Inventory.RemoveAt(index);

        if (ItemManager.IsPassiveItem(itemID)) {
            p_NumPassives--;
        }

        return itemID;
    }

    private void InsertItemIntoInventory(int itemID) {
        if (itemID == Consts.NULL_ITEM_ID) {
            throw new System.ArgumentNullException("A null value was " +
                "provided for the item ID");
        }

        if (ItemManager.IsWeaponItem(itemID)) {
            InsertWeaponItemIntoInv(itemID);
        }
        if (ItemManager.IsActiveItem(itemID)) {
            InsertActiveItemIntoInv(itemID);
        }
        if (ItemManager.IsPassiveItem(itemID)) {
            InsertPassiveItemIntoInv(itemID);
            p_NumPassives++;
        }
    }

    private void InsertWeaponItemIntoInv(int itemID) {
        if (!ItemManager.IsWeaponItem(itemID)) {
            throw new System.ArgumentException($"This function only inserts " +
                $"weapon items into the inventory but was given {itemID} " +
                $"which is NOT a weapon item.");
        }
        if (HasTwoWeapons()) {
            throw new System.AccessViolationException("Trying to insert " +
                "a weapon item into the inventory but player already " +
                "has two weapons in inventory.");
        }

        if (p_Inventory.Count > 0) {
            if (ItemManager.IsWeaponItem(p_Inventory[0])) {
                p_Inventory.Insert(1, itemID);
            }
            else {
                p_Inventory.Insert(0, itemID);
            }
        }
        else {
            p_Inventory.Insert(0, itemID);
        }
    }

    private void InsertActiveItemIntoInv(int itemID) {
        if (!ItemManager.IsActiveItem(itemID)) {
            throw new System.ArgumentException($"This function only inserts " +
                $"active items into the inventory but was given {itemID} " +
                $"which is NOT an active item.");
        }
        if (HasActive()) {
            throw new System.AccessViolationException("Trying to insert " +
                "an active item into the inventory but player already " +
                "has an active in inventory.");
        }

        if (p_Inventory.Count > 0) {
            //  Inventory does not have any weapon items
            if (ItemManager.IsPassiveItem(p_Inventory[0])) {
                p_Inventory.Insert(0, itemID);
            }
            else if (p_Inventory.Count > 1) {
                // Inventory has a single weapon item
                if (ItemManager.IsPassiveItem(p_Inventory[1])) {
                    p_Inventory.Insert(1, itemID);
                }
                else { // Inventory has two weapon items
                    p_Inventory.Insert(2, itemID);
                }
            }
            else { // Inventory has a single item. It is a weapon
                p_Inventory.Insert(1, itemID);
            }
        }
        else { // Inventory is empty
            p_Inventory.Insert(0, itemID);
        }
    }

    private void InsertPassiveItemIntoInv(int itemID) {
        if (!ItemManager.IsPassiveItem(itemID)) {
            throw new System.ArgumentException($"This function only inserts " +
                $"passive items into the inventory but was given {itemID} " +
                $"which is NOT a passive item.");
        }
        if (!HasPassiveRoom()) {
            throw new System.AccessViolationException("Trying to insert " +
                "a passive item into the inventory but player already " +
                "has maximum passives in inventory.");
        }

        // TODO: sort by rarity then alphabetically
        p_Inventory.Add(itemID);
    }

    private int ReplaceCurrentActive(int itemID) {
        if (!HasActive()) {
            throw new System.InvalidOperationException("Attempting to " +
                "replace the current active item but this inventory does " +
                "not have any active items.");
        }

        int activeInd = Consts.NULL_ITEM_ID;
        if (p_Inventory.Count > 0) {
            if (ItemManager.IsActiveItem(p_Inventory[0])) {
                activeInd = 0;
            }
            else if (p_Inventory.Count > 1) {
                if (ItemManager.IsActiveItem(p_Inventory[1])) {
                    activeInd = 1;
                }
                else {
                    activeInd = 2;
                }
            }
        }
        
        int currentActiveID = DropItemAtIndex(activeInd);
        InsertItemIntoInventory(itemID);
        return currentActiveID;
    }

    private int ReplaceCurrentWeapon(int itemID) {
        int currentWeaponID = DropCurrentWeapon();
        InsertItemIntoInventory(itemID);
        if (HasTwoWeapons()) {
            SwitchWeapons();
        }
        return currentWeaponID;
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
        if (p_Inventory.Count > 0 && 
            ItemManager.IsWeaponItem(p_Inventory[0])) {
            return DropItemAtIndex(0);
        }
        return Consts.NULL_ITEM_ID;
    }

    public int DropSelectedItem() {
        return DropItemAtIndex(p_CurrentlySelectedItem);
    }

    public int PickupItem(int itemID) {
        if (ItemManager.IsActiveItem(itemID) && HasActive()) {
            return ReplaceCurrentActive(itemID);
        }
        else if(ItemManager.IsWeaponItem(itemID) && HasTwoWeapons()) {
            return ReplaceCurrentWeapon(itemID);
        }
        else if (ItemManager.IsPassiveItem(itemID) && !HasPassiveRoom()) {
            return Consts.NULL_ITEM_ID;
        }
        InsertItemIntoInventory(itemID);
        return itemID;
    }
    #endregion

    #region Getters
    public int GetCurrentWeapon() {
        if (p_Inventory.Count > 0 &&
            ItemManager.IsWeaponItem(p_Inventory[0])) {
            return p_Inventory[0];
        }
        return Consts.NULL_ITEM_ID;
    }

    public int GetSecondaryWeapon() {
        if (!HasTwoWeapons()) {
            return Consts.NULL_ITEM_ID;
        }

        return p_Inventory[1];
    }

    public int GetCurrentActive() {
        if (!HasActive()) {
            return Consts.NULL_ITEM_ID;
        }

        if (ItemManager.IsActiveItem(p_Inventory[0])) {
            return p_Inventory[0];
        }
        else if (ItemManager.IsActiveItem(p_Inventory[1])) {
            return p_Inventory[1];
        }
        return p_Inventory[2];
    }

    public int GetSelectedItem() {
        if (!p_IsOpen) {
            return Consts.NULL_ITEM_ID;
        }
        return p_Inventory[p_CurrentlySelectedItem];
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
        if (ItemManager.IsWeaponItem(p_Inventory[0]) &&
            ItemManager.IsWeaponItem(p_Inventory[1])) {
            int temp = p_Inventory[0];
            p_Inventory[0] = p_Inventory[1];
            p_Inventory[1] = temp;
        }
    }

    public void RemoveActive() {
        if (!HasActive()) {
            return;
        }
        if (ItemManager.IsActiveItem(p_Inventory[0])) {
            p_Inventory.RemoveAt(0);
        }
        else if (ItemManager.IsActiveItem(p_Inventory[1])) {
            p_Inventory.RemoveAt(1);
        }
        else {
            p_Inventory.RemoveAt(2);
        }
    }
    #endregion
}
