﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;
using UnityEngine.UI;
using System;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour {
    #region Events and Delegates
    public delegate void EffectToApply(PlayerManager player);
    public delegate void Death(int playerID, int respawnTime);
    public static event Death DeathEvent;
    public delegate void PlayerReady(int playerID, bool ready);
    public static event PlayerReady PlayerReadyEvent;
    #endregion

    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The starting data for the player.")]
    private PlayerData m_Data;
    #endregion

    #region Private Variables
    // The player's inventory
    private Inventory m_Inventory;

    private bool m_CycleInvRight;
    private bool m_CycleInvLeft;
    private float m_InvCyclePauseTimer;

    private float m_CooldownTimer;

    private Dictionary<int, List<IEnumerator>> m_TimedEffects;

    private Vector2 m_AimDir;

    private int m_PlayerID;

    private List<MonoBehaviour> m_ScriptsToDisable;
    #endregion
    #endregion

    #region Cached Components
    // A reference to the Player Input component to swap action maps
    private PlayerInput m_Input;

    private PlayerWeapon m_Weapon;

    // Need to cache sprite renderer and collider in order to disable on death
    private SpriteRenderer m_SpriteRenderer;
    private Collider2D m_Collider;
    private PlayerCanvas m_PlayerCanvas;
    #endregion

    #region Initialization
    private void Awake() {
        // Number of max passives subject to change based on item balancing
        m_Data.ResetAllStatsDefault();

        //Debugging RespawnTime
        m_Data.RespawnTime = Consts.BASE_RESPAWN_TIME;

        m_Inventory = new Inventory();
        m_TimedEffects = new Dictionary<int, List<IEnumerator>>();

        m_CycleInvRight = false;
        m_CycleInvLeft = false;
        m_InvCyclePauseTimer = 0;

        m_CooldownTimer = 0;

        m_TimedEffects = new Dictionary<int, List<IEnumerator>>();

        m_AimDir = Vector2.right;

        m_Input = GetComponent<PlayerInput>();
        m_Weapon = GetComponent<PlayerWeapon>();

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<Collider2D>();

        m_PlayerCanvas = transform.GetChild(0).GetComponent<PlayerCanvas>();

        m_ScriptsToDisable = new List<MonoBehaviour>();
        foreach (MonoBehaviour component in GetComponents(typeof(MonoBehaviour))) {
            if (!(component == this || component is PlayerInput)) {
                m_ScriptsToDisable.Add(component);
            }
        }
    }

    private void OnEnable() {
        GameManager.StartGameEvent += SwitchToGameplayActions;
        foreach (var component in m_ScriptsToDisable) {
            component.enabled = true;
        }

        m_SpriteRenderer.enabled = true;
        m_Collider.enabled = true;
        m_PlayerCanvas.SetSlider(m_Data.MaxHealth);
    }
    #endregion

    #region Main Updates
    private void Update() {
        ReduceActiveItemCooldown();
    }
    #endregion

    #region Accessors and Setters
    public float GetMoveSpeed() {
        return m_Data.CurrMovementSpeed;
    }

    public PlayerData GetPlayerData() {
        return m_Data;
    }

    public Vector2 GetAimDir() {
        return m_AimDir;
    }

    public int GetID() {
        return m_PlayerID;
    }

    public void SetID(int ID) {
        m_PlayerID = ID;
        PlayerCanvas deathCanvas = GetComponentInChildren<PlayerCanvas>();
        deathCanvas.PlayerID = ID;
    }
    #endregion

    #region Input Receivers
    private void OnOpenInventory() {
        m_Input.SwitchCurrentActionMap(Consts.INVENTORY_INPUT_ACTION_MAP_NAME);
        m_PlayerCanvas.EnableInventoryUI(m_PlayerID, m_Inventory);
        m_Inventory.OpenInventory();
    }

    private void OnCloseInventory() {
        m_Inventory.CloseInventory();
        m_PlayerCanvas.DisableInventoryUI(m_PlayerID);
        m_Input.SwitchCurrentActionMap(Consts.GAMEPLAY_INPUT_ACTION_MAP_NAME);
    }

    private void OnSwitchWeapons() {
        m_Inventory.SwitchWeapons();
    }

    private void OnDropItem() {
        try {
            int itemID = m_Inventory.DropSelectedItem();
            if (itemID != Consts.NULL_ITEM_ID) {
                DropItem(itemID);
                m_PlayerCanvas.ClearImageAtIndex(m_Inventory.GetSelectedItemIndex());
                m_PlayerCanvas.HighlightWedge(m_Inventory.GetSelectedItemIndex());
            }
        }
        catch (System.IndexOutOfRangeException) {
            Debug.LogWarning("No item at selected item index");
        }
    }

    private void OnDropWeapon() {
        m_Inventory.DropCurrentWeapon();
    }

    private void OnInteract() {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, m_Data.GetInteractionRadius(), Vector2.zero, 0, 1 << LayerMask.NameToLayer(Consts.ITEM_PHYSICS_LAYER));
        if (!hit) {
            return;
        }
        PickUpItem(hit.collider.GetComponent<ItemGameObject>());
    }

    private void OnUseActive(InputValue value) {
        if (m_CooldownTimer > 0) {
            return;
        }

        if (value.isPressed) {
            int id = m_Inventory.GetCurrentActive();
            if (id != Consts.NULL_ITEM_ID) {
                BaseActiveItem item = ItemManager.GetActiveItem(id);
                item.UseEffect(this);
                m_CooldownTimer = item.GetCooldown();
            }
        }
    }

    private void OnAim(InputValue value) {
        Vector2 dir = value.Get<Vector2>();
        if (dir.sqrMagnitude > Consts.SQR_MAG_CLOSE_TO_ZERO_HIGH) {
            m_AimDir = dir;
        }
    }

    private void OnSelect(InputValue value) {
        // Get aim vector
        Vector2 aimVector = value.Get<Vector2>();

        // Don't do anything on release; aim vector will be (0, 0)
        if (Math.Abs(aimVector.x) < Mathf.Epsilon && Math.Abs(aimVector.y) < Mathf.Epsilon) {
            return;
        }

        // Calculate angle (flip if aim vector points to the right)
        float angle = Vector2.Angle(Vector2.up, aimVector);
        if (aimVector.x > 0) {
            angle = 360 - angle;
        }

        // Set up variables
        int selectIndex = 0;
        float wedgeAngle = 360 / m_Inventory.GetInventoryCapacity();

        // Special case: selectIndex is 0 (top wedge wraps around between 337.5 and 22.5 degrees)

        if (angle > 360 - wedgeAngle / 2 || angle <= wedgeAngle / 2) {
            selectIndex = 0;
        }

        // Calculate selectIndex
        else {
            for (int i = 1; i < m_Inventory.GetInventoryCapacity(); i++) {
                if (angle <= wedgeAngle / 2 + wedgeAngle * i) {
                    selectIndex = i;
                    break;
                }
            }
        }

        // Select that item
        m_Inventory.SelectItemAtIndex(selectIndex);

        // Highlight the corresponding wedge
        m_PlayerCanvas.HighlightWedge(m_Inventory.GetSelectedItemIndex());
    }

    private void OnReady() {
        PlayerReadyEvent(m_PlayerID, true);
    }

    private void OnUnready() {
        PlayerReadyEvent(m_PlayerID, false);
    }
    #endregion

    #region For Matthew
    private void OnSwapToMatthew() {
        m_Input.SwitchCurrentActionMap("MatthewGameplay");
    }

    private void OnSwapToRegular() {
        m_Input.SwitchCurrentActionMap(Consts.GAMEPLAY_INPUT_ACTION_MAP_NAME);
    }
    #endregion

    #region Picking up and Dropping Items
    private void PickUpItem(ItemGameObject item) {
        int itemID = item.GetID();
        if (ItemManager.IsActiveItem(itemID) &&
            itemID == m_Inventory.GetCurrentActive()) {
            return;
        }
        int retItem = m_Inventory.PickupItem(itemID);
        if (retItem == Consts.NULL_ITEM_ID) {
            return;
        }

        if (retItem != itemID) {
            DropItem(retItem);
        }
        if (ItemManager.IsPassiveItem(itemID)) {
            PickUpPassiveItem(itemID);
        }
        Destroy(item.gameObject);
    }

    private void PickUpPassiveItem(int itemID) {
        BasePassiveItem item = ItemManager.GetPassiveItem(itemID);
        item.ApplyEffect(this);
    }

    private void DropItem(int itemID) {
        if (ItemManager.IsPassiveItem(itemID)) {
            ItemManager.GetPassiveItem(itemID).RemoveEffect(this);
        }
        else if (ItemManager.IsActiveItem(itemID)) {
            ItemManager.GetActiveItem(itemID).CancelEffect();
        }
        DropManager.DropItem(itemID, transform.position);
    }
    #endregion

    #region Health Methods
    public void TakeDamage(int damage) {
        m_Data.TakeDamage(damage);
        m_PlayerCanvas.SliderDamage(damage);
        if (m_Data.CurrHealth <= 0) {
            DeathEvent(m_PlayerID, m_Data.RespawnTime);
        }
    }

    public void Heal(float m_HealPercent) {
        int healing = (int)(m_HealPercent * m_Data.MaxHealth);
        m_Data.Heal(healing);
        m_PlayerCanvas.SliderHeal(healing);
    }
    #endregion

    #region Timed Effects
    public void AddTimedEffect(int itemID, EffectToApply effect, float repeatTime) {
        IEnumerator effectFunction = TimedEffect(effect, repeatTime);
        if (m_TimedEffects.ContainsKey(itemID)) {
            m_TimedEffects[itemID].Add(effectFunction);
        }
        else {
            List<IEnumerator> functionEffects = new List<IEnumerator>();
            functionEffects.Add(effectFunction);
            m_TimedEffects.Add(itemID, functionEffects);
        }
        StartCoroutine(effectFunction);
    }

    public void SubtractTimedEffect(int itemID) {
        if (!m_TimedEffects.ContainsKey(itemID)) {
            throw new System.ArgumentException($"Trying to subtract timed effect based on item ID {itemID} even though player {name} doesn't have that item.");
        }
        IEnumerator effectFunction = m_TimedEffects[itemID][0];
        StopCoroutine(effectFunction);
        m_TimedEffects[itemID].RemoveAt(0);
        if (m_TimedEffects[itemID].Count == 0) {
            m_TimedEffects.Remove(itemID);
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
    public void AddOnAttackEffect(int itemID,
        WeaponBase.OnAttackEffect effect) {
        m_Weapon.AddItemEffect(itemID, effect);
    }

    public void SubtractOnAttackEffect(int itemID) {
        m_Weapon.SubtractItemEffect(itemID);
    }

    public void AddOnAttackEffectForXSec(int itemID,
        WeaponBase.OnAttackEffect effect, float effectLength) {
        StartCoroutine(AddTempOnAttackEffect(itemID, effect, effectLength));
    }

    private IEnumerator AddTempOnAttackEffect(int itemID,
        WeaponBase.OnAttackEffect effect, float effectLength) {
        AddOnAttackEffect(itemID, effect);
        yield return new WaitForSeconds(effectLength);
        SubtractOnAttackEffect(itemID);
    }
    #endregion

    #region Cooldown
    private void ReduceActiveItemCooldown() {
        if (m_CooldownTimer > 0) {
            m_CooldownTimer -= Time.deltaTime;
        }
    }
    #endregion

    #region Status Effects
    public void AddStatusEffectForXSec(Status status, float effectLength) {
        StartCoroutine(AddTempStatusEffect(status, effectLength));
    }

    private IEnumerator AddTempStatusEffect(Status status, float effectLength) {
        m_Data.AddStatus(status);
        yield return new WaitForSeconds(effectLength);
        m_Data.RemoveStatus(status);
    }
    #endregion

    #region OnDisable And Other Enders
    private void OnDisable() {
        GameManager.StartGameEvent -= SwitchToGameplayActions;
        foreach (var component in m_ScriptsToDisable) {
            component.enabled = false;
        }
        m_SpriteRenderer.enabled = false;
        m_Collider.enabled = false;
    }
    #endregion

    #region Game Start Methods
    private void SwitchToGameplayActions() {
        m_Input.SwitchCurrentActionMap(Consts.GAMEPLAY_INPUT_ACTION_MAP_NAME);
    }
    #endregion
}
