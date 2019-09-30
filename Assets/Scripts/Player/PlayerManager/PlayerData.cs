using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : UnitStats {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The interaction radius when a player presses the interact button.")]
    private float m_InteractionRadius;
    #endregion

    #region Resetters
    public override void ResetAllStatsDefault() {
        base.ResetAllStatsDefault();
        p_Type = EUnitType.Player;
    }
    #endregion

    #region Accessors
    public float GetInteractionRadius() {
        return m_InteractionRadius;
    }
    #endregion
}
