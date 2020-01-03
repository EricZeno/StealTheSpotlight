using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MythrilBell", menuName = "Items/Passives/MythrilBell")]
public class MythrilBell : BasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("Amount of health that will be added to MaxHealth.")]
    private int m_MaxHealthIncrease;
    #endregion

    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.GetPlayerData().AddXMaxHealth(m_MaxHealthIncrease);
        player.GetPlayerData().AddStatusImmunity(Status.Poison);
    }

    public override void RemoveEffect(PlayerManager player) {
        player.GetPlayerData().SubtractXMaxHealth(m_MaxHealthIncrease);
        player.GetPlayerData().RemoveStatusImmunity(Status.Poison);
    }
    #endregion
}
