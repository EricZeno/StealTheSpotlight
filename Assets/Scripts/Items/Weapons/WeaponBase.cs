using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WeaponBaseData {
    #region Private Variables
    private float m_Damage;

    private float m_KnockbackPower;

    private float m_AttackSpeed;

    private Sprite m_Sprite;
    #endregion

    #region Constructors
    public WeaponBaseData(float damage, float knockbackPower, float attackSpeed, Sprite sprite) {
        m_Damage = Mathf.Max(0, damage);
        m_KnockbackPower = Mathf.Max(0, knockbackPower);
        m_AttackSpeed = Mathf.Max(Consts.MINIMUM_ATTACK_SPEED, attackSpeed);
        m_Sprite = sprite;
    }

    public WeaponBaseData(WeaponBaseData data) {
        m_Damage = data.m_Damage;
        m_KnockbackPower = data.m_KnockbackPower;
        m_AttackSpeed = data.m_AttackSpeed;
        m_Sprite = data.m_Sprite;
    }
    #endregion

    #region Accessors and Mutators
    public float GetDamage() {
        return m_Damage;
    }

    public void SetDamage(float newDamage) {
        m_Damage = newDamage;
    }

    public float GetKnockbackPower() {
        return m_KnockbackPower;
    }
    public void SetKnockbackPower(float newPower) {
        m_KnockbackPower = newPower;
    }

    public float GetAttackSpeed() {
        return m_AttackSpeed;
    }

    public Sprite GetSprite() {
        return m_Sprite;
    }
    #endregion
}

public interface WeaponInterface {
    void Activate(WeaponBaseData data, WeaponBase.OnAttackEffect[] effects);
    void Deactivate();
    void UpdateGraphics(WeaponBaseData data);
}

[DisallowMultipleComponent]
public class WeaponBase : MonoBehaviour, WeaponInterface
{
    #region Delegates
    public delegate void OnAttackEffect(WeaponBaseData originalData, ref WeaponBaseData newData);
    #endregion

    #region Cached Reference
    private SpriteRenderer m_Renderer;

    private WeaponHitbox m_Hitbox;
    #endregion

    #region Initialization
    private void Start() {
        m_Renderer = GetComponentInChildren<SpriteRenderer>();
        if (m_Renderer == null) {
            Debug.LogError("Could not find sprite renderer.");
        }

        m_Hitbox = GetComponentInChildren<WeaponHitbox>();
        if (m_Hitbox == null) {
            Debug.LogError("Could not find weapon collider.");
        }
    }
    #endregion

    #region Interface Required Methods
    public void Activate(WeaponBaseData originalData, OnAttackEffect[] effects) {
        WeaponBaseData postEffectsData = ApplyAllOnAttackEffects(originalData, effects);

        m_Hitbox.gameObject.SetActive(true);
        m_Hitbox.SetHitboxStats(postEffectsData);
    }

    public void Deactivate() {
        m_Hitbox.gameObject.SetActive(false);
    }

    public void UpdateGraphics(WeaponBaseData data) {
        m_Renderer.sprite = data.GetSprite();
    }
    #endregion

    #region On Attack Effects
    private WeaponBaseData ApplyAllOnAttackEffects(WeaponBaseData originalData, OnAttackEffect[] effects) {
        WeaponBaseData postEffectsData = new WeaponBaseData(originalData);
        foreach (var effect in effects) {
            effect(originalData, ref postEffectsData);
        }
        return postEffectsData;
    }
    #endregion
}
