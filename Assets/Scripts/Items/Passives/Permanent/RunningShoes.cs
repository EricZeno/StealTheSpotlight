using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunningShoes", menuName = "Items/Passives/RunningShoes")]
public class RunningShoes : BasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Range(0, 50)]
    [Tooltip("How much faster the player gets while wearing these shoes. A one will yield no effect." +
        "Less than one makes you slower and more than one makes you faster where two is two times faster.")]
    private float m_MoveSpeedBoost;
    #endregion

    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.GetPlayerData().AddXPercBaseSpeed(m_MoveSpeedBoost);
    }

    public override void RemoveEffect(PlayerManager player) {
        player.GetPlayerData().SubtractXPercBaseSpeed(m_MoveSpeedBoost);
    }
    #endregion
}
