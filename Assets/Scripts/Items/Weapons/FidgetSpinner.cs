using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FidgetSpinner", menuName = "Items/Weapons/FidgetSpinner")]
public class FidgetSpinner : BaseWeaponItem
{

    #region Editor Variables
    [SerializeField]
    [Tooltip("Projectile for ranged weapons.")]
    private FidgeProjectile m_Projectile;

    [SerializeField]
    [Tooltip("Projectile Speed")]
    private float m_ProjectileSpeed;
    #endregion

    #region Attack methods

    public override void Fire(BaseWeaponItem weaponData)
    {
        float angle1 = Vector2.SignedAngle(Vector2.up, m_manager.GetAimDir());
        FidgeProjectile projectile1 = Instantiate(m_Projectile, m_manager.transform.position, Quaternion.Euler(0, 0, angle1));
        projectile1.Setup(m_Damage, m_manager.GetAimDir().normalized, m_ProjectileSpeed, m_manager, KnockbackPower);
    }

    public override string PlayAudio()
    {
        return null;
    }
    #endregion
}