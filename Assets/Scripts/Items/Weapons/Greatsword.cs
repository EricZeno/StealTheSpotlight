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

    [SerializeField]
    [Tooltip("Attack anims, from low charge to high charge.")]
    private List<AnimationClip> m_AttackAnims;

    [SerializeField]
    [Tooltip("Idle anims, from low charge to charge charge.")]
    private List<AnimationClip> m_IdleAnims;
    #endregion

    #region Private Variables
    private float m_CurrentCharge;

    private int m_PrevThresholdIndex;
    private float m_PrevThreshold;
    private bool m_PassedThreshold;

    private bool m_KilledEnemy;
    #endregion

    public override void RunEverySecond() {
        m_CurrentCharge += m_ChargePerSecond;

        if (m_CurrentCharge >= m_PrevThreshold + m_MaxCharge / 3) {
            m_PrevThresholdIndex++;
            m_PassedThreshold = true;
            m_PrevThreshold += m_MaxCharge / 3;

            if (m_PrevThresholdIndex > m_AttackAnims.Count - 1) {
                m_PrevThresholdIndex = m_AttackAnims.Count - 1;
            }
        }

        if (m_PassedThreshold) {
            OverrideAnimations();

            m_PassedThreshold = false;
        }

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

        m_PrevThreshold = 0;
        m_PrevThresholdIndex = 0;
        m_PassedThreshold = false;
        OverrideAnimations();
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

    #region Animations
    private void OverrideAnimations() {
        AnimatorOverrideController aoc = new AnimatorOverrideController(m_manager.m_Weapon.GetWeaponAnimator().runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (AnimationClip clip in aoc.animationClips) {
            ReplaceAnimation(clip, anims);
        }

        aoc.ApplyOverrides(anims);
        m_manager.m_Weapon.GetWeaponAnimator().runtimeAnimatorController = aoc;
    }

    private void ReplaceAnimation(AnimationClip oldClip, List<KeyValuePair<AnimationClip, AnimationClip>> anims) {
        if (!oldClip.name.Contains("heat_sword")) {
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldClip, oldClip));
            return;
        }

        AnimationClip newClip = oldClip;

        if (oldClip.name.Contains("idle")) {
            newClip = m_IdleAnims[m_PrevThresholdIndex];
        }
        else if (oldClip.name.Contains("swing")) {
            newClip = m_AttackAnims[m_PrevThresholdIndex];
        }

        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(oldClip, newClip));
    }
    #endregion
}
