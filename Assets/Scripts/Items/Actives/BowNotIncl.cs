using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BowNotIncl", menuName = "Items/Actives/BowNotIncl")]
public class BowNotIncl : BaseActiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How fast the projectile travels.")]
    private float m_ProjectileSpeed;

    [SerializeField]
    [Tooltip("The dimensions of the box collider.")]
    private Vector2 m_BoxColliderDimensions;

    [SerializeField]
    [Tooltip("The prefab for the actual projectile to be fired.")]
    private Projectile m_ProjectilePrefab;
    #endregion

    #region Interface Methods
    public override void UseEffect(PlayerManager player) {
        float angle = Vector2.SignedAngle(Vector2.up, player.GetAimDir());
        Projectile projectile = Instantiate(m_ProjectilePrefab, 
            player.transform.position, Quaternion.Euler(0, 0, angle));
        projectile.Setup(player.GetPlayerData().GetPower(), player.GetAimDir().normalized,
            m_ProjectileSpeed, GetIcon(), m_BoxColliderDimensions);
    }
    
    public override void StopEffect() { }

    public override void CancelEffect() { }

    protected override void AimGFX() { }
    #endregion
}
