using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AngrySun", menuName = "Items/Passives/AngrySun")]
public class AngrySun : BasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much to multiply the base knockback by when landing an attack.")]
    private float m_KnockbackMultiplier;
    #endregion

    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.AddOnAttackEffect(GetID(), IncreaseKnockback);
    }

    public override void RemoveEffect(PlayerManager player) {
        player.SubtractOnAttackEffect(GetID());
    }
    #endregion

    #region Effect Functions
    private void IncreaseKnockback(WeaponBaseData originalData, WeaponBaseData newData) {
        float originalPower = originalData.GetKnockbackPower();
        newData.SetKnockbackPower(originalPower * m_KnockbackMultiplier + originalPower);
    }
    #endregion
}
