using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Guandao", menuName = "Items/Weapons/Guandao")]
public class Guandao : BaseWeaponItem
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Max charge for shooting forwards.")]
    private float m_MaxCharge;

    [SerializeField]
    [Tooltip("Charging weapon animation bool.")]
    private string m_ChargeBool;
    #endregion

    #region Private Variables
    private float m_CurrentCharge;
    private bool m_Holding;
    #endregion

    public override void OnHold() {
        m_Holding = true;
        m_manager.m_Weapon.SetAnimationBool(m_ChargeBool, true);
    }

    public override void OnRelease() {
        if (m_Holding) {
            Release();
        }
    }

    public override void RunEverySecond() {
        if (m_Holding) {
            m_CurrentCharge += 300;

            if (m_CurrentCharge > m_MaxCharge) {
                Release();
            }
        }
    }

    private void Release() {
        m_manager.m_Weapon.SetAnimationBool(m_ChargeBool, false);

        Debug.Log(m_CurrentCharge);

        m_manager.GetComponent<PlayerMovement>().ApplyExternalForce(m_manager.GetAimDir() * m_CurrentCharge);

        m_CurrentCharge = 0;
        m_Holding = false;
    }

    public override string PlayAudio()
    {
        return "Light Hit 1";
    }
}
