using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDamageToPlayer : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The player to apply damage to.")]
    private PlayerManager m_Player;

    [SerializeField]
    [Tooltip("The amount of damage to apply")]
    private int m_Damage;
    #endregion

    #region Apply Damage Methods
    public void ApplyDamage() {
        m_Player.TakeDamage(m_Damage);
    }
    #endregion
}
