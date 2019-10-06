using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour {
    #region Events and Delegates
    public delegate void EffectToApply(PlayerManager player);
    public delegate void Death(int playerID, int respawnTime);
    public static event Death DeathEvent;
    #endregion

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

    #region Cached Components
    // A reference to the Player Input component to swap action maps
    private PlayerInput m_Input;

    private PlayerWeapon m_Weapon;

    // Need to cache sprite renderer and collider in order to disable on death
    private SpriteRenderer m_SpriteRenderer;
    private Collider2D m_Collider;
	#endregion

	#region Initialization
	private void Awake() {
		// Number of max passives subject to change based on item balancing
		m_Data.ResetAllStatsDefault();

        //Debugging RespawnTime
        m_Data.RespawnTime = Consts.BASE_RESPAWN_TIME;

		m_Inventory = new Inventory(10);
		m_TimedEffects = new Dictionary<int, List<IEnumerator>>();

        m_Inventory = new Inventory(Consts.NUM_MAX_PASSIVES_IN_INV);

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

        m_ScriptsToDisable = new List<MonoBehaviour>();
        foreach (MonoBehaviour component in GetComponents(typeof(MonoBehaviour))) {
            if (!(component == this || component is PlayerInput)) {
                m_ScriptsToDisable.Add(component);
            }
        }
    }

    private void OnEnable() {
        foreach (var component in m_ScriptsToDisable) {
            component.enabled = true;
        }

        m_SpriteRenderer.enabled = true;
        m_Collider.enabled = true;
    }
    #endregion

    #region Main Updates
    private void Update() {
        CycleInventory();
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
        DeathCanvas deathCanvas = GetComponentInChildren<DeathCanvas>();
        deathCanvas.PlayerID = ID;
	}
	#endregion

    #region Input Receivers
    private void OnCycle(InputValue value) {
        // TODO: first two items cycle slowly but then it cycles really fast
        float dir = value.Get<Vector2>().x;
        if (dir > Consts.INV_CYCLE_THRESHOLD) {
            m_CycleInvRight = true;
            m_CycleInvLeft = false;
        }
        else if (dir < -Consts.INV_CYCLE_THRESHOLD) {
            m_CycleInvRight = false;
            m_CycleInvLeft = true;
        }
        else {
            m_CycleInvRight = false;
            m_CycleInvLeft = false;
        }
    }

    private void OnOpenInventory() {
        m_Input.SwitchCurrentActionMap(Consts.INVENTORY_INPUT_ACTION_MAP_NAME);
        m_Inventory.OpenInventory();
    }

    private void OnCloseInventory() {
        m_Inventory.CloseInventory();
        m_Input.SwitchCurrentActionMap(Consts.GAMEPLAY_INPUT_ACTION_MAP_NAME);
    }

    private void OnSwitchWeapons() {
        m_Inventory.SwitchWeapons();
    }

    private void OnDropItem() {
        try {
            int itemID = m_Inventory.DropSelectedItem();
            DropItem(itemID);
        }
        catch (System.IndexOutOfRangeException) { }
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
        else if (retItem != itemID) {
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
        if (m_Data.CurrHealth <= 0) {
            DeathEvent(m_PlayerID, m_Data.RespawnTime);
        }
    }

    public void Heal(float m_HealPercent) {
        int healing = (int)(m_HealPercent * m_Data.MaxHealth);
        m_Data.Heal(healing);
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

    #region Inventory Cycling
    private void CycleInventory() {
        if (m_CycleInvLeft && m_CycleInvRight) {
            throw new System.InvalidProgramException("The player is trying " +
                "to cycle left and right at the same time.");
        }

        if (m_InvCyclePauseTimer > 0) {
            m_InvCyclePauseTimer -= Time.deltaTime;
            return;
        }
        if (m_CycleInvRight) {
            m_Inventory.CycleRight();
        }
        else if (m_CycleInvLeft) {
            m_Inventory.CycleLeft();
        }
        else {
            return;
        }
        m_InvCyclePauseTimer = Consts.TIME_INV_CYCLE_PAUSED;
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
		foreach (var component in m_ScriptsToDisable) {
				component.enabled = false;
			}

        m_SpriteRenderer.enabled = false;
        m_Collider.enabled = false;
    }
	#endregion
}
