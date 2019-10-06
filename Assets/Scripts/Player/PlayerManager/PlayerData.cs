using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : UnitStats {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The interaction radius when a player presses the interact button.")]
    private float m_InteractionRadius;

    [SerializeField]
    [Tooltip("The amount of time it takes a player to respawn after dying.")]
    private int m_RespawnTime;
    #endregion

    #region Resetters
    public override void ResetAllStatsDefault() {
        base.ResetAllStatsDefault();
        m_Type = EUnitType.Player;
    }
    #endregion

    #region Accessors
    public float GetInteractionRadius() {
        return m_InteractionRadius;
    }

    public int RespawnTime {
        get { return m_RespawnTime; }
        set { m_RespawnTime = value; }
    }
    #endregion
}
