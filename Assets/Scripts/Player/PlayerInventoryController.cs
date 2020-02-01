using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerCanvas))]
[RequireComponent(typeof(PlayerWeapon))]
public class PlayerInventoryController : MonoBehaviour {
    #region Interface
    public bool PickUpItem(ItemGameObject item) {
        return PickUpItem(item.GetID());
    }
    #endregion

    #region Variables
    #region Private Variables
    private Inventory m_Inventory;
    #endregion

    #region Cached Components
    private PlayerManager m_Manager;
    private PlayerCanvas m_UI;
    private PlayerWeapon m_Weapon;
    private AudioManager m_AudioManager;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        SetupBasicVariables();
        SetupCachedComponents();
    }

    private void SetupBasicVariables() {
        m_Inventory = new Inventory();
    }

    private void SetupCachedComponents() {
        m_Manager = GetComponent<PlayerManager>();
        m_UI = GetComponent<PlayerCanvas>();
        m_Weapon = GetComponent<PlayerWeapon>();
        m_Inventory.SetCanvasUI(m_UI);
        //m_AudioManager = GetComponent<AudioManager>();
    }
    #endregion

    #region Accessors
    public int MaxInventoryCapacity {
        get {
            return m_Inventory.MaxInventoryCapacity;
        }
    }

    public int[] AllInventoryItems {
        get {
            return m_Inventory.GetInventoryList();
        }
    }

    public int SelectedItemIndex {
        get {
            return m_Inventory.GetSelectedItemIndex();
        }
    }
    #endregion

    #region Input Receivers
    private void OnOpenInventory() {
        OpenInventory();
    }

    private void OnCloseInventory() {
        CloseInventory();
    }

    private void OnSwitchWeapons() {
        SwitchWeapons();
    }

    private void OnDropItem() {
        DropItem();
    }

    private void OnDropWeapon() {
        DropWeapon();
    }

    private void OnSelect(InputValue value) {
        SelectItem(value.Get<Vector2>());
    }
    #endregion

    #region Opening and Closing Inventory
    private void OpenInventory() {
        m_Manager.OpenInventory();
        m_Inventory.OpenInventory();
        m_UI.OpenInventoryUI();
        //m_AudioManager.Play("wilhelm");
    }

    private void CloseInventory() {
        m_Manager.CloseInventory();
        m_Inventory.CloseInventory();
        m_UI.CloseInventoryUI();
    }
    #endregion

    #region Switching Weapons
    private void SwitchWeapons() {
        if (m_Weapon.IsAttacking) {
            return;
        }
        int prevWeapon = m_Inventory.CurrentWeapon;
        m_Inventory.SwitchWeapons();
        int curWeapon = m_Inventory.CurrentWeapon;
        if (prevWeapon != curWeapon) {
            if (curWeapon != Consts.NULL_ITEM_ID) {
                m_Weapon.Equip(curWeapon);
            }
            else {
                m_Weapon.Unequip();
            }
        }
    }
    #endregion

    #region Dropping Items
    private void DropItem() {
        try {
            bool hasWeaponEquipped = (m_Inventory.CurrentWeapon != Consts.NULL_ITEM_ID);
            int itemID = m_Inventory.DropSelectedItem();
            if (itemID != Consts.NULL_ITEM_ID) {
                DropItem(itemID);
                m_UI.DropSelectedItem();

                if (ItemManager.IsWeaponItem(itemID)) {
                    int curWeapon = m_Inventory.CurrentWeapon;
                    if (hasWeaponEquipped && curWeapon == Consts.NULL_ITEM_ID) {
                        m_Weapon.Unequip();
                    }
                }
            }
        }
        catch (System.InvalidOperationException) { }
    }

    private void DropWeapon() {
        if (m_Weapon.IsAttacking) {
            return;
        }
        try {
            int itemID = m_Inventory.DropCurrentWeapon();
            if (itemID == Consts.NULL_ITEM_ID) {
                return;
            }
            DropItem(itemID);
            m_Weapon.Unequip();
        }
        catch (System.IndexOutOfRangeException) { }
    }

    private void DropItem(int itemID) {
        if (ItemManager.IsPassiveItem(itemID)) {
            ItemManager.GetPassiveItem(itemID).RemoveEffect(m_Manager);
        }
        else if (ItemManager.IsActiveItem(itemID)) {
            ItemManager.GetActiveItem(itemID).CancelEffect();
        }
        DropManager.DropItem(itemID, transform.position);
    }
    #endregion

    #region Selecting Items
    private void SelectItem(Vector2 aimVector) {
        // Don't do anything on release; aim vector will be (0, 0)
        if (aimVector.sqrMagnitude < Mathf.Epsilon) {
            return;
        }

        // Calculate angle (flip if aim vector points to the right)
        int selectIndex = GetIndex(AngleFromAim(aimVector));

        // Select that item
        m_Inventory.SelectItemAtIndex(selectIndex);

        // Highlight the corresponding wedge
        m_UI.SelectItem();
    }

    private float AngleFromAim(Vector2 aimVector) {
        float angle = Vector2.Angle(Vector2.up, aimVector);
        if (aimVector.x > 0) {
            angle = 360 - angle;
        }
        return angle;
    }

    private int GetIndex(float angle) {
        if (angle < 0 || angle > 360) {
            throw new System.ArgumentOutOfRangeException($"The angle must be between 0 and 360 " +
                $"when selecting an item in inventory. Angle was {angle}");
        }

        // Set up variables
        float wedgeAngle = 360 / m_Inventory.MaxInventoryCapacity;
        float halfWedgeAngle = wedgeAngle / 2;
        
        // Special case: selectIndex is 0 (top wedge wraps around between 337.5 and 22.5 degrees)
        if (angle > 360 - halfWedgeAngle || angle <= halfWedgeAngle) {
            return 0;
        }

        // Calculate selectIndex
        for (int i = 1; i < m_Inventory.MaxInventoryCapacity; i++) {
            if (angle <= halfWedgeAngle + wedgeAngle * i) {
                return i;
            }
        }

        throw new System.ArgumentOutOfRangeException($"The code is wrong. The angle is {angle}, " +
            $"which is between 0 and 360 but an index was not returned.");
    }
    #endregion

    #region Picking Up Items
    private bool PickUpItem(int itemID) {
        if (!m_Inventory.CanPickUp(itemID)) {
            return false;
        }

        int replacedItem = PickUpAndReplaceItem(itemID);
        if (replacedItem == Consts.NULL_ITEM_ID) {
            return false;
        }

        if (replacedItem != itemID) {
            DropItem(replacedItem);
        }

        if (m_Inventory.IsOpen) {
            m_UI.UpdateUI();
        }
        
        return true;
    }

    private int PickUpAndReplaceItem(int itemID) {
        if (ItemManager.IsPassiveItem(itemID)) {
            return PickUpPassiveItem(itemID); ;
        }
        if (ItemManager.IsActiveItem(itemID)) {
            return PickUpActiveItem(itemID);
        }
        if (ItemManager.IsWeaponItem(itemID)) {
            return PickUpWeaponItem(itemID);
        }

        throw new System.ArgumentException($"The item ID provided ({itemID}) is not a valid " +
            $"item ID for any item type.");
    }

    private int PickUpPassiveItem(int itemID) {
        if (!ItemManager.IsPassiveItem(itemID)) {
            throw new System.ArgumentException("This function requires a " +
                $"passive item. The provided item ID is {itemID}.");
        }
        m_Inventory.PickupItem(itemID);
        m_Manager.ApplyPassiveItemEffect(itemID);
        return itemID;
    }

    private int PickUpActiveItem(int itemID) {
        if (!ItemManager.IsActiveItem(itemID)) {
            throw new System.ArgumentException("This function requires a " +
                $"active item. The provided item ID is {itemID}.");
        }
        return m_Inventory.PickupItem(itemID);
    }

    private int PickUpWeaponItem(int itemID) {
        if (!ItemManager.IsWeaponItem(itemID)) {
            throw new System.ArgumentException("This function requires a " +
                $"weapon item. The provided item ID is {itemID}.");
        }
        int curWeaponItem = m_Inventory.CurrentWeapon;
        bool hasWeapon = curWeaponItem != Consts.NULL_ITEM_ID;
        int replacedWeapon = m_Inventory.PickupItem(itemID);
        if (hasWeapon && curWeaponItem != m_Inventory.CurrentWeapon) {
            m_Weapon.Equip(itemID);
        }
        else if (!hasWeapon) {
            m_Weapon.Equip(itemID);
        }
        return replacedWeapon;
    }
    #endregion
}
