using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heavy", menuName = "Items/Weapons/Heavy")]
public class Heavy : BaseWeaponItem {
    #region Initialization
    protected override void Awake() {
        m_WeaponType = WeaponTypes.HEAVY;

        m_NumRaycasts = 10;

        m_RightOffset = new Vector2(1f, -.2f);
        m_IdleAngleOffset = -90;

        m_AttackAngleOffset = -70;
        m_WindupPercent = .1f;
        m_AttackAnimationPercent = .3f;
        m_AnimationResetPercent = .3f;
    }
    #endregion
}
