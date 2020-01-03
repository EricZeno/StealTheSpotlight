using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoldenFlask", menuName = "Items/Passives/GoldenFlask")]
public class GoldenFlask : BasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("Amount of health that will be healed up each iteration of the effect.")]
    private int m_HealAmount;

    [SerializeField]
    [Tooltip("How many seconds until the next heal.")]
    private float m_TimeToNextHeal;

    [SerializeField]
    [Tooltip("Effect will only activate when health is below this percentage.")]
    private float m_ActivationPercent;
    #endregion

    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.AddTimedEffect(GetID(), Heal, m_TimeToNextHeal);
    }

    public override void RemoveEffect(PlayerManager player) {
        player.RemoveTimedEffect(GetID());
    }
    #endregion

    #region Lasting Effect
    private void Heal(PlayerManager player) {
        if (player.GetPlayerData().CurrHealth * 1.0f / player.GetPlayerData().MaxHealth < m_ActivationPercent) {
            player.Heal(m_HealAmount);
        }
    }
    #endregion
}
