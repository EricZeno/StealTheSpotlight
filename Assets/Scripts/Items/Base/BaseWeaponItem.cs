using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeaponItem : BaseItem {
    #region Editor Variables
    [Header("Properties")]

    [SerializeField]
    [Tooltip("The amount of damage this weapon deals.")]
    private int m_Damage;

    [SerializeField]
    [Tooltip("How much an enemy is pushed back when hit with this weapon.")]
    private float m_KnockbackPower;

    [SerializeField]
    [Tooltip("Name of the animation.")]
    private string m_AnimationName;

    [SerializeField]
    [Tooltip("Is it a ranged weapon?")]
    private bool m_IsRanged;

    [SerializeField]
    [Tooltip("Projectile for ranged weapons.")]
    private GameObject m_Projectile;
    #endregion

    #region Attack methods
    public abstract void Attack();
    #endregion

    #region Accessors
    public int Damage {
        get {
            return m_Damage;
        }
    }

    public float KnockbackPower {
        get {
            return m_KnockbackPower;
        }
    }

    public string AnimationBool {
        get {
            return m_AnimationName;
        }
    }

    public bool IsRanged {
        get {
            return m_IsRanged;
        }
    }

    public GameObject Projectile {
        get {
            return m_Projectile;
        }
    }
    #endregion
}
