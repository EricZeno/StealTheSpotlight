using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzerMovement : EnemyMovement {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The minimum amount of time the enemy will stay still before moving or attacking")]
    private float m_MinStationaryTime;

    [SerializeField]
    [Tooltip("The maximum amount of time the enemy will stay still before moving or attacking")]
    private float m_MaxStationaryTime;

    [SerializeField]
    [Tooltip("The minimum range the enemy can be at to use the laser attack")]
    private float m_MinCloseAttackRange;

    [SerializeField]
    [Tooltip("The maximum range the enemy can be at to use the laser attack")]
    private float m_MaxCloseAttackRange;

    [SerializeField]
    [Tooltip("The minimum range the enemy can be at to use the bullet attack")]
    private float m_MinLongAttackRange;

    [SerializeField]
    [Tooltip("The maximum range the enemy can be at to use the bullet attack")]
    private float m_MaxLongAttackRange;

    [SerializeField]
    [Tooltip("The probability that the enemy will move to long range when it isn't in attack range")]
    [Range(0, 1)]
    private float m_RetreatChance;

    [SerializeField]
    [Tooltip("The amount of randomness that should be added to the enemy's movement")]
    private float m_MovementNoise;

    [SerializeField]
    [Tooltip("How long the enemy should move for")]
    private float m_MoveTime;
    #endregion

    #region Private Variables
    private bool m_Moving;
    private bool m_Attacking;
    private float m_CurrStationaryTime;
    private float m_CurrMoveTime;
    #endregion
    #endregion

    #region Accessors and Setters
    public bool Attacking {
        set {
            m_Attacking = value;
        }
    }
    #endregion

    #region Initialization
    protected override void Awake() {
        base.Awake();
        m_CurrStationaryTime = Random.Range(m_MinStationaryTime, m_MaxStationaryTime);
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
                m_CurrMoveTime = m_MoveTime;

                FreezePosition();
            }
            else {
                return;
            }
        }

        if (m_CurrStationaryTime > 0) {
            m_CurrStationaryTime -= Time.fixedDeltaTime;
            return;
        }

        GameObject target = FindClosestTarget();
        Vector2 targetPos = target.transform.position;

        if (InCloseRange(targetPos)) {
            m_Attacking = true;
            m_Attack.Attack(BuzzerAttack.CLOSE_ATTACK_NUM, targetPos);
        }
        else if (InLongRange(targetPos)) {
            m_Attacking = true;
            m_Attack.Attack(BuzzerAttack.LONG_ATTACK_NUM, targetPos);
        }
        else {
            CalculateMove(targetPos);
        }

        m_CurrStationaryTime = Random.Range(m_MinStationaryTime, m_MaxStationaryTime);
    }

    private void CalculateMove(Vector2 targetPos) {
        Unfreeze();

        float distance = Vector2.Distance(targetPos, m_RB.position);

        if (distance > m_MaxLongAttackRange) {
            MoveCloser(targetPos);
        }
        else if (distance < m_MinCloseAttackRange) {
            MoveAway(targetPos);
        }
        else {
            if (Random.value < m_RetreatChance) {
                MoveAway(targetPos);
            }
            else {
                MoveCloser(targetPos);
            }
        }
    }

    private void MoveCloser(Vector2 targetPos) {
        Vector2 targetVec = targetPos - m_RB.position;

        float xNoise = Random.Range(-m_MovementNoise, m_MovementNoise);
        float yNoise = Random.Range(-m_MovementNoise, m_MovementNoise);

        targetVec += new Vector2(xNoise, yNoise);
        SetMove(targetVec);

        m_Moving = true;
    }

    private void MoveAway(Vector2 targetPos) {
        Vector2 targetVec = m_RB.position - targetPos;

        float xNoise = Random.Range(-m_MovementNoise, m_MovementNoise);
        float yNoise = Random.Range(-m_MovementNoise, m_MovementNoise);

        targetVec += new Vector2(xNoise, yNoise);
        SetMove(targetVec);

        m_Moving = true;
    }
    #endregion

    #region Range Check
    private bool InCloseRange(Vector2 target) {
        float distance = Vector2.Distance(target, transform.position);
        return distance >= m_MinCloseAttackRange && distance <= m_MaxCloseAttackRange;
    }

    private bool InLongRange(Vector2 target) {
        float distance = Vector2.Distance(target, transform.position);
        return distance >= m_MinLongAttackRange && distance <= m_MaxLongAttackRange;
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
