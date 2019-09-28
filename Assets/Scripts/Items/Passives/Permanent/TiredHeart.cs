using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TiredHeart", menuName = "Items/Passives/TiredHeart")]
public class TiredHeart : BasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The percentage of health that will be healed up each iteration of the effect.")]
    private float m_HealPercent;

    [SerializeField]
    [Tooltip("How many seconds until the next heal.")]
    private float m_TimeToNextHeal;
    #endregion

    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.AddTimedEffect(GetID(), Heal, m_TimeToNextHeal);
    }

    public override void RemoveEffect(PlayerManager player) {
        player.SubtractTimedEffect(GetID());
    }
    #endregion

    #region Lasting Effect
    private void Heal(PlayerManager player) {
        player.Heal(m_HealPercent);
    }
    #endregion
}
