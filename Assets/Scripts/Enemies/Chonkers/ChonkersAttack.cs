using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonkersAttack : EnemyAttack {
    #region Constants
    public const int SHOCKWAVE_ATTACK_NUM = 0;
    public const int RAM_ATTACK_NUM = 1;
    #endregion

    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("Cooldown for sweep")]
    private float m_SweepCD;

    [SerializeField]
    [Tooltip("Cooldown for ram")]
    private float m_RamCD;

    [SerializeField]
    [Tooltip("How big the sweep is")]
    private float m_SweepRadius;

    [SerializeField]
    [Tooltip("How big the ram is")]
    private float m_RamRadius;

    [SerializeField]
    [Tooltip("How big the ram is")]
    private float m_ShockwaveTime;
    #endregion

    #region Private Variables
    private float m_SweepCurrCD;
    private float m_RamCurrCD;
    private bool m_Collide;
    private int m_AttackNum;
    private Animator m_Animator;


    public bool SetCollide {
        set {
            m_Collide = value;
        }
    }
    public float GetShockwaveCurrCD {
        get {
            return m_SweepCurrCD;
        }
    }
    public float GetRamCurrCD {
        get {
            return m_RamCurrCD;
        }
    }
    #endregion

    #region Cached Components
    private BoxCollider2D m_RamCollider;
    private CircleCollider2D m_ShockwaveCollider;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        base.Awake();
        m_SweepCurrCD = 0;
        m_RamCurrCD = 0;
        InitializeAttackCollider();
        m_Animator = GetComponentInParent<Animator>();
    }

    public void InitializeAttackCollider() {
        m_RamCollider = gameObject.AddComponent<BoxCollider2D>();
        m_RamCollider.isTrigger = true;
        m_RamCollider.enabled = false;
        m_RamCollider.size = new Vector2(m_RamRadius, m_RamRadius);
        m_RamCollider.offset = new Vector2(-0.3f, 0);

        m_ShockwaveCollider = gameObject.AddComponent<CircleCollider2D>();
        m_ShockwaveCollider.isTrigger = true;
        m_ShockwaveCollider.enabled = false;
        m_ShockwaveCollider.radius = m_SweepRadius;
        m_ShockwaveCollider.offset = new Vector2(0, 0);

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    #endregion

    #region Update
    private void Update() {
        if (!((ChonkersMovement)m_Movement).Attacking) {
            if (m_RamCurrCD > 0) {
                m_RamCurrCD -= Time.deltaTime;
            }
            if (m_SweepCurrCD > 0) {
                m_SweepCurrCD -= Time.deltaTime;
            }
        }
    }
    #endregion

    #region External Calls
    public override void Attack(int attackNum, Vector2 target) {
        m_AttackNum = attackNum;
        switch (attackNum) {
            case SHOCKWAVE_ATTACK_NUM:
                Debug.Log("Shockwave");
                m_SweepCurrCD = m_SweepCD;
                StartCoroutine(Shockwave(target));
                break;
            case RAM_ATTACK_NUM:
                m_RamCurrCD = m_RamCD;
                StartCoroutine(Ram(target));
                break;
            default:
                throw new System.ArgumentException("Trying to trigger an invalid attack");
        }
    }
    #endregion

    #region Attacks
    private IEnumerator Shockwave(Vector3 target) {
        //Insert animation
        m_Animator.SetBool("IsShockwaving", true);
        yield return new WaitForSeconds(0.8f);
        Quaternion r = transform.rotation;
        m_ShockwaveCollider.enabled = true;
        float t = 0f;
        while (t < m_ShockwaveTime) {
            t += Time.deltaTime;
            yield return null;
        }
        m_Animator.SetBool("IsShockwaving", false);
        m_ShockwaveCollider.enabled = false;

        //Insert recovering phase
        yield return new WaitForSeconds(2);

        ((ChonkersMovement)m_Movement).Attacking = false;
    }

    private IEnumerator Ram(Vector3 target) {
        //Insert animation
        Vector2 dir = target - transform.position;
        Debug.Log(dir);
        bool isRammingLeft = dir.x < 0;
        m_Animator.SetBool("IsRammingLeft", isRammingLeft);
        m_Animator.SetBool("IsRamming", true);
        yield return new WaitForSeconds(0.5f);
        m_RamCollider.enabled = true;

        float t = 0f;
        while (!m_Collide) {
            Vector2 delta = dir * m_Manager.GetEnemyData().CurrMovementSpeed;
            delta *= Time.fixedDeltaTime;
            Rigidbody2D Rb = GetComponentInParent<Rigidbody2D>();
            Rb.MovePosition(Rb.position + delta);
            yield return null;
        }
        m_Animator.SetBool("IsRamming", false);
        m_RamCollider.enabled = false;

        //Insert recovering phase
        yield return new WaitForSeconds(2);

        ((ChonkersMovement)m_Movement).Attacking = false;
    }
    #endregion

    #region Attack Colliders
    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject target = collision.gameObject;
        if (target.CompareTag(Consts.PLAYER_TAG)) {
            Vector2 dir = m_Manager.GetDir();
            switch (m_AttackNum) {
                case SHOCKWAVE_ATTACK_NUM:
                    target.GetComponent<PlayerManager>().TakeDamage(m_Manager.GetEnemyData().Damage);
                    target.GetComponent<PlayerMovement>().ApplyExternalForce(m_Manager.GetDir() * 100);
                    break;
                case RAM_ATTACK_NUM:
                    target.GetComponent<PlayerManager>().TakeDamage(m_Manager.GetEnemyData().Damage);
                    target.GetComponent<PlayerMovement>().ApplyExternalForce(m_Manager.GetDir() * 400);
                    target.GetComponent<PlayerMovement>().ApplyExternalForce(new Vector2(-dir.y, dir.x) * 200);
                    break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        GameObject target = collision.gameObject;
        if (target.CompareTag(Consts.PLAYER_TAG)) {
            Vector2 dir = m_Manager.GetDir();
            switch (m_AttackNum) {
                case SHOCKWAVE_ATTACK_NUM:
                    target.GetComponent<PlayerManager>().TakeDamage(m_Manager.GetEnemyData().Damage);
                    target.GetComponent<PlayerMovement>().ApplyExternalForce(m_Manager.GetDir() * 100);
                    break;
            }
        }
    }
    #endregion
}
