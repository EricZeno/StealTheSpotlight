using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobMovement : EnemyMovement {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The minimum number of bounces the blob will take before resting")]
    private int m_MinNumBounces;

    [SerializeField]
    [Tooltip("The maximum number of bounces the blob will take before resting")]
    private int m_MaxNumBounces;

    [SerializeField]
    [Tooltip("How long the blob will move for on a single bounce")]
    private float m_BounceTime;

    [SerializeField]
    [Tooltip("How long the blob will pause between bounces at minimum")]
    private float m_BounceDelay;

    [SerializeField]
    [Tooltip("The minimum time the blob will rest for")]
    private int m_MinRestTime;

    [SerializeField]
    [Tooltip("The maximum time the blob will rest for")]
    private int m_MaxRestTime;

    [SerializeField]
    [Tooltip("The amount of randomness that should be added to the enemy's movement")]
    private float m_MovementNoise;
    #endregion

    #region Private Variables
    private int m_CurrBounces;
    private int m_NumBounces;
    private float m_CurrMoveTime;
    private float m_CurrRestTime;
    private float m_CurrBounceDelay;
    private bool m_Moving;
    #endregion
    #endregion

    #region Movement
    protected override void MovementAlgorithm() {
        base.MovementAlgorithm();

        if (m_Moving) {
            if (m_CurrBounceDelay > 0) {
                m_CurrBounceDelay -= Time.fixedDeltaTime;
                return;
            }

            Move();

            m_CurrMoveTime -= Time.fixedDeltaTime;

            if (m_CurrMoveTime <= 0) {
                BounceComplete();
            }
            else {
                return;
            }
        }

        if (m_CurrRestTime > 0) {
            m_CurrRestTime -= Time.fixedDeltaTime;
            return;
        }

        m_NumBounces = Random.Range(m_MinNumBounces, m_MaxNumBounces);
        CalculateMove();
    }

    private void CalculateMove() {
        GameObject target = FindClosestTarget();

        if (target == null) {

            return;
        }

        m_CurrMoveTime = m_BounceTime;

        Vector2 targetPos = target.transform.position;
        Vector2 targetVec = targetPos - m_RB.position;

        float xNoise = Random.Range(-m_MovementNoise, m_MovementNoise);
        float yNoise = Random.Range(-m_MovementNoise, m_MovementNoise);

        targetVec += new Vector2(xNoise, yNoise);
        SetMove(targetVec);
        m_Moving = true;
    }

    private void BounceComplete() {
        if (m_CurrBounces == m_NumBounces - 1) {
            Rest();
        }
        else {
            m_CurrBounces++;
            m_CurrBounceDelay = m_BounceDelay;
            CalculateMove();
        }
    }

    private void Rest() {
        m_CurrBounces = 0;
        m_CurrRestTime = Random.Range(m_MinRestTime, m_MaxRestTime);
        m_Moving = false;
        SetMove(Vector2.zero);
    }
    #endregion
}
