using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeartyHeart", menuName = "Items/Passives/HeartyHeart")]
public class HeartyHeart : BasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much to multiply the player's health by.")]
    private float m_HealthMultiplier;
    #endregion
    
    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.GetPlayerData().AddXPercBaseHealth(m_HealthMultiplier);
    }

    public override void RemoveEffect(PlayerManager player) {
        player.GetPlayerData().SubtractXPercBaseHealth(m_HealthMultiplier);
    }
    #endregion
}
