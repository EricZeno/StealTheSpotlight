using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour {
    #region Delegates
    public delegate void EffectToApply(PlayerManager player);
    #endregion

    #region Constant Variables
    // The names of the action maps referenced by this script 
    private const string INVENTORY = "Inventory";
    private const string GAMEPLAY = "Gameplay";
    #endregion

    #region Editor Variables
    [SerializeField]
    [Tooltip("The starting data for the player.")]
    private PlayerData m_Data;
    #endregion

    #region Private Variables
    // The player's inventory
    private Inventory p_Inventory;

    private Dictionary<int, List<IEnumerator>> p_TimedEffects;
    #endregion

    #region Cached Components
    // A reference to the Player Input component to swap action maps
    private PlayerInput cc_Input;

    private PlayerWeapon cc_Weapon;
    #endregion

    #region Initialization
    private void Awake() {
        // Number of max passives subject to change based on item balancing
        m_Data.ResetAllStatsDefault();

        p_Inventory = new Inventory(10);
        p_TimedEffects = new Dictionary<int, List<IEnumerator>>();

        cc_Input = GetComponent<PlayerInput>();
        cc_Weapon = GetComponent<PlayerWeapon>();
    }
    #endregion

    #region Accessors
    public float GetMoveSpeed() {
        return m_Data.CurrMovementSpeed;
    }

    public PlayerData GetPlayerData() {
        return m_Data;
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
        int itemID = p_Inventory.DropSelectedItem();
        ItemManager.GetPassiveItem(itemID).RemoveEffect(this);
        DropManager.DropItem(itemID, transform.position);
    }

    private void OnDropWeapon() {
        p_Inventory.DropCurrentWeapon();
    }

    private void OnInteract() {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, m_Data.GetInteractionRadius(), Vector2.zero, 0, 1 << LayerMask.NameToLayer(Consts.ITEM_PHYSICS_LAYER));
        if (!hit) {
            return;
        }
        PickUpItem(hit.collider.GetComponent<ItemGameObject>());
    }
    #endregion

    #region Picking up
    private void PickUpItem(ItemGameObject item) {
        int itemID = item.GetID();
        p_Inventory.PickupItem(itemID);

        if (ItemManager.IsPassiveItem(itemID)) {
            PickUpPassiveItem(itemID);
        }

        Destroy(item.gameObject);
    }

    private void PickUpPassiveItem(int itemID) {
        BasePassiveItem item = ItemManager.GetPassiveItem(itemID);
        item.ApplyEffect(this);
    }
    #endregion

    #region Health Methods
    public void TakeDamage(int damage) {
        m_Data.TakeDamage(damage);
    }

    public void Heal(float m_HealPercent) {
        int healing = (int)(m_HealPercent * m_Data.MaxHealth);
        m_Data.Heal(healing);
    }
    #endregion

    #region Timed Effects
    public void AddTimedEffect(int itemID, EffectToApply effect, float repeatTime) {
        IEnumerator effectFunction = TimedEffect(effect, repeatTime);
        if (p_TimedEffects.ContainsKey(itemID)) {
            p_TimedEffects[itemID].Add(effectFunction);
        }
        else {
            List<IEnumerator> functionEffects = new List<IEnumerator>();
            functionEffects.Add(effectFunction);
            p_TimedEffects.Add(itemID, functionEffects);
        }
        StartCoroutine(effectFunction);
    }

    public void SubtractTimedEffect(int itemID) {
        if (!p_TimedEffects.ContainsKey(itemID)) {
            throw new System.ArgumentException($"Trying to subtract timed effect based on item ID {itemID} even though player {name} doesn't have that item.");
        }
        IEnumerator effectFunction = p_TimedEffects[itemID][0];
        StopCoroutine(effectFunction);
        p_TimedEffects[itemID].RemoveAt(0);
        if (p_TimedEffects[itemID].Count == 0) {
            p_TimedEffects.Remove(itemID);
        }
    }

    private IEnumerator TimedEffect(EffectToApply effect, float repeatTime) {
        while (true) {
            yield return new WaitForSeconds(repeatTime);

            effect(this);
        }
    }
    #endregion

    #region On Attack Effects
    public void AddOnAttackEffect(int itemID, WeaponBase.OnAttackEffect effect) {
        cc_Weapon.AddItemEffect(itemID, effect);
    }

    public void SubtractOnAttackEffect(int itemID) {
        cc_Weapon.SubtractItemEffect(itemID);
    }
    #endregion
}
