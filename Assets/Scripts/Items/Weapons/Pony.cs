using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pony", menuName = "Items/Weapons/Pony")]
public class Pony : BaseWeaponItem
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Charge gained per second.")]
    private float m_ChargeGainedPerSecond;

    [SerializeField]
    [Tooltip("Charge lost per second.")]
    private float m_ChargeLostPerSecond;

    [SerializeField]
    [Tooltip("Max charge for the greatsword.")]
    private float m_MaxCharge;

    [SerializeField]
    [Tooltip("Speed boost when in pony mode.")]
    private float m_SpeedMultiplier;

    [SerializeField]
    [Tooltip("Animation clip for pony.")]
    private string m_PonyClipBool;
    #endregion

    #region Private Variables
    private float m_CurrentCharge;
    private bool m_PonyMode;
    #endregion

    public override void RunEverySecond() {
        if (m_PonyMode) {
            m_CurrentCharge -= m_ChargeLostPerSecond;

            if (m_CurrentCharge <= 0) {
                ResetPony();
            }
        } else {
            m_CurrentCharge += m_ChargeGainedPerSecond;

            if (m_CurrentCharge > m_MaxCharge) {
                m_CurrentCharge = m_MaxCharge;
                m_PonyMode = true;

                m_manager.m_Weapon.SetAnimationBool(m_PonyClipBool, true);
                m_manager.GetPlayerData().AddXPercBaseSpeed(m_SpeedMultiplier);
            }
        }
    }

    public override void OnUnequip() {
        ResetPony();
    }

    private void ResetPony() {
        m_CurrentCharge = 0;
        m_PonyMode = false;

        m_manager.m_Weapon.SetAnimationBool(m_PonyClipBool, false);
        m_manager.GetPlayerData().SubtractXPercBaseSpeed(m_SpeedMultiplier);
    }
}
