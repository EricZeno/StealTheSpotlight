using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponTypes {
    DAGGER,
    HEAVY
}

public abstract class BaseWeaponItem : BaseItem {
    #region Variables
    #region Editor Variables
    [Header("Properties")]

    [SerializeField]
    [Tooltip("The amount of damage this weapon deals.")]
    private int m_Damage;
    
    [SerializeField]
    [Tooltip("How many times this weapon attacks every second.")]
    private float m_AttackSpeed;

    [SerializeField]
    [Tooltip("How far the weapon reaches.")]
    private float m_AttackRange;

    [SerializeField]
    [Tooltip("Half of the total arc angle that the weapon will move across.")]
    private float m_ArcHalfAngle;

    [SerializeField]
    [Tooltip("How much an enemy is pushed back when hit with this weapon.")]
    private float m_KnockbackPower;
    #endregion

    #region Private Variables
    protected WeaponTypes m_WeaponType;

    protected int m_NumRaycasts; // Melee only

    #region Idle Positioning
    protected Vector2 m_RightOffset;
    protected float m_IdleAngleOffset;
    #endregion

    #region Attack Animation
    protected float m_AttackAngleOffset;

    protected float m_WindupPercent;
    protected float m_AttackAnimationPercent;
    protected float m_AnimationResetPercent;
    #endregion
    #endregion
    #endregion

    #region Initialization
    protected abstract void Awake();
    #endregion

    #region Accessors
    public WeaponTypes WeaponType {
        get {
            return m_WeaponType;
        }
    }

    public int NumRaycasts {
        get {
            return m_NumRaycasts;
        }
    }

    #region Properties
    public int Damage {
        get {
            return m_Damage;
        }
    }

    public float AttackSpeed {
        get {
            return m_AttackSpeed;
        }
    }

    public float AttackRange {
        get {
            return m_AttackRange;
        }
    }

    public float ArcHalfAngle {
        get {
            return m_ArcHalfAngle;
        }
    }

    public float KnockbackPower {
        get {
            return m_KnockbackPower;
        }
    }
    #endregion

    #region Idle Positioning
    public Vector2 RightOffset {
        get {
            return m_RightOffset;
        }
    }

    public float IdleAngleOffset {
        get {
            return m_IdleAngleOffset;
        }
    }
    #endregion

    #region Attack Animation 
    public float AttackAngleOffset {
        get {
            return m_AttackAngleOffset;
        }
    }

    public float WindupPercent {
        get {
            return m_WindupPercent;
        }
    }

    public float AttackAnimationPercent {
        get {
            return m_AttackAnimationPercent;
        }
    }

    public float AnimationResetPercent {
        get {
            return m_AnimationResetPercent;
        }
    }
    #endregion
    #endregion
}
