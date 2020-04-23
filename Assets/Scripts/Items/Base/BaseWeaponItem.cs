using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeaponItem : BaseItem {
    #region Editor Variables
    [Header("Properties")]

    [SerializeField]
    [Tooltip("The amount of damage this weapon deals.")]
    protected int m_Damage;

    [SerializeField]
    [Tooltip("How much an enemy is pushed back when hit with this weapon.")]
    private float m_KnockbackPower;

    [SerializeField]
    [Tooltip("Name of the animation.")]
    private string m_AnimationName;

    [SerializeField]
    [Tooltip("Is it a ranged weapon?")]
    private bool m_IsRanged;

    #endregion

    protected PlayerManager m_manager;

    #region Attack methods
    public virtual void Attack() { }
    public virtual void RunEverySecond() { }

    public virtual void Fire(BaseWeaponItem weaponData) { }

    public virtual void OnKillEnemy() { }

    public virtual void EnemyExplode(Vector3 position) { }
    public virtual void OnHold() { }
    public virtual void OnRelease() { }

    public virtual string PlayAudio() { return null; }
    #endregion

    #region Accessors
    public virtual PlayerManager Manager {
        set {
            m_manager = value;
        }
    }

    public virtual int Damage {
        get {
            return m_Damage;
        }
    }

    public float KnockbackPower {
        get {
            return m_KnockbackPower;
        }
    }

    public virtual string AnimationBool {
        get {
            return m_AnimationName;
        }
    }

    public bool IsRanged {
        get {
            return m_IsRanged;
        }
    }


    #endregion
}
