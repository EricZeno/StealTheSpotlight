using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyManager))]
[RequireComponent(typeof(EnemyGraphics))]
public class EnemyMovement : MonoBehaviour {
    #region Constants
    private const float RESET_ERROR_TOLERANCE = 0.5f;
    #endregion

    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The attack functionality for the enemy.")]
    protected EnemyAttack m_Attack;
    #endregion

    #region Private Variables
    protected Vector2 m_MoveDir;
    protected Vector3 m_Spawn;
    protected Vector2 m_ExternalForce;
    protected Vector2 m_TargetDir;
    protected bool m_Reset;
    #endregion

    #region Cached Components
    protected Rigidbody2D m_RB;
    protected EnemyManager m_Manager;
    protected EnemyGraphics m_Graphics;
    #endregion
    #endregion

    #region Accessors and Setters
    public Vector2 Dir {
        get {
            return m_MoveDir;
        }
    }

    protected void SetMove(Vector2 value) {
        m_MoveDir = value;
        m_MoveDir.Normalize();
    }
    #endregion

    #region Initialization
    protected virtual void Awake() {
        m_RB = GetComponent<Rigidbody2D>();
        m_Manager = GetComponent<EnemyManager>();
        m_Graphics = GetComponent<EnemyGraphics>();
        m_Spawn = transform.position;
        m_ExternalForce = Vector2.zero;
        m_TargetDir = Vector2.zero;
    }
    #endregion

    #region Main Updates
    protected virtual void Update() {
        UpdateGFX();
        UpdateExternalForce();
    }

    protected void FixedUpdate() {
        MovementAlgorithm();
    }
    #endregion

    #region Movement
    protected virtual void MovementAlgorithm() {
        if (m_Reset) {
            Vector2 dirReset = m_Spawn - transform.position;
            SetMove(dirReset);

            Vector2 error = transform.position - m_Spawn;
            if (error.magnitude > RESET_ERROR_TOLERANCE) {
                Move();
            }
            else {
                m_Reset = false;
            }
            return;
        }
    }

    protected void Move() {
        Vector2 delta = m_MoveDir * m_Manager.GetEnemyData().CurrMovementSpeed;
        delta += m_ExternalForce;
        delta *= Time.fixedDeltaTime;
        m_RB.MovePosition(m_RB.position + delta);
    }
    #endregion

    #region Targeting
    protected GameObject FindClosestTarget() {
        Vector2 pos = transform.position;
        float minDist = float.PositiveInfinity;
        GameObject minTarget = null;
        List<PlayerManager> players = new List<PlayerManager>();
        Room room = m_Manager.GetRoom();

        if (room != null) {
            players = room.GetPlayers();
        }

        foreach (PlayerManager player in players) {
            float dist = Vector2.Distance(pos, player.transform.position);
            if (dist < minDist) {
                dist = minDist;
                minTarget = player.gameObject;
            }
        }

        if (minTarget != null) {
            m_TargetDir = minTarget.transform.position - transform.position;
            m_Graphics.FacingDirection(m_TargetDir);
        }

        return minTarget;
    }
    #endregion

    #region External Forces
    public void ApplyExternalForce(Vector2 initialForce) {
        m_ExternalForce += initialForce;
    }

    protected void UpdateExternalForce() {
        m_ExternalForce = Vector2.Lerp(m_ExternalForce, Vector2.zero, Consts.EXTERNAL_FORCE_REDUCTION_RATE);
    }
    #endregion

    #region Room Logic
    public void Reset() {
        m_Reset = true;
    }
    #endregion

    #region Graphics
    protected void UpdateGFX() {
        m_Graphics.Move(m_MoveDir);
    }
    #endregion
}
