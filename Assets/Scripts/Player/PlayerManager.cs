using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;
using UnityEngine.UI;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerWeapon))]
[RequireComponent(typeof(PlayerGraphics))]
[RequireComponent(typeof(PlayerCanvas))]
[RequireComponent(typeof(Collider2D))]
public class PlayerManager : MonoBehaviour {
    #region Events and Delegates
    public delegate void EffectToApply(PlayerManager player);
    public delegate void Death(int playerID, int respawnTime);
    public static event Death DeathEvent;
    public delegate void PK(int player, int playerkilled);
    public static event PK PKEvent;
    public delegate void DropSpotlight(float x, float y);
    public static event DropSpotlight DropSpotlightEvent;
    public delegate void Pause();
    public static event Pause PauseEvent;
    public delegate void Select(string input);
    public static event Select SelectEvent;
    #endregion

    #region Interface
    public void OpenInventory() {
        SwitchInputMap(InputMaps.INVENTORY);
    }

    public void CloseInventory() {
        SwitchInputMap(InputMaps.GAMEPLAY);
    }

    public void ApplyPassiveItemEffect(int itemID) {
        if (!ItemManager.IsPassiveItem(itemID)) {
            throw new System.ArgumentException("This function requires a " +
                $"passive item. The provided item ID is {itemID}.");
        }

        ApplyPassiveItemEffect(ItemManager.GetPassiveItem(itemID));
    }
    #endregion

    #region Constants
    private const string INVENTORY_INPUT_ACTION_MAP_NAME = "Inventory";
    private const string GAMEPLAY_INPUT_ACTION_MAP_NAME = "Gameplay";
    #endregion

    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The starting data for the player.")]
    private PlayerData m_Data;

    [SerializeField]
    [Tooltip("All of the scripts the get disabled when player dies.")]
    private MonoBehaviour[] m_ScriptsToDisable;

    [SerializeField]
    [Tooltip("All of the game objects that get disabled when player dies.")]
    private GameObject[] m_ObjectsToDisable;
    #endregion

    #region Private Variables
    private Dictionary<int, List<IEnumerator>> m_TimedEffects;

    private Vector2 m_AimDir;

    private int m_PlayerID;

    private bool m_IsFlashing;
    #endregion

    #region Cached Components
    // A reference to the Player Input component to swap action maps
    private PlayerInput m_Input;
    public PlayerWeapon m_Weapon { get; private set; }
    private PlayerGraphics m_Graphics;
    private PlayerCanvas m_UI;
    private Collider2D m_Collider;
    private PlayerInventoryController m_Inventory;
    #endregion

    #region Cached References
    private PlayerCanvas m_PlayerCanvas;
    private GameObject m_WeaponObject;
    private AudioManager m_AudioManager;
    #endregion
    #endregion

    #region Initialization
    #region Awake
    private void Awake() {
        // Number of max passives subject to change based on item balancing
        m_Data.ResetAllStatsDefault();

        m_TimedEffects = new Dictionary<int, List<IEnumerator>>();

        SetupBasicVariables();
        SetupCachedComponents();
        SetupCachedReferences();
        m_AudioManager = GetComponent<AudioManager>();
    }

    private void SetupBasicVariables() {
        m_AimDir = Vector2.right;
    }

    private void SetupCachedComponents() {
        m_Input = GetComponent<PlayerInput>();
        m_Weapon = GetComponent<PlayerWeapon>();
        m_Graphics = GetComponent<PlayerGraphics>();
        m_UI = GetComponent<PlayerCanvas>();
        m_Collider = GetComponent<Collider2D>();
        m_Inventory = GetComponent<PlayerInventoryController>();
    }

    private void SetupCachedReferences() {
        m_WeaponObject = transform.Find(Consts.WEAPON_OBJECT_NAME).gameObject;
        m_PlayerCanvas = transform.GetComponentInChildren<PlayerCanvas>();
    }
    #endregion

    #region OnEnable
    private void OnEnable() {
        EnableScriptsAndComponents();
        ResetComponentsToDefaults();
    }

    private void EnableScriptsAndComponents() {
        foreach (var component in m_ScriptsToDisable) {
            component.enabled = true;
        }
        foreach (var gameObject in m_ObjectsToDisable) {
            gameObject.SetActive(true);
        }

        m_Graphics.Show();
        m_Collider.enabled = true;
    }

    private void ResetComponentsToDefaults() {
        m_PlayerCanvas.SetSlider(m_Data.MaxHealth);
    }
    #endregion

    public void InitialSetup(PlayerSprites sprites) {
        m_Graphics.SetupSprites(sprites);
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
    }

    public float InteractionRadius {
        get {
            return m_Data.GetInteractionRadius();
        }
    }
    #endregion

    #region Input Receivers
    private void OnAim(InputValue value) {
        Vector2 dir = value.Get<Vector2>();
        if (dir.sqrMagnitude > Consts.SQR_MAG_CLOSE_TO_ZERO_HIGH) {
            m_AimDir = dir;
        }
    }

    private void OnPause(InputValue value) {
        PauseEvent();
    }

    private void OnUnpause(InputValue value) {
        PauseEvent();
    }

    private void OnUp(InputValue value) {
        SelectEvent("up");
    }

    private void OnDown(InputValue value) {
        SelectEvent("down");
    }

    private void OnChoose(InputValue value) {
        SelectEvent("choose");
    }
    #endregion

    #region Swap Input Maps
    private void OnSwapToMatthew() {
        m_Input.SwitchCurrentActionMap("MatthewGameplay");
    }

    public void PauseMap() {
        m_Input.SwitchCurrentActionMap("Pause");
    }

    public void EndMap() {
        m_Input.SwitchCurrentActionMap("End");
    }

    public void RegularMap() {
        SwitchInputMap(InputMaps.GAMEPLAY);
    }

    private void OnSwapToRegular() {
        SwitchInputMap(InputMaps.GAMEPLAY);
    }
    #endregion

    #region Item effects
    private void ApplyPassiveItemEffect(BasePassiveItem item) {
        item.ApplyEffect(this);
    }

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

    public void RemoveTimedEffect(int itemID) {
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
    public void AddOnAttackEffect(int itemID, PlayerWeapon.OnAttackEffect effect) {
        m_Weapon.AddItemEffect(itemID, effect);
    }

    public void SubtractOnAttackEffect(int itemID) {
        m_Weapon.SubtractItemEffect(itemID);
    }

    public void AddOnAttackEffectForXSec(int itemID,
        PlayerWeapon.OnAttackEffect effect, float effectLength) {
        StartCoroutine(AddTempOnAttackEffect(itemID, effect, effectLength));
    }

    private IEnumerator AddTempOnAttackEffect(int itemID,
        PlayerWeapon.OnAttackEffect effect, float effectLength) {
        AddOnAttackEffect(itemID, effect);
        yield return new WaitForSeconds(effectLength);
        SubtractOnAttackEffect(itemID);
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
    #endregion

    #region Health Methods
    public void TakeDamage(int damage, PlayerManager from = null) {
        TakeDamage(null, damage, from);
        TakeDamageSounds();
    }

    public void TakeDamage(BaseWeaponItem weaponUsed, int damage, PlayerManager from = null) {
        m_Data.TakeDamage(damage);

        if (!m_IsFlashing) {
            StartCoroutine(DamageFlash());
        }

        TakeDamageSounds();
        m_PlayerCanvas.SliderDamage(damage);
        if (m_Data.CurrHealth <= 0) {
            if (weaponUsed != null) {
                weaponUsed.OnKillEnemy();
            }

            Die();
            m_AudioManager.Play("Death_Player");
            if (from != null) {
                PKEvent(from.GetID(), GetID());
            }
            else {
                DropSpotlightEvent(gameObject.transform.position.x, gameObject.transform.position.y);
            }
        }
    }

    private void TakeDamageSounds() {
        int randomNumber = UnityEngine.Random.Range(1, 4);
        float randomPitch = UnityEngine.Random.Range(0.8f, 1.3f);
        switch (randomNumber) {
            case 1:
                m_AudioManager.Play("Light Hit 1", randomPitch);
                break;
            case 2:
                m_AudioManager.Play("Heavy Hit 1", randomPitch);
                break;
            case 3:
                m_AudioManager.Play("Heavy Hit 2", randomPitch);
                break;
            default:
                m_AudioManager.Play("Light Hit 1", randomPitch);
                break;
        }
    }

    public void Heal(float m_HealPercent) {
        int healing = (int)(m_HealPercent * m_Data.MaxHealth);
        m_Data.Heal(healing);
        m_PlayerCanvas.SliderHeal(healing);
    }

    public void Die() {
        m_UI.StartDeath(m_Data.RespawnTime);
        DeathEvent(m_PlayerID, m_Data.RespawnTime);
    }

    private IEnumerator DamageFlash() {
        m_IsFlashing = true;

        float flashDelay = .1f;
        Color flashColor = Color.red;
        int numOfFlashes = 1;
        for (int i = 0; i < numOfFlashes; i++) {
            m_Graphics.SetColor(flashColor);
            yield return new WaitForSeconds(flashDelay);
            m_Graphics.SetColor(Color.white);
            yield return new WaitForSeconds(flashDelay);
        }

        m_IsFlashing = false;
    }
    #endregion

    #region OnDisable And Other Enders
    private void OnDisable() {
        DisableScriptsAndComponents();
    }

    private void DisableScriptsAndComponents() {
        foreach (var component in m_ScriptsToDisable) {
            component.enabled = false;
        }
        foreach (var gameObject in m_ObjectsToDisable) {
            gameObject.SetActive(false);
        }

        m_Graphics.Hide();
        m_Collider.enabled = false;
    }
    #endregion

    #region Game Start Methods
    private void SwitchToGameplayActions() {
        SwitchInputMap(InputMaps.GAMEPLAY);
    }

    public void EquipStartingWeapon() {
        m_Inventory.EquipItem(Consts.STARTING_WEAPON_ID);
    }
    #endregion

    #region Input Maps
    private void SwitchInputMap(InputMaps newMap) {
        string mapName = "";
        switch (newMap) {
            case InputMaps.INVENTORY:
                mapName = INVENTORY_INPUT_ACTION_MAP_NAME;
                break;
            case InputMaps.GAMEPLAY:
                mapName = GAMEPLAY_INPUT_ACTION_MAP_NAME;
                break;
            default:
                throw new System.ArgumentException("Trying to change input map to one " +
                    "that has not been set up yet.");
        }
        m_Input.SwitchCurrentActionMap(mapName);
    }
    #endregion

    #region Enums
    private enum InputMaps {
        INVENTORY,
        GAMEPLAY
    }
    #endregion

    #region UI
    public void PointUI(float curr, int end) {
        m_PlayerCanvas.SetPoints(curr, end);
    }
    #endregion
}