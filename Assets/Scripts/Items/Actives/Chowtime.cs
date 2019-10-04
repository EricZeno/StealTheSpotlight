using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chowtime", menuName = "Items/Actives/Chowtime")]
public class Chowtime : BaseActiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How long the effect should last for.")]
    private float m_EffectLength;

    [SerializeField]
    [Tooltip("How much to multiply the damage by.")]
    private float m_DamageMultiplier;
    #endregion

    #region Interface Methods
    public override void UseEffect(PlayerManager player) {
        player.AddOnAttackEffectForXSec(GetID(), MultiplyAttackDamage, 
            m_EffectLength);
    }

    public override void StopEffect() { }

    public override void CancelEffect() { }

    protected override void AimGFX() { }
    #endregion

    #region Effect Methods
    private void MultiplyAttackDamage(WeaponBaseData originalData, ref WeaponBaseData newData) {
        newData.SetDamage(originalData.GetDamage() * m_DamageMultiplier);
    }
    #endregion
}
