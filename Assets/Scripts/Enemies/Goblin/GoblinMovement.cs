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
        Move();
    }
    #endregion
}
