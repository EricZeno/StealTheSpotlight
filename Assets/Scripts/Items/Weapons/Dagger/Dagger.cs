using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dagger", menuName = "Items/Weapons/Dagger")]
public class Dagger : BaseWeaponItem {
    #region Initialization
    protected override void Awake() {
        m_WeaponType = WeaponTypes.DAGGER;

        m_NumRaycasts = 20;

        m_RightOffset = new Vector2(1f, -.5f);
        m_IdleAngleOffset = -90;

        m_AttackAngleOffset = -70;
        m_WindupPercent = .1f;
        m_AttackAnimationPercent = .3f;
        m_AnimationResetPercent = .3f;
    }
    #endregion
}
