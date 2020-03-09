using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;

public struct WeaponBaseData {
    #region Someone else decide on a name for this region
    public int Damage;
    public float KnockbackPower;
    #endregion

    #region Constructors
    public WeaponBaseData(int damage, float knockbackPower) {
        Damage = damage;
        KnockbackPower = knockbackPower;
    }
    #endregion
}

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(Animator))]
public class PlayerWeapon : MonoBehaviour {
    #region Delegates
    public delegate WeaponBaseData OnAttackEffect(WeaponBaseData origData);
    #endregion

    #region Variables
    #region Private Variables
    private int m_WeaponID;
    private BaseWeaponItem m_WeaponData;

    private bool m_HasWeaponEquipped;
    private bool m_IsInUse;
    private bool m_IsAttacking;

    private Vector2 m_CurrentDirection;

    private List<ItemAndEffect> m_OnAttackEffects;
    #endregion

    #region Cached Components
    private PlayerManager m_Manager;
    #endregion

    #region Cached References
    private GameObject m_Weapon;
    private SpriteRenderer m_WeaponSprite;
    private Animator m_WeaponAnimator;
    private AudioManager m_Audio;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_WeaponID = Consts.NULL_ITEM_ID;
        m_WeaponData = null;

        m_HasWeaponEquipped = false;
        m_IsInUse = false;
        m_IsAttacking = false;

        m_CurrentDirection = Vector2.right;

        m_OnAttackEffects = new List<ItemAndEffect>();

        m_Manager = GetComponent<PlayerManager>();
        m_Audio = GetComponent<AudioManager>();
    }

    private void Start() {
        m_Weapon = transform.Find(Consts.WEAPON_OBJECT_NAME).gameObject;
        if (m_Weapon == null) {
            Debug.LogError("Could not find weapon.");
        }
        m_WeaponSprite = m_Weapon.GetComponent<SpriteRenderer>();
        m_WeaponAnimator = m_Weapon.GetComponent<Animator>();
    }
    #endregion

    #region Main Updates
    private void Update() {
        if (m_IsInUse && CanUse()) {
            StartCoroutine(UseWeapon());
        }
    }
    #endregion

    #region Accessors
    public bool IsAttacking {
        get {
            return m_IsAttacking;
        }
    }
    #endregion

    #region Checkers
    private bool CanUse() {
        if (!m_HasWeaponEquipped) {
            return false;
        }
        if (m_IsAttacking) {
            return false;
        }

        return true;
    }
    #endregion

    #region Equip Helper Methods
    private void SetWeaponData() {
        m_WeaponData = ItemManager.GetWeaponItem(m_WeaponID);
        m_WeaponData.Manager = m_Manager;
    }

    private void ApplyWeaponData() {
        m_WeaponSprite.sprite = m_WeaponData.GetIcon();
        m_WeaponAnimator.SetBool(m_WeaponData.AnimationBool, true);

        StartCoroutine(RunEverySecond());
    }
    #endregion

    #region Use Methods
    private IEnumerator UseWeapon() {
        m_WeaponAnimator.SetBool(m_WeaponData.AnimationBool, true);
        m_IsAttacking = true;

        m_WeaponAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(m_WeaponAnimator.GetCurrentAnimatorStateInfo(0).length + .1f);

        //Special attack effects
        m_WeaponData.Attack();

        if (m_WeaponData.IsRanged) {
            FireProjectile();
        }

        m_IsAttacking = false;
    }

    private void FireProjectile() {
        //Create projectiles here
    }

    public void HitEnemy(GameObject enemy) {
        WeaponBaseData data = new WeaponBaseData(
            m_Manager.GetPlayerData().GetPower() + m_WeaponData.Damage,
            m_WeaponData.KnockbackPower);
        data = ApplyOnAttackEffects(data);
        if (enemy.CompareTag(Consts.PLAYER_TAG)) {
            PlayerManager other = enemy.GetComponent<PlayerManager>();
            other.TakeDamage(m_WeaponData, data.Damage, m_Manager);
            Vector2 dir = (other.transform.position - transform.position).normalized;
            other.GetComponent<PlayerMovement>().ApplyExternalForce(dir * data.KnockbackPower);
        }
        else if (enemy.CompareTag(Consts.GENERAL_ENEMY_TAG)) {
            EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
            enemyManager.TakeDamage(m_WeaponData, data.Damage, m_Manager.GetID());
            Vector2 dir = (enemyManager.transform.position - transform.position).normalized;
            enemyManager.GetComponent<EnemyMovement>().ApplyExternalForce(dir * data.KnockbackPower);
        }

        m_Audio.Play("Light Hit 1");
    }

    private WeaponBaseData ApplyOnAttackEffects(WeaponBaseData origValues) {
        WeaponBaseData result = origValues;
        foreach (var itemAndEffect in m_OnAttackEffects) {
            result = itemAndEffect.Effect(result);
        }
        return result;
    }

    private IEnumerator RunEverySecond() {
        while (true) {
            m_WeaponData.RunEverySecond();

            yield return new WaitForSeconds(1f);
        }
    }

    public void SetAnimationBool(string animation, bool b) {
        m_WeaponAnimator.SetBool(animation, b);
    }
    #endregion

    #region Interface Required Methods
    public bool Equip(int weaponID) {
        if (m_IsAttacking) {
            return false;
        }

        m_WeaponID = weaponID;

        SetWeaponData();
        ApplyWeaponData();

        m_HasWeaponEquipped = true;

        UpdatePositionAndRotation();

        return true;
    }

    public void Unequip() {
        m_HasWeaponEquipped = false;
        m_WeaponSprite.sprite = null;
        m_WeaponAnimator.SetBool(m_WeaponData.AnimationBool, false);
        StopAllCoroutines();
    }

    public void Use() {
        m_IsInUse = true;
    }

    public void StopUse() {
        m_IsInUse = false;
    }
    #endregion

    #region Input Receivers
    private void OnAim(InputValue value) {
        Vector2 dir = value.Get<Vector2>();
        // Ensure aim was not just released (at (0, 0))
        if (dir.sqrMagnitude < Consts.ALMOST_ZERO_THRESHOLD) {
            return;
        }

        m_CurrentDirection = dir;
        
        if (m_WeaponID == Consts.NULL_ITEM_ID) {
            return;
        }

        UpdatePositionAndRotation();
    }

    // Executes attack functionality when player attack input is received
    private void OnAttack(InputValue value) {
        if (value.isPressed && m_IsInUse) {
            return;
        }
        else if (value.isPressed) {
            Use();
        }
        else {
            StopUse();
        }
    }
    #endregion

    #region Weapon Positioning
    private void UpdatePositionAndRotation() {
        float rotation = Mathf.Rad2Deg * Mathf.Atan2(m_CurrentDirection.y, m_CurrentDirection.x) - 90;

        m_Weapon.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
    #endregion

    #region On Attack Effect
    public void AddItemEffect(int itemID, OnAttackEffect effect) {
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
        #region Someone else decide on a name for this region
        public int ItemID;
        public OnAttackEffect Effect;
        #endregion

        #region Constructors
        public ItemAndEffect(int itemID, OnAttackEffect effect) {
            ItemID = itemID;
            Effect = effect;
        }
        #endregion
    }
    #endregion

    #region OnEnable
    private void OnEnable() {
        if (m_HasWeaponEquipped) {
            StopAllCoroutines();
            m_IsInUse = false;
            m_WeaponAnimator.ResetTrigger("Attack");
            m_WeaponAnimator.SetTrigger("Reset");
            m_WeaponAnimator.SetBool(m_WeaponData.AnimationBool, true);
        }
    }
    #endregion
}
