using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistMovement : EnemyMovement {
    #region Private Variables
    private Vector2 currVec;
    private bool m_Moving;
    private float m_MoveTime;
    private bool m_Attacking;
    public bool Attacking {
        set {
            m_Attacking = value;
        }
    }
    private float m_CurrMoveTime;
    private float m_Range;
    #endregion

    #region Initialization
    protected override void Awake() {
        base.Awake();
        m_MoveTime = 1f / m_Manager.GetEnemyData().AttackSpeed;
        m_Range = m_Manager.GetEnemyData().AttackRange;
    }
    #endregion

    #region Movement
    protected override void MovementAlgorithm() {
        base.MovementAlgorithm();
        if (m_Attacking) {
            return;
        }

        if (m_Moving) {
            Move();

            m_CurrMoveTime -= Time.fixedDeltaTime;

            if (m_CurrMoveTime <= 0) {
                m_Moving = false;

            }
            else {
                return;
            }
        }

        GameObject target = FindClosestTarget();
        if (target != null) {
            Vector2 targetPos = target.transform.position;

            if (InRange(targetPos)) {
                m_Attacking = true;
                ((CultistAttack)m_Attack).Attack(0, targetPos);

                CalculateMove();
            }
        }
        else {
            CalculateMove();
        }
    }

    private void CalculateMove() {
        Unfreeze();

        if (m_CurrMoveTime <= 0) {
            Vector2 randomVec = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            currVec = randomVec;
            SetMove(currVec);
        }
        else {
            SetMove(currVec);
        }
        m_Moving = true;
        m_CurrMoveTime = m_MoveTime;
    }
    #endregion

    #region Range Check
    private bool InRange(Vector2 target) {
        float distance = Vector2.Distance(target, transform.position);
        return distance <= m_Range;
    }
    #endregion

    #region Collision
    private void OnCollisionEnter2D(Collision2D collision) {
        if (m_Moving && collision.gameObject.layer == LayerMask.NameToLayer("Wall")
             || collision.gameObject.layer == LayerMask.NameToLayer("Object")) {
            m_Moving = false;
            m_CurrMoveTime = m_MoveTime;
            FreezePosition();
        }
    }
    #endregion

    #region Stopping Pushes
    private void FreezePosition() {
        m_RB.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void Unfreeze() {
        m_RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    #endregion
}
