using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyManager))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(EnemyGraphics))]
public class EnemyMovement : MonoBehaviour {
    #region Variables
    #region Private Variables
    private Vector2 m_MoveDir;
    private bool m_Reset;
    public Vector2 dir {
        get {
            return m_MoveDir;
        }
    }
    private List<GameObject> m_Targets;
    private Vector3 m_Spawn;
    private Vector2 m_ExternalForce;
    #endregion

    #region Cached Components
    private Rigidbody2D m_Rb;
    private EnemyManager m_Manager;
    private CircleCollider2D m_Trigger;
    private EnemyGraphics m_Graphics;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Manager = GetComponent<EnemyManager>();
        m_Trigger = GetComponent<CircleCollider2D>();
        m_Graphics = GetComponent<EnemyGraphics>();
        m_Targets = new List<GameObject>();
        m_Spawn = transform.position;
        m_ExternalForce = Vector2.zero;
    }
    #endregion

    #region Main Updates
    private void Update() {
        UpdateGFX();
    }

    private void FixedUpdate() {
        MovementAlgorithm();
    }
    #endregion

    #region Input Receivers
    // Detects movement "input" from movement algorithm
    private void SetMove(Vector2 value) {
        m_MoveDir = value;
        m_MoveDir.Normalize();
    }
    #endregion

    #region Movement
    private void Move() {
        Vector2 delta = m_MoveDir * Time.fixedDeltaTime;
        delta *= m_Manager.GetEnemyData().CurrMovementSpeed;
        m_Rb.MovePosition(m_Rb.position + delta);
    }

    //New enemy movement scripts should override this
    private void MovementAlgorithm() {
        if (m_Reset) {
            Vector2 dirReset = m_Spawn - transform.position;
            SetMove(dirReset);

            Vector2 error = transform.position - m_Spawn;
            if (error.magnitude > 0.5f) {
                Move();
            }
            else {
                m_Reset = false;
            }
            return;
        }
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
            m_Manager.GetEnemyAttack().CanAttack = false;
        }
        else {
            m_Manager.GetEnemyAttack().CanAttack = true;
        }
    }

    private GameObject FindClosestTarget() {
        Vector2 pos = transform.position;
        float minDist = float.PositiveInfinity;
        GameObject minTarget = null;
        foreach (GameObject target in m_Targets) {
            float dist = Vector2.Distance(pos, target.transform.position);
            if (dist < minDist) {
                dist = minDist;
                minTarget = target;
            }
        }
        return minTarget;
    }
    #endregion

    #region External Forces
    public void ApplyExternalForce(Vector2 initialForce) {
        m_ExternalForce += initialForce;
    }

    private void UpdateExternalForce() {
        m_ExternalForce = Vector2.Lerp(m_ExternalForce, Vector2.zero, Consts.EXTERNAL_FORCE_REDUCTION_RATE);
    }
    #endregion

    #region Room Logic
    public void Reset() {
        m_Reset = true;
    }
    #endregion

    #region Collisions
    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject target = collision.gameObject;
        if (target.CompareTag(Consts.PLAYER_TAG)) {
            m_Targets.Add(target);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        GameObject target = collision.gameObject;
        if (target.CompareTag(Consts.PLAYER_TAG)) {
            m_Targets.Remove(target);
        }
    }
    #endregion

    #region Graphics
    private void UpdateGFX() {
        m_Graphics.Move(m_MoveDir);
        m_Graphics.FacingDirection(m_MoveDir);
    }
    #endregion
}
