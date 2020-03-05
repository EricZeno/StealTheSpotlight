using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Greatsword", menuName = "Items/Weapons/Greatsword")]
public class Greatsword : BaseWeaponItem {

    #region Editor Variables
    [SerializeField]
    [Tooltip("Charge gained per second.")]
    private float m_ChargePerSecond;

    [SerializeField]
    [Tooltip("Max charge for the greatsword.")]
    private float m_MaxCharge;
    #endregion

    #region Private Variables
    private float m_CurrentCharge;

    private bool m_KilledEnemy;
    #endregion

    public override void RunEverySecond() {
        m_CurrentCharge += m_ChargePerSecond;

        if (m_CurrentCharge > m_MaxCharge) {
            m_CurrentCharge = m_MaxCharge;
        }

    }

    #region Attack methods
    public override void Attack() {
        if (m_KilledEnemy) {
            m_KilledEnemy = false;
            return;
        }

        m_CurrentCharge = 0;
    }

    public override void OnKillEnemy() {
        m_KilledEnemy = true;
    }

    public override int Damage {
        get {
            return m_Damage + (int)m_CurrentCharge;
        }
    }
    #endregion
}
