using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlotArmor", menuName = "Items/Passives/PlotArmor")]
public class PlotArmor : BasePassiveItem {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("Only applies multipliers when the player is beneath or at this health percentage.")]
    private float m_ActivationPercent;

    [SerializeField]
    [Tooltip("How much faster the player gets.")]
    private float m_SpeedMultiplier;

    [SerializeField]
    [Tooltip("How much more damage the player does.")]
    private float m_AttackMultiplier;
    #endregion

    #region Private Variables
    private bool m_addedMultiplier = false;
    private float m_updateTimer = .1f;
    #endregion
    #endregion

    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.AddTimedEffect(GetID(), CheckAndAddMultipliers, m_updateTimer);
    }

    public override void RemoveEffect(PlayerManager player) {
        player.RemoveTimedEffect(GetID());
        if (m_addedMultiplier) {
            RemoveMultipliers(player);
        }
    }
    #endregion

    #region helper functions
    private void CheckAndAddMultipliers(PlayerManager player) {
        bool shouldApplyEffect = player.GetPlayerData().CurrHealth * 1.0f / player.GetPlayerData().MaxHealth <= m_ActivationPercent;
        if (m_addedMultiplier && !shouldApplyEffect) {
            RemoveMultipliers(player);
            m_addedMultiplier = false;
        } else if (!m_addedMultiplier && shouldApplyEffect) {
            AddMultipliers(player);
            m_addedMultiplier = true;
        }
    }

    private void AddMultipliers(PlayerManager player) {
        player.GetPlayerData().AddXPercBaseSpeed(m_SpeedMultiplier);
        player.GetPlayerData().AddXPercBasePower(m_AttackMultiplier);
    }

    private void RemoveMultipliers(PlayerManager player) {
        player.GetPlayerData().SubtractXPercBaseSpeed(m_SpeedMultiplier);
        player.GetPlayerData().SubtractXPercBasePower(m_AttackMultiplier);
    }
    #endregion
}
