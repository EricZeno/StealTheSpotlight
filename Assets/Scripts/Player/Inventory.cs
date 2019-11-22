using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Inventory {
    #region Private Variables
    // All of the items in the inventory. The items are stored using their IDs.
    // Under the hood, the zeroth position will usually be for the current
    // weapon, the first position will be for the secondary weapon, the second
    // position will be for the active item and the rest of the positions are
    // for all of the passive items.
    private int[] m_Inventory;

    // The current number of passive items being carried
    private int m_NumPassives;

    // The number of items currently in the inventory.
    private int m_ItemCount;

    // If the inventory is open, this is set to true. Certain things will not
    // work unless the inventory is open (cycling).
    private bool m_IsOpen;

    // When the inventory is open, this is the position in the inventory array
    // of the currently selected item.
    private int m_CurrentlySelectedItem;

    // The index of the current passive item.
    private int m_CurrPassiveItemIndex;
    #endregion

    #region Constants
    // The maximum number of passive items allowed to be carried
    private const int m_MaxPassives = Consts.NUM_MAX_PASSIVES_IN_INV;

    // The maximum number of active items allowed to be carried
    private const int m_MaxActives = Consts.NUM_MAX_ACTIVES_IN_INV;

    // The maximum number of weapons allowed to be carried
    private const int m_MaxWeapons = Consts.NUM_MAX_WEAPONS_IN_INV;

    // The index of the first weapon.
    private const int m_WeaponStartingIndex = 0;

    // The index of the active item (if contained).
    private const int m_ActiveItemIndex = 2;

    // The index of the first passive item.
    private const int m_PassiveItemsStartingIndex = 3;
    #endregion

    #region Constructor
    public Inventory() {
        m_NumPassives = 0;
        m_IsOpen = false;
        m_CurrentlySelectedItem = 0;
        m_Inventory = new int[m_MaxActives + m_MaxPassives + m_MaxWeapons];
        for (int i = 0; i < m_Inventory.Length; i++) {
            m_Inventory[i] = Consts.NULL_ITEM_ID;
        }
        m_ItemCount = 0;
    }
    #endregion

    #region Checkers
    private bool IsEmpty() {
        return m_ItemCount == 0;
    }

    private bool HasPassiveRoom() {
        return m_NumPassives < m_MaxPassives;
    }

    private bool HasActive() {
        if (m_Inventory[m_ActiveItemIndex] != Consts.NULL_ITEM_ID) {
            return true;
        }
        return false;
    }

    public bool HasTwoWeapons() {
        return (m_Inventory[m_WeaponStartingIndex] != Consts.NULL_ITEM_ID
                && m_Inventory[m_WeaponStartingIndex + 1] != Consts.NULL_ITEM_ID);
    }

    public bool CanPickUp(int itemID) {
        if (ItemManager.IsActiveItem(itemID) &&
            itemID == CurrentActive) {
            return false;
        }
        if (HasTwoWeapons() &&
            ItemManager.IsWeaponItem(itemID) &&
            itemID == CurrentWeapon) {
            return false;
        }
        if (ItemManager.IsPassiveItem(itemID) && !HasPassiveRoom()) {
            return false;
        }
        return true;
    }

    public bool IsOpen {
        get {
            return m_IsOpen;
        }
    }
    #endregion

    #region Picking up and Dropping
    private int DropItemAtIndex(int index) {
        if (index < 0) {
            throw new System.IndexOutOfRangeException($"Tried to drop item " +
                $"in inventory position {index}. This is illegal.");
        }
        if (m_Inventory.Length <= index) {
            throw new System.IndexOutOfRangeException($"Tried to drop item " +
                $"in inventory position {index} but inventory only has a capacity of " +
                $"{m_Inventory.Length}.");
        }

        // Get item ID
        int itemID = m_Inventory[index];

        // If no item at this index, error
        if (itemID == Consts.NULL_ITEM_ID) {
            throw new System.InvalidOperationException($"No item at index {index}.");
        }

        // Otherwise, remove the item at that index
        m_Inventory[index] = Consts.NULL_ITEM_ID;
        m_ItemCount -= 1;

        // If the item was a passive, decrement the number of passives
        if (ItemManager.IsPassiveItem(itemID)) {
            m_NumPassives--;
        }

        return itemID;
    }

    private void InsertItemIntoInventory(int itemID) {
        if (itemID == Consts.NULL_ITEM_ID) {
            throw new System.ArgumentNullException("A null value was " +
                "provided for the item ID");
        }

        // Increase the item count
        m_ItemCount += 1;

        if (ItemManager.IsWeaponItem(itemID)) {
            InsertWeaponItemIntoInv(itemID);
        }
        if (ItemManager.IsActiveItem(itemID)) {
            InsertActiveItemIntoInv(itemID);
        }
        if (ItemManager.IsPassiveItem(itemID)) {
            InsertPassiveItemIntoInv(itemID);
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

        // Inserting into first weapon slot
        if (m_Inventory[m_WeaponStartingIndex] == Consts.NULL_ITEM_ID) {
            m_Inventory[m_WeaponStartingIndex] = itemID;
        }

        // Inserting into second weapon slot
        else {
            m_Inventory[m_WeaponStartingIndex + 1] = itemID;
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

        // Insert active item into active item slot
        m_Inventory[m_ActiveItemIndex] = itemID;
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

        // Insert into the first available passive space
        for (int i = 0; i < m_MaxPassives; i++) {
            if (m_Inventory[m_PassiveItemsStartingIndex + i] == Consts.NULL_ITEM_ID) {
                m_Inventory[m_PassiveItemsStartingIndex + i] = itemID;
                break;
            }
        }
        m_NumPassives++;
    }

    private int ReplaceCurrentActive(int itemID) {
        if (!HasActive()) {
            throw new System.InvalidOperationException("Attempting to " +
                "replace the current active item but this inventory does " +
                "not have any active items.");
        }
        int currentActiveID = DropItemAtIndex(m_ActiveItemIndex);
        InsertItemIntoInventory(itemID);
        return currentActiveID;
    }

    private int ReplaceCurrentWeapon(int itemID) {
        int currentWeaponID = DropCurrentWeapon();
        InsertItemIntoInventory(itemID);
        return currentWeaponID;
    }
    #endregion

    /*----------------------------*
     * Interface required methods *
     *----------------------------*/

    #region Inventory Control
    public void OpenInventory() {
        m_CurrentlySelectedItem = 0;
        m_IsOpen = true;
    }

    public void CloseInventory() {
        m_IsOpen = false;
    }

    public void SelectItemAtIndex(int index) {
        if (!m_IsOpen) {
            return;
        }
        m_CurrentlySelectedItem = index;
    }
    #endregion

    #region Pickup and Drop
    public int DropCurrentWeapon() {
        if (m_ItemCount > 0 && m_Inventory[m_WeaponStartingIndex] != Consts.NULL_ITEM_ID) {
            return DropItemAtIndex(0);
        }
        return Consts.NULL_ITEM_ID;
    }

    public int DropSelectedItem() {
        return DropItemAtIndex(m_CurrentlySelectedItem);
    }

    public int PickupItem(int itemID) {
        if (ItemManager.IsActiveItem(itemID) && HasActive()) {
            return ReplaceCurrentActive(itemID);
        }
        else if (ItemManager.IsWeaponItem(itemID) && HasTwoWeapons()) {
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
    public int[] GetInventoryList() {
        return m_Inventory;
    }

    public int GetInventoryItemCount() {
        return m_ItemCount;
    }

    public int GetNumPassives() {
        return m_NumPassives;
    }

    public int GetSecondaryWeapon() {
        if (!HasTwoWeapons()) {
            return Consts.NULL_ITEM_ID;
        }

        return m_Inventory[1];
    }

    public int GetSelectedItem() {
        if (!m_IsOpen) {
            return Consts.NULL_ITEM_ID;
        }
        return m_Inventory[m_CurrentlySelectedItem];
    }

    public int GetSelectedItemIndex() {
        return m_CurrentlySelectedItem;
    }

    public int MaxInventoryCapacity {
        get {
            return m_Inventory.Length;
        }
    }

    public int CurrentWeapon {
        get {
            return m_Inventory[m_WeaponStartingIndex];
        }
    }

    public int CurrentActive {
        get {
            return m_Inventory[m_ActiveItemIndex];
        }
    }
    #endregion

    #region Misc
    public void SwitchWeapons() {
        if (m_IsOpen) {
            return;
        }
        if (m_Inventory[m_WeaponStartingIndex] == Consts.NULL_ITEM_ID &&
            m_Inventory[m_WeaponStartingIndex + 1] == Consts.NULL_ITEM_ID) {
            return;
        }
        
        int temp = m_Inventory[m_WeaponStartingIndex];
        m_Inventory[m_WeaponStartingIndex] = m_Inventory[m_WeaponStartingIndex + 1];
        m_Inventory[m_WeaponStartingIndex + 1] = temp;
    }

    public void RemoveActive() {
        if (!HasActive()) {
            return;
        }
        m_ItemCount--;
        m_Inventory[m_ActiveItemIndex] = Consts.NULL_ITEM_ID;
    }
    #endregion
}
