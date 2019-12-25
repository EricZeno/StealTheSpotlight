using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;

[DisallowMultipleComponent]
public class PlayerActiveItem : MonoBehaviour {
    #region Interface
    public void ChangeActiveItem(int itemID) {
        if (!ItemManager.IsActiveItem(itemID)) {
            throw new System.ArgumentException($"Trying to set item {itemID} as current " +
                $"active item but it is not an active.");
        }
        m_ItemID = itemID;
        m_Item = ItemManager.GetActiveItem(itemID);
    }

    public void RemoveActiveItem() {
        m_ItemID = Consts.NULL_ITEM_ID;
    }
    #endregion

    #region Variables
    #region Private Variables
    private int m_ItemID;
    private BaseActiveItem m_Item;

    private float m_CooldownTimer;
    #endregion

    #region Cached Components
    private PlayerManager m_Manager;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        SetupBasicVariables();
        SetupCachedComponents();
    }

    private void SetupBasicVariables() {
        m_ItemID = Consts.NULL_ITEM_ID;
        m_Item = null;

        m_CooldownTimer = 0;
    }

    private void SetupCachedComponents() {
        m_Manager = GetComponent<PlayerManager>();
    }
    #endregion

    #region Main Updates
    private void Update() {
        ReduceCooldown();
    }
    #endregion

    #region Checkers
    private bool CanUseItem() {
        if (m_CooldownTimer > 0) {
            return false;
        }
        if (m_ItemID == Consts.NULL_ITEM_ID) {
            return false;
        }
        return true;
    }
    #endregion

    #region Input Receivers
    private void OnUseActive(InputValue value) {
        if (!CanUseItem()) {
            return;
        }

        if (value.isPressed) {
            UseItem();
        }
    }
    #endregion

    #region Cooldown
    private void ResetCooldown() {
        if (m_ItemID == Consts.NULL_ITEM_ID) {
            throw new System.AccessViolationException("Trying to reset cooldown on active" +
                "item but there is no active item equipped");
        }
        m_CooldownTimer = m_Item.GetCooldown();
    }

    private void ReduceCooldown() {
        if (m_CooldownTimer > 0) {
            m_CooldownTimer -= Time.deltaTime;
        }
    }
    #endregion

    #region Using Item
    private void UseItem() {
        m_Item.UseEffect(m_Manager);
        ResetCooldown();
    }
    #endregion
}
