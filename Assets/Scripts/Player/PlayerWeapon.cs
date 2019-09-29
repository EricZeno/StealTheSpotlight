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
    private bool p_HasWeaponEquipped;
    private int p_WeaponID;
    private bool p_IsInUse;
    private float p_CooldownTimer;

    private bool p_IsInAnimation;
    private float p_WindupTime;
    private float p_AttackTime;
    private float p_AnimationLength;

    private WeaponBaseData p_WeaponBaseData;
    private List<ItemAndEffect> p_OnAttackEffects;
    #endregion

    #region Cached Components
    private Animator cc_Animator;
    #endregion

    #region Cached References
    private WeaponInterface cr_Weapon;
    #endregion

    #region Initialization
    private void Awake() {
        cc_Animator = GetComponent<Animator>();

        p_HasWeaponEquipped = false;
        p_WeaponID = -1;    // TODO: Set to not a weapon
        p_IsInUse = false;
        p_IsInAnimation = false;

        p_OnAttackEffects = new List<ItemAndEffect>();
    }

    private void Start() {
        cr_Weapon = GetComponentInChildren<WeaponInterface>();
        if (cr_Weapon == null) {
            Debug.LogError("Could not find WeaponBase.");
        }
        cr_Weapon.Deactivate();
    }
    #endregion

    #region Main Updates
    private void Update() {
        if (p_IsInUse && CanUse()) {
            StartCoroutine(UseWeapon());
        }
        else if (WaitingOnCooldown()) {
            p_CooldownTimer -= Time.deltaTime;
        }
        else {
            p_CooldownTimer = 0;
        }
    }
    #endregion

    #region Resetters
    private void ResetCooldown() {
        p_CooldownTimer = Consts.ATTACK_SPEED_MULTIPLIER * 1 / p_WeaponBaseData.GetAttackSpeed();
    }
    #endregion

    #region Checkers
    private bool CanUse() {
        if (!p_HasWeaponEquipped)
            return false;
        if (p_IsInAnimation)
            return false;
        if (p_CooldownTimer > 0)
            return false;

        return true;
    }

    private bool WaitingOnCooldown() {
        return p_CooldownTimer > 0;
    }
    #endregion

    #region Equip Helper Methods
    private void SetWeaponData() {
        string type = "ShortBlade";
        p_WindupTime = WeaponTypeDataManager.singleton.WindupTime(type);
        p_AttackTime = WeaponTypeDataManager.singleton.AttackTime(type);
        p_AnimationLength = WeaponTypeDataManager.singleton.AnimationTime(type);

        // TODO: set rest of data
        if (p_WeaponID == 0) {
            p_WeaponBaseData = new WeaponBaseData(0, 0, 1, Utility.LoadSpriteFile(Consts.TEST_SWORD_1_SPRITE_PATH));
        }
        else if (p_WeaponID == 1) {
            p_WeaponBaseData = new WeaponBaseData(0, 0, 1, Utility.LoadSpriteFile(Consts.TEST_SWORD_2_SPRITE_PATH));
        }
    }

    private void ApplyWeaponData() {
        cr_Weapon.UpdateGraphics(p_WeaponBaseData);
    }
    #endregion

    #region Use Methods
    private IEnumerator UseWeapon() {
        StartAnimations();

        yield return new WaitForSeconds(p_WindupTime);

        ActivateWeapon();

        yield return new WaitForSeconds(p_AttackTime - p_WindupTime);

        DeactivateWeapon();

        yield return new WaitForSeconds(p_AnimationLength - p_AttackTime);

        EndAnimations();

        ResetCooldown();
    }

    private void StartAnimations() {
        p_IsInAnimation = true;

        cc_Animator.SetTrigger(Consts.USE_WEAPON_ANIMATOR_TRIGGER);
    }

    private void EndAnimations() {
        p_IsInAnimation = false;
    }

    private void ActivateWeapon() {
        WeaponBase.OnAttackEffect[] effects = new WeaponBase.OnAttackEffect[p_OnAttackEffects.Count];
        for (int i = 0; i < p_OnAttackEffects.Count; i++) {
            effects[i] = p_OnAttackEffects[i].Effect;
        }
        cr_Weapon.Activate(p_WeaponBaseData, effects);
    }

    private void DeactivateWeapon() {
        cr_Weapon.Deactivate();
    }
    #endregion

    #region Interface Required Methods
    public void Equip(int weaponID) {
        p_WeaponID = weaponID;

        // TODO: Get weapon data
        // TODO: Set weapon data
        SetWeaponData();
        ApplyWeaponData();
        
        p_HasWeaponEquipped = true;
    }

    public void Unequip() {
        p_HasWeaponEquipped = false;
    }

    public void Use() {
        p_IsInUse = true;
    }

    public void StopUse() {
        p_IsInUse = false;
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
        p_OnAttackEffects.Add(new ItemAndEffect(itemID, effect));
    }

    public void SubtractItemEffect(int itemID) {
        for (int i = 0; i < p_OnAttackEffects.Count; i++) {
            if (p_OnAttackEffects[i].ItemID == itemID) {
                p_OnAttackEffects.RemoveAt(i);
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
