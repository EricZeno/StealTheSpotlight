using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonkersMovement : EnemyMovement {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The max distance for Chonkers to sweep")]
    private float m_MaxShockwaveDist;

    [SerializeField]
    [Tooltip("The max distance for Chonkers to ram")]
    private float m_MaxRamDist;

    [SerializeField]
    [Tooltip("The max timer before Chonkers changes directions")]
    private float m_MaxRandomMovementTimer;

    [SerializeField]
    [Tooltip("The minimum timer before Chonkers changes directions")]
    private float m_MinRandomMovementTimer;

    [SerializeField]
    [Tooltip("The max timer before Chonkers changes directions when next to a wall")]
    private float m_MaxOnWallTimer;

    [SerializeField]
    [Tooltip("The offset X when moving towards the center")]
    private float m_OffsetX;

    [SerializeField]
    [Tooltip("The offset Y when moving towards the center")]
    private float m_OffsetY;
    #endregion

    #region Private Variables
    private bool m_Attacking;
    private bool m_InitialWallCollision;
    private bool m_WallCollision;
    private float m_RandomTimer;
    private float m_OnWall;
    #endregion
    #endregion

    #region Accessors and Setters
    public bool Attacking {
        set {
            m_Attacking = value;
        }
        get {
            return m_Attacking;
        }
    }
    #endregion

    #region Initialization
    protected override void Awake() {
        base.Awake();
        m_RandomTimer = Random.Range(m_MinRandomMovementTimer, m_MaxRandomMovementTimer);
        m_OnWall = m_MaxOnWallTimer;
        SetMove(new Vector2(Random.Range(-100, 101), Random.Range(-100, 101)));
    }
    #endregion

    #region Main Updates
    protected override void Update() {
        base.Update();
        m_RandomTimer -= Time.deltaTime;
        if (m_RandomTimer <= 0 && !m_Attacking) {
            m_RandomTimer = Random.Range(m_MinRandomMovementTimer, m_MaxRandomMovementTimer);
            SetMove(new Vector2(Random.Range(-100, 101), Random.Range(-100, 101)));
        }
        if (m_WallCollision && !m_Attacking) {
            m_OnWall -= Time.deltaTime;
            if (m_OnWall <= 0) {
                m_OnWall = m_MaxOnWallTimer;
                Vector3 center = new Vector3(GetComponentInParent<Room>().GetComponent<BoxCollider2D>().bounds.center.x, GetComponentInParent<Room>().GetComponent<BoxCollider2D>().bounds.center.y);
                Vector3 offset = new Vector3(Random.Range(-m_OffsetX, m_OffsetX + 1), Random.Range(-m_OffsetY, m_OffsetY + 1));
                SetMove(center - transform.position + offset);
            }
        }
    }
    #endregion

    #region Movement
    protected override void MovementAlgorithm() {
        base.MovementAlgorithm();

        GameObject target = FindClosestTarget();
 
        if (target == null) {
            return;
        }

        Vector3 targetPos = target.transform.position;
        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (((ChonkersAttack)m_Attack).GetShockwaveCurrCD <= 0 && dist <= m_MaxShockwaveDist && !m_Attacking) {
            m_Attacking = true;
            m_Attack.Attack(ChonkersAttack.SHOCKWAVE_ATTACK_NUM, targetPos);
        }
        else if (((ChonkersAttack)m_Attack).GetRamCurrCD <= 0 && dist <= m_MaxRamDist && !m_Attacking) {
            m_Attacking = true;
            m_Attack.Attack(ChonkersAttack.RAM_ATTACK_NUM, targetPos);
        }
        else if (m_InitialWallCollision == true) {
            m_RandomTimer = Random.Range(m_MinRandomMovementTimer, m_MaxRandomMovementTimer);
            Vector2 dir = new Vector2(-m_MoveDir.y, m_MoveDir.x);
            SetMove(dir);
            m_InitialWallCollision = false;
        }
        if (!m_Attacking) {
            Move();
        }
    }

    #endregion

    #region Colliders
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Object")) {
            m_InitialWallCollision = true;
            m_WallCollision = true;
            ((ChonkersAttack)m_Attack).SetCollide = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Object")) {
            m_WallCollision = false;
            m_OnWall = m_MaxOnWallTimer;
            ((ChonkersAttack)m_Attack).SetCollide = false;
        }
    }
    #endregion
}
