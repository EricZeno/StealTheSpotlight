using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponTypes {
    DAGGER,
    POLEARM,
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
    [Tooltip("Whether this weapon's animation is an arc attack or a stab attack.")]
    private bool m_HasArcAttack;

    [SerializeField]
    [Tooltip("The arc angle that the weapon will move across if it arcs. This is used " +
        "everywhere by the weapons to decide the range that they hit.")]
    private float m_ArcAngle;

    [SerializeField]
    [Tooltip("The starting position of where the weapon will be if it stabs. During the wind " +
        "up, the weapon moves here before the actual attack.")]
    private Vector2 m_AttackStartPosition;

    [SerializeField]
    [Tooltip("The final position of where the the weapon will be if it stabs.")]
    private Vector2 m_AttackFinalPosition;

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

    public bool HasArcAttack {
        get {
            return m_HasArcAttack;
        }
    }

    public float ArcHalfAngle {
        get {
            return m_ArcAngle / 2;
        }
    }

    public Vector2 AttackStartPosition {
        get {
            return m_AttackStartPosition;
        }
    }

    public Vector2 AttackFinalPosition {
        get {
            return m_AttackFinalPosition;
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
