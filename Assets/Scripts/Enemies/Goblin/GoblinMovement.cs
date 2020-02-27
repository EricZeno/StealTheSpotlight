using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinMovement : EnemyMovement {
    #region Movement Algorithm
    protected override void MovementAlgorithm() {
        base.MovementAlgorithm();

        GameObject target = FindClosestTarget();

        if (target == null && transform.position != m_Spawn) {
            m_Manager.Reset();
            return;
        }
        if (target == null) {
            return;
        }
        Vector2 dir = target.transform.position - transform.position;
        SetMove(dir);


        float dist = Vector2.Distance(transform.position, target.transform.position);
        float scale = Mathf.Max(transform.localScale.x, transform.localScale.y);
        if (dist >= (m_Manager.GetEnemyData().AttackRange * 4 / 5) * scale) {
            Move();
            m_Attack.CanAttack = false;
        }
        else {
            m_Attack.CanAttack = true;
        }
    }
    #endregion
}
