using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scattershot", menuName = "Items/Weapons/Scattershot")]
public class Scattershot : BaseWeaponItem
{

    #region Editor Variables
    [SerializeField]
    [Tooltip("Projectile for ranged weapons.")]
    private ScatterBullet m_Projectile;

    [SerializeField]
    [Tooltip("Number of bounces for projectile")]
    private int m_MaxBounces;

    [SerializeField]
    [Tooltip("Projectile Speed")]
    private float m_ProjectileSpeed;
    #endregion

    #region Attack methods

    public override void Fire(BaseWeaponItem weaponData)
    {
        float angle1 = Vector2.SignedAngle(Vector2.up, m_manager.GetAimDir());
        ScatterBullet projectile1 = Instantiate(m_Projectile, m_manager.transform.position, Quaternion.Euler(0, 0, angle1));
        projectile1.Setup(m_Damage, m_manager.GetAimDir().normalized, m_ProjectileSpeed, m_manager, KnockbackPower, m_MaxBounces);

        float angle2 = angle1 + 15;
        Vector2 offsetAngle2 = Rotate(m_manager.GetAimDir(), 15).normalized;
        ScatterBullet projectile2 = Instantiate(m_Projectile, m_manager.transform.position, Quaternion.Euler(0, 0, angle2));
        projectile2.Setup(m_Damage, offsetAngle2, m_ProjectileSpeed, m_manager, KnockbackPower, m_MaxBounces);

        float angle3 = angle1 - 15;
        Vector2 offsetAngle3 = Rotate(m_manager.GetAimDir(), -15).normalized;
        ScatterBullet projectile3 = Instantiate(m_Projectile, m_manager.transform.position, Quaternion.Euler(0, 0, angle3));
        projectile3.Setup(m_Damage, offsetAngle3, m_ProjectileSpeed, m_manager, KnockbackPower, m_MaxBounces);
    }

    private Vector2 Rotate(Vector2 input, float offset) {
        float radians = offset * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = input.x;
        float ty = input.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public override string PlayAudio()
    {
        return "Scattershot Use";
    }
    #endregion
}