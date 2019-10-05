using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerWeaponInterface {
    void Unequip();
    void Equip(int weaponID);

    void Use();
    void StopUse();
}

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerWeapon : MonoBehaviour, PlayerWeaponInterface {
    #region Private Variables
    private bool m_HasWeaponEquipped;
    private int m_WeaponID;
    private bool m_IsInUse;
    private float m_CooldownTimer;

    private bool m_IsInAnimation;
    private float m_WindupTime;
    private float m_AttackTime;
    private float m_AnimationLength;

    private WeaponBaseData m_WeaponBaseData;
    private List<ItemAndEffect> m_OnAttackEffects;
    #endregion

    #region Cached Components
    private Animator m_Animator;
    #endregion

    #region Cached References
    private WeaponInterface m_Weapon;
    #endregion

    #region Initialization
    private void Awake() {
        m_Animator = GetComponent<Animator>();

        m_HasWeaponEquipped = false;
        m_WeaponID = Consts.NULL_ITEM_ID;
        m_IsInUse = false;
        m_IsInAnimation = false;

        m_OnAttackEffects = new List<ItemAndEffect>();
    }

    private void Start() {
        m_Weapon = GetComponentInChildren<WeaponInterface>();
        if (m_Weapon == null) {
            Debug.LogError("Could not find WeaponBase.");
        }
        Invoke("DeactivateWeapon", 0.2f);
    }
    #endregion

    #region Main Updates
    private void Update() {
        if (m_IsInUse && CanUse()) {
            StartCoroutine(UseWeapon());
        }
        else if (WaitingOnCooldown()) {
            m_CooldownTimer -= Time.deltaTime;
        }
        else {
            m_CooldownTimer = 0;
        }
    }
    #endregion

    #region Resetters
    private void ResetCooldown() {
        m_CooldownTimer = Consts.ATTACK_SPEED_MULTIPLIER * 1 / m_WeaponBaseData.GetAttackSpeed();
    }
    #endregion

    #region Checkers
    private bool CanUse() {
        if (!m_HasWeaponEquipped)
            return false;
        if (m_IsInAnimation)
            return false;
        if (m_CooldownTimer > 0)
            return false;

        return true;
    }

    private bool WaitingOnCooldown() {
        return m_CooldownTimer > 0;
    }
    #endregion

    #region Equip Helper Methods
    private void SetWeaponData() {
        string type = "ShortBlade";
        m_WindupTime = WeaponTypeDataManager.singleton.WindupTime(type);
        m_AttackTime = WeaponTypeDataManager.singleton.AttackTime(type);
        m_AnimationLength = WeaponTypeDataManager.singleton.AnimationTime(type);

        // TODO: set rest of data
        if (m_WeaponID == 0) {
            m_WeaponBaseData = new WeaponBaseData(0, 0, 1, Utility.LoadSpriteFile(Consts.TEST_SWORD_1_SPRITE_PATH));
        }
        else if (m_WeaponID == 1) {
            m_WeaponBaseData = new WeaponBaseData(0, 0, 1, Utility.LoadSpriteFile(Consts.TEST_SWORD_2_SPRITE_PATH));
        }
    }

    private void ApplyWeaponData() {
        m_Weapon.UpdateGraphics(m_WeaponBaseData);
    }
    #endregion

    #region Use Methods
    private IEnumerator UseWeapon() {
        StartAnimations();

        yield return new WaitForSeconds(m_WindupTime);

        ActivateWeapon();

        yield return new WaitForSeconds(m_AttackTime - m_WindupTime);

        DeactivateWeapon();

        yield return new WaitForSeconds(m_AnimationLength - m_AttackTime);

        EndAnimations();

        ResetCooldown();
    }

    private void StartAnimations() {
        m_IsInAnimation = true;

        m_Animator.SetTrigger(Consts.USE_WEAPON_ANIMATOR_TRIGGER);
    }

    private void EndAnimations() {
        m_IsInAnimation = false;
    }

    private void ActivateWeapon() {
        WeaponBase.OnAttackEffect[] effects = new WeaponBase.OnAttackEffect[m_OnAttackEffects.Count];
        for (int i = 0; i < m_OnAttackEffects.Count; i++) {
            effects[i] = m_OnAttackEffects[i].Effect;
        }
        m_Weapon.Activate(m_WeaponBaseData, effects);
    }

    private void DeactivateWeapon() {
        m_Weapon.Deactivate();
    }
    #endregion

    #region Interface Required Methods
    public void Equip(int weaponID) {
        m_WeaponID = weaponID;

        // TODO: Get weapon data
        // TODO: Set weapon data
        SetWeaponData();
        ApplyWeaponData();
        
        m_HasWeaponEquipped = true;
    }

    public void Unequip() {
        m_HasWeaponEquipped = false;
    }

    public void Use() {
        m_IsInUse = true;
    }

    public void StopUse() {
        m_IsInUse = false;
    }
    #endregion

    #region Input Receivers
    // Executes attack functionality when player attack input is received
    private void OnAttack() {
        Debug.Log("Detected attack input");
    }
    #endregion

    #region On Attack Effect
    public void AddItemEffect(int itemID, WeaponBase.OnAttackEffect effect) {
        m_OnAttackEffects.Add(new ItemAndEffect(itemID, effect));
    }

    public void SubtractItemEffect(int itemID) {
        for (int i = 0; i < m_OnAttackEffects.Count; i++) {
            if (m_OnAttackEffects[i].ItemID == itemID) {
                m_OnAttackEffects.RemoveAt(i);
                break;
            }
        }
    }

    private struct ItemAndEffect {
        public int ItemID;
        public WeaponBase.OnAttackEffect Effect;

        public ItemAndEffect(int itemID, WeaponBase.OnAttackEffect effect) {
            ItemID = itemID;
            Effect = effect;
        }
    }
    #endregion
}
