using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAttack : MonoBehaviour {
    #region Variables
    #region Private Variables
    protected bool m_CanAttack;
    public bool CanAttack {
        set {
            m_CanAttack = value;
        }
    }
    #endregion

    #region Cached Components
    protected EnemyManager m_Manager;
    protected EnemyMovement m_Movement;
    protected EnemyGraphics m_Graphics;
    #endregion
    #endregion

    #region Initialization
    protected virtual void Awake() {
        m_Manager = GetComponentInParent<EnemyManager>();
        m_Movement = GetComponentInParent<EnemyMovement>();
        m_Graphics = GetComponentInParent<EnemyGraphics>();
    }
    #endregion

    #region External Attack Call
    public virtual void Attack(int attackNum, Vector2 target) {
        // Use this function to trigger an enemy attack
        // attackNum is used to differentiate enemies with multiple attacks
        // target is the position that the enemy is attacking

        if (!m_CanAttack) {
            return;
        }
    }
    #endregion
}
