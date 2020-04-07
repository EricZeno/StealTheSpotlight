using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData : UnitStats {
    #region Points
    [SerializeField]
    [Tooltip("Name of enemy")]
    private int m_points;
    public int Points {
        get {
            return m_points;
        }
    }
    #endregion

    #region Attack Stats
    [SerializeField]
    [Tooltip("Number of attacks per second")]
    private float m_AttackSpeed;
    public float AttackSpeed {
        get {
            return m_AttackSpeed;
        }
    }
    [SerializeField]
    [Tooltip("Amount of damage done per attack")]
    private int m_Damage;
    public int Damage {
        get {
            return m_Damage;
        }
    }
    [SerializeField]
    [Tooltip("Radial distance from the center of the enemy (to the edge of a target's collider)")]
    private float m_AttackRange;
    public float AttackRange {
        get {
            return m_AttackRange;
        }
    }
    [SerializeField]
    [Tooltip("Angle of the cone of attack, -1 implies single target")]
    private float m_AttackCone;
    public float AttackCone {
        get {
            return m_AttackCone;
        }
    }
    #endregion
}
