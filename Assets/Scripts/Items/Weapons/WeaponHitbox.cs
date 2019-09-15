using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class WeaponHitbox : MonoBehaviour
{
    #region Private Variables
    private float p_Damage;
    private float p_KnockbackPower;

    private List<int> p_EnemiesHit;
    #endregion

    #region Initialization
    private void Awake() {
        p_EnemiesHit = new List<int>();
    }
    #endregion

    #region OnEnable and OnDisable
    private void OnDisable() {
        p_EnemiesHit.Clear();
    }
    #endregion

    #region Mutators
    public void SetHitboxStats(WeaponBaseData data) {
        SetDamage(data.GetDamage());
        SetKnockbackPower(data.GetKnockbackPower());
    }

    private void SetDamage(float damage) {
        if (damage >= 0) {
            p_Damage = damage;
        }
    }

    private void SetKnockbackPower(float knockbackPower) {
        if (knockbackPower >= 0) {
            p_KnockbackPower = knockbackPower;
        }
    }
    #endregion

    #region Collision Methods
    private void OnTriggerEnter2D(Collider2D other) {
        // if other.comparetag against consts.general enemy tag is true
            // get enemy ID
            // if enemy ID in enemies hit list, skip
            // get enemy ID and add it to enemies list
            // deal damage to enemy
            // knockback enemy in direction: enemy position - gameobject position
            // knockback should be normalized then multiplied by knockback multiplier
            // potentially take into account enemy knockback resistance
            // potentially perform knockback calculation inside enemy so resistance is
                // not part of something weapon hitbox worries about (abstraction)
    }
    #endregion
}
