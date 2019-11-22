using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Polearm", menuName = "Items/Weapons/Polearm")]
public class Polearm : BaseWeaponItem {
    #region Initialization
    protected override void Awake() {
        m_WeaponType = WeaponTypes.POLEARM;

        m_NumRaycasts = 5;

        m_RightOffset = new Vector2(1f, -.6f);
        m_IdleAngleOffset = -90;

        m_AttackAngleOffset = 0;
        m_WindupPercent = .3f;
        m_AttackAnimationPercent = .2f;
        m_AnimationResetPercent = .2f;
    }
    #endregion
}
