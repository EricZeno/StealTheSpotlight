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
    private float m_CooldownTimer;

    private Vector2 m_CurrentDirection;
    private Vector2 m_AttackDirection;

    private List<ItemAndEffect> m_OnAttackEffects;

    private GameObject[] m_HitEnemies;
    #endregion

    #region Cached Components
    private PlayerManager m_Manager;
    private AudioManager m_Audio;
    #endregion

    #region Cached References
    private GameObject m_Weapon;
    private SpriteRenderer m_WeaponSprite;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_WeaponID = Consts.NULL_ITEM_ID;
        m_WeaponData = null;

        m_HasWeaponEquipped = false;
        m_IsInUse = false;
        m_IsAttacking = false;
        m_CooldownTimer = 0;

        m_CurrentDirection = Vector2.right;

        m_OnAttackEffects = new List<ItemAndEffect>();

        m_HitEnemies = null;

        m_Manager = GetComponent<PlayerManager>();
        m_Audio = GetComponent<AudioManager>();
    }

    private void Start() {
        m_Weapon = transform.Find(Consts.WEAPON_OBJECT_NAME).gameObject;
        if (m_Weapon == null) {
            Debug.LogError("Could not find weapon.");
        }
        m_WeaponSprite = m_Weapon.GetComponent<SpriteRenderer>();
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

    #region Accessors
    public bool IsAttacking {
        get {
            return m_IsAttacking;
        }
    }
    #endregion

    #region Resetters
    private void ResetCooldown() {
        m_CooldownTimer = 1 / m_WeaponData.AttackSpeed;
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
        if (m_CooldownTimer > 0) {
            return false;
        }

        return true;
    }

    private bool WaitingOnCooldown() {
        return m_CooldownTimer > 0;
    }
    #endregion

    #region Equip Helper Methods
    private void SetWeaponData() {
        m_WeaponData = ItemManager.GetWeaponItem(m_WeaponID);

        m_HitEnemies = new GameObject[m_WeaponData.NumRaycasts];
        ResetHitEnemies();
    }

    private void ApplyWeaponData() {
        m_WeaponSprite.sprite = m_WeaponData.GetIcon();
    }
    #endregion

    #region Use Methods
    private IEnumerator UseWeapon() {
        m_IsAttacking = true;

        float totalAttackTime = 1 / m_WeaponData.AttackSpeed;
        ResetCooldown();

        if (m_WeaponData.HasArcAttack) {
            StartCoroutine(AnimateWeaponArc());
        }
        else {
            StartCoroutine(AnimateWeaponStab());
        }
        yield return new WaitForSeconds(totalAttackTime * m_WeaponData.WindupPercent);

        StartCoroutine(Attack());
    }

    private IEnumerator Attack() {
        // Find starting and ending angle
        float dirAngle = Vector2.SignedAngle(Vector2.right, m_CurrentDirection); // in DEGREES
        float startingAngle = (dirAngle - m_WeaponData.ArcHalfAngle) * Mathf.Deg2Rad;
        float endingAngle = (dirAngle + m_WeaponData.ArcHalfAngle) * Mathf.Deg2Rad;

        //Raycast information
        float range = m_WeaponData.AttackRange;
        float currentAngle = startingAngle;
        LayerMask layerMask = GameManager.GetSingleton().HittableLayers;
        layerMask ^= (1 << gameObject.layer);

        int numCasts = m_WeaponData.NumRaycasts;
        float angleIncrement = (endingAngle - startingAngle) / numCasts;
        float timeBtwnRays = (1 / m_WeaponData.AttackSpeed);
        timeBtwnRays *= m_WeaponData.AttackAnimationPercent;
        timeBtwnRays /= numCasts;

        for (int i = 0; i < numCasts; i++) {
            Vector2 dir = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, range, layerMask);
            foreach (var hit in hits) {
                if (hit.collider.CompareTag(Consts.PLAYER_TAG) || hit.collider.CompareTag(Consts.GENERAL_ENEMY_TAG)) {
                    HitEnemy(hit.collider.gameObject);
                }
            }

            yield return new WaitForSeconds(timeBtwnRays);
            currentAngle += angleIncrement;
        }

        ResetHitEnemies();
    }

    private void HitEnemy(GameObject enemy) {
        for (int i = 0; i < m_HitEnemies.Length; i++) {
            if (m_HitEnemies[i] == enemy) {
                return;
            }
            if (m_HitEnemies[i] == null) {
                m_HitEnemies[i] = enemy;
                break;
            }
        }

        m_Audio.Play("Light Hit 1");

        WeaponBaseData data = new WeaponBaseData(
            m_Manager.GetPlayerData().GetPower() + m_WeaponData.Damage,
            m_WeaponData.KnockbackPower);
        data = ApplyOnAttackEffects(data);
        if (enemy.CompareTag(Consts.PLAYER_TAG)) {
            PlayerManager other = enemy.GetComponent<PlayerManager>();
            other.TakeDamage(data.Damage, m_Manager);
            Vector2 dir = (other.transform.position - transform.position).normalized;
            other.GetComponent<PlayerMovement>().ApplyExternalForce(dir * data.KnockbackPower);
        }
        else if (enemy.CompareTag(Consts.GENERAL_ENEMY_TAG)) {
            EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
            enemyManager.TakeDamage(data.Damage, m_Manager.GetID());
            Vector2 dir = (enemyManager.transform.position - transform.position).normalized;
            enemyManager.GetComponent<EnemyMovement>().ApplyExternalForce(dir * data.KnockbackPower);
        }
    }

    private void ResetHitEnemies() {
        for (int i = 0; i < m_HitEnemies.Length; i++) {
            m_HitEnemies[i] = null;
        }
    }

    private WeaponBaseData ApplyOnAttackEffects(WeaponBaseData origValues) {
        WeaponBaseData result = origValues;
        foreach (var itemAndEffect in m_OnAttackEffects) {
            result = itemAndEffect.Effect(result);
        }
        return result;
    }
    #endregion

    #region Animating Weapon
    private IEnumerator AnimateWeaponArc() {
        float totalAttackTime = 1 / m_WeaponData.AttackSpeed;

        float dirAngle = Vector2.SignedAngle(Vector2.right, m_CurrentDirection); // in DEGREES
        float theta = Vector2.SignedAngle(Vector2.right, m_WeaponData.RightOffset);

        // The wind up animation
        float time = totalAttackTime * m_WeaponData.WindupPercent;
        float startAnimationAngle = dirAngle + theta;
        float startAttackAnimationAngle = -m_WeaponData.ArcHalfAngle + dirAngle;
        StartCoroutine(AnimateWeaponMoveArc(
            startAnimationAngle,
            startAttackAnimationAngle,
            time));
        yield return new WaitForSeconds(time);

        // The attack animation
        time = totalAttackTime * m_WeaponData.AttackAnimationPercent;
        float endAttackAnimationAngle = m_WeaponData.ArcHalfAngle + dirAngle;
        StartCoroutine(AnimateWeaponMoveArc(
            startAttackAnimationAngle,
            endAttackAnimationAngle,
            time));
        yield return new WaitForSeconds(time);

        // Resetting position animation
        time = totalAttackTime * m_WeaponData.AnimationResetPercent;
        StartCoroutine(AnimateWeaponMoveArc(
            endAttackAnimationAngle,
            startAnimationAngle,
            time));
        yield return new WaitForSeconds(time);

        m_IsAttacking = false;
    }

    private IEnumerator AnimateWeaponMoveArc(float startingAngle, float endingAngle, float time) {
        float currentAngle = startingAngle;

        int dir = (endingAngle > currentAngle) ? 1 : -1;
        float speed = dir * (endingAngle - currentAngle) / time;

        while (dir * (endingAngle - currentAngle) > 0) {
            UpdatePositionAndRotation(currentAngle, false);
            yield return null;
            currentAngle += dir * speed * Time.deltaTime;
        }
    }

    private IEnumerator AnimateWeaponStab() {
        m_AttackDirection = m_CurrentDirection;
        float theta = Vector2.SignedAngle(Vector2.right, m_AttackDirection); // in DEGREES
        RotateWeapon(theta, true);
        float totalAttackTime = 1 / m_WeaponData.AttackSpeed;

        // The wind up animation
        float time = totalAttackTime * m_WeaponData.WindupPercent;
        Vector2 startAnimationPosition = m_WeaponData.RightOffset;
        Vector2 startAttackAnimationPosition = m_WeaponData.AttackStartPosition;
        StartCoroutine(AnimateWeaponMoveStab(
            startAnimationPosition,
            startAttackAnimationPosition,
            time));
        yield return new WaitForSeconds(time);

        // The attack animation
        time = totalAttackTime * m_WeaponData.AttackAnimationPercent;
        Vector2 endAttackAnimationPosition = m_WeaponData.AttackFinalPosition;
        StartCoroutine(AnimateWeaponMoveStab(
            startAttackAnimationPosition,
            endAttackAnimationPosition,
            time));
        yield return new WaitForSeconds(time);

        // Resetting position animation
        time = totalAttackTime * m_WeaponData.AnimationResetPercent;
        StartCoroutine(AnimateWeaponMoveStab(
            endAttackAnimationPosition,
            startAnimationPosition,
            time));
        yield return new WaitForSeconds(time);

        m_IsAttacking = false;
    }

    private IEnumerator AnimateWeaponMoveStab(Vector2 startingPosition, Vector2 endingPosition, float time) {
        float currentTime = 0;

        while (currentTime < time) {
            RepositionWeapon(Vector2.Lerp(startingPosition, endingPosition, currentTime / time));
            yield return null;
            currentTime += Time.deltaTime;
        }
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

        if (m_IsAttacking) {
            return;
        }
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
    private void UpdatePositionAndRotation(bool isIdle = true) {
        float theta = Vector2.SignedAngle(Vector2.right, m_CurrentDirection); // in DEGREES
        UpdatePositionAndRotation(theta, isIdle);
    }

    private void UpdatePositionAndRotation(float theta, bool isIdle) {
        RotateWeapon(theta, isIdle);
        RepositionWeapon(theta, isIdle);
    }

    private void RotateWeapon(float theta, bool isIdle) {
        float angleOffset = 0;
        if (isIdle) {
            angleOffset = ItemManager.GetWeaponItem(m_WeaponID).IdleAngleOffset;
        }
        else {
            angleOffset = ItemManager.GetWeaponItem(m_WeaponID).AttackAngleOffset;
        }
        m_Weapon.transform.rotation = Quaternion.Euler(0, 0, theta + angleOffset);
    }

    private void RepositionWeapon(float theta, bool isIdle) {
        // At the correct relative position given weapon type and aim direction
        Vector2 offset = Vector2.right;
        if (isIdle) {
            offset = ItemManager.GetWeaponItem(m_WeaponID).RightOffset;
        }
        else {
            offset *= ItemManager.GetWeaponItem(m_WeaponID).RightOffset.magnitude;
        }
        float oldX = offset.x;
        float oldY = offset.y;

        theta *= Mathf.Deg2Rad; // in RADIANS
        float newX = oldX * Mathf.Cos(theta) - oldY * Mathf.Sin(theta);
        float newY = oldX * Mathf.Sin(theta) + oldY * Mathf.Cos(theta);
        m_Weapon.transform.localPosition = new Vector2(newX, newY);
    }

    private void RepositionWeapon(Vector2 position) {
        float theta = Vector2.SignedAngle(Vector2.right, m_AttackDirection); // in DEGREES
        theta *= Mathf.Deg2Rad;
        float newX = position.x * Mathf.Cos(theta) - position.y * Mathf.Sin(theta);
        float newY = position.x * Mathf.Sin(theta) + position.y * Mathf.Cos(theta);
        m_Weapon.transform.localPosition = new Vector2(newX, newY);
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
}
