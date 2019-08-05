using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WeaponBaseData {
    #region Private Variables
    private float p_Damage;

    private float p_KnockbackPower;

    private float p_AttackSpeed;

    private Sprite p_Sprite;
    #endregion

    #region Constructors
    public WeaponBaseData(float damage, float knockbackPower, float attackSpeed, Sprite sprite) {
        p_Damage = Mathf.Max(0, damage);
        p_KnockbackPower = Mathf.Max(0, knockbackPower);
        p_AttackSpeed = Mathf.Max(Consts.MINIMUM_ATTACK_SPEED, attackSpeed);
        p_Sprite = sprite;
    }
    #endregion

    #region Accessors and Mutators
    public float GetDamage() {
        return p_Damage;
    }

    public float GetKnockbackPower() {
        return p_KnockbackPower;
    }

    public float GetAttackSpeed() {
        return p_AttackSpeed;
    }

    public Sprite GetSprite() {
        return p_Sprite;
    }
    #endregion
}

public interface WeaponInterface {
    void Activate(WeaponBaseData data);
    void Deactivate();
    void UpdateGraphics(WeaponBaseData data);
}

[DisallowMultipleComponent]
public class WeaponBase : MonoBehaviour, WeaponInterface
{
    #region Cached Reference
    private SpriteRenderer cr_Renderer;

    private WeaponHitbox cr_Hitbox;
    #endregion

    #region Initialization
    private void Start() {
        cr_Renderer = GetComponentInChildren<SpriteRenderer>();
        if (cr_Renderer == null) {
            Debug.LogError("Could not find sprite renderer.");
        }

        cr_Hitbox = GetComponentInChildren<WeaponHitbox>();
        if (cr_Hitbox == null) {
            Debug.LogError("Could not find weapon collider.");
        }
    }
    #endregion

    #region Interface Required Methods
    public void Activate(WeaponBaseData data) {
        cr_Hitbox.gameObject.SetActive(true);
        cr_Hitbox.SetHitboxStats(data);
    }

    public void Deactivate() {
        cr_Hitbox.gameObject.SetActive(false);
    }

    public void UpdateGraphics(WeaponBaseData data) {
        cr_Renderer.sprite = data.GetSprite();
    }
    #endregion
}
