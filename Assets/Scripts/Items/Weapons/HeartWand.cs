using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeartWand", menuName = "Items/Weapons/HeartWand")]
public class HeartWand : BaseWeaponItem
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Projectile for ranged weapons.")]
    private HeartProjectile m_Projectile;

    [SerializeField]
    [Tooltip("Projectile Speed")]
    private float m_ProjectileSpeed;

    [SerializeField]
    [Tooltip("Explosion Radius")]
    private float m_ExplodeRadius;

    [SerializeField]
    [Tooltip("How much the death explosion damage will do")]
    private int m_ExplosionDamage;

    [SerializeField]
    [Tooltip("Chance to BOOM")]
    [Range(0.0f, 1.0f)]
    private float m_ChanceHuge;

    [SerializeField]
    [Tooltip("How much to upscale explosion size/damage. Damage is multiplied by double the scale")]
    [Range(1.0f, 4.0f)]
    private float m_Scale;

    private Vector3 m_SpherePosition;

    private BaseWeaponItem m_WeaponData;
    #endregion

    #region Attack methods
    public override void Fire(BaseWeaponItem weaponData)
    {
        bool huge = (Random.value <= m_ChanceHuge);
        m_WeaponData = weaponData;
        float angle1 = Vector2.SignedAngle(Vector2.up, m_manager.GetAimDir());
        HeartProjectile projectile = Instantiate(m_Projectile, m_manager.transform.position, Quaternion.Euler(0, 0, angle1));
        if (huge)
        {
            projectile.transform.localScale = projectile.transform.localScale * 3;
            projectile.Setup(m_Damage, m_manager.GetAimDir().normalized, m_ProjectileSpeed, m_manager, KnockbackPower, weaponData, m_Scale, m_ExplodeRadius, true);
        }
        else {
            projectile.Setup(m_Damage, m_manager.GetAimDir().normalized, m_ProjectileSpeed, m_manager, KnockbackPower, weaponData, 1, m_ExplodeRadius, false);
        }
        
    }

    public override void EnemyExplode(Vector3 position) {
        m_SpherePosition = position;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, m_ExplodeRadius);

        foreach (Collider2D other in hitColliders) {
            
            if (other.CompareTag(Consts.GENERAL_ENEMY_TAG)) {

                EnemyManager enemyManager = other.GetComponent<EnemyManager>();
                enemyManager.TakeDamage(m_WeaponData, m_ExplosionDamage, m_manager.GetID());
            }
            else if (other.CompareTag(Consts.PLAYER_TAG))
            {
                other.GetComponent<PlayerManager>().TakeDamage(m_ExplosionDamage);
            }

            if (other.CompareTag(Consts.BUSH_PHYSICS_LAYER))
            {
                Destroy(other.gameObject);
            }

            if (other.CompareTag(Consts.POT_PHYSICS_LAYER))
            {
                Destroy(other.gameObject);
            }
        }

    }

    public override string PlayAudio()
    {
        return "Wand use";
    }
    #endregion
}
