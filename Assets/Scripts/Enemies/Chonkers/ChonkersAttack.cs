using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonkersAttack : EnemyAttack {
    #region Constants
    public const int SWEEP_ATTACK_NUM = 0;
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
    private float m_RotationTime;
    #endregion

    #region Private Variables
    private float m_SweepCurrCD;
    private float m_RamCurrCD;
    private bool m_Collide;
    private int m_AttackNum;


    public bool SetCollide {
        set {
            m_Collide = value;
        }
    }
    public float GetSweepCurrCD {
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
    private BoxCollider2D m_AttackCollider;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        base.Awake();
        m_SweepCurrCD = 0;
        m_RamCurrCD = 0;
        InitializeAttackCollider();
    }

    public void InitializeAttackCollider() {
        m_AttackCollider = gameObject.AddComponent<BoxCollider2D>();
        m_AttackCollider.offset = new Vector2(0, 0);
        m_AttackCollider.isTrigger = true;
        m_AttackCollider.enabled = false;
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
        SetAttackCollider(m_AttackNum);
        switch (attackNum) {
            case SWEEP_ATTACK_NUM:
                m_SweepCurrCD = m_SweepCD;
                StartCoroutine(Sweep(target));
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
    private IEnumerator Sweep(Vector3 target) {
        SetAttackCollider(SWEEP_ATTACK_NUM);
        //Insert animation
        yield return new WaitForSeconds(0.5f);
        //Not sure how to get the initial angle.
        //transform.rotation = Quaternion.Euler(0, 0, Vector3.Angle(gameObject.GetComponentInParent<Transform>().position, target));
        Quaternion r = transform.rotation;
        m_AttackCollider.enabled = true;
        float t = 0f;
        while (t < m_RotationTime / 2) {
            transform.rotation = Quaternion.Lerp(r, Quaternion.Euler(0, 0, 180), t / (m_RotationTime / 2));
            t += Time.deltaTime;
            yield return null;
        }

        r = Quaternion.Euler(0, 0, 180);
        t = 0f;
        while (t < m_RotationTime / 2) {
            transform.rotation = Quaternion.Lerp(r, Quaternion.Euler(0, 0, 360), t / (m_RotationTime / 2));
            t += Time.deltaTime;
            yield return null;
        }

        m_AttackCollider.enabled = false;

        //Insert recovering phase
        yield return new WaitForSeconds(2);

        ((ChonkersMovement)m_Movement).Attacking = false;
    }

    private IEnumerator Ram(Vector3 target) {
        SetAttackCollider(RAM_ATTACK_NUM);
        //Insert animation
        yield return new WaitForSeconds(0.5f);
        m_AttackCollider.enabled = true;

        Vector2 dir = target - transform.position;

        float t = 0f;
        while (!m_Collide) {
            Vector2 delta = dir * m_Manager.GetEnemyData().CurrMovementSpeed;
            delta *= Time.fixedDeltaTime;
            Rigidbody2D Rb = GetComponentInParent<Rigidbody2D>();
            Rb.MovePosition(Rb.position + delta);
            yield return null;
        }

        m_AttackCollider.enabled = false;

        //Insert recovering phase
        yield return new WaitForSeconds(2);

        ((ChonkersMovement)m_Movement).Attacking = false;
    }
    #endregion

    #region Attack Helpers
    private void SetAttackCollider(int attackNum) {
        switch (attackNum) {
            case SWEEP_ATTACK_NUM:
                m_AttackCollider.size = new Vector2(m_SweepRadius, 1);
                m_AttackCollider.offset = new Vector2(2, 0);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case RAM_ATTACK_NUM:
                m_AttackCollider.size = new Vector2(m_RamRadius, m_RamRadius);
                m_AttackCollider.offset = new Vector2(0, 0);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }

    private float signedAngleBetween(Vector3 a, Vector3 b) {
        float angle = Vector3.Angle(a, b);
        if (Mathf.Sign(angle) == -1)
            angle = 360 + angle;
        return angle;
    }
    #endregion

    #region Attack Colliders
    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject target = collision.gameObject;
        if (target.CompareTag(Consts.PLAYER_TAG)) {
            Vector2 dir = m_Manager.GetDir();
            switch (m_AttackNum) {
                case SWEEP_ATTACK_NUM:
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
                case SWEEP_ATTACK_NUM:
                    target.GetComponent<PlayerManager>().TakeDamage(m_Manager.GetEnemyData().Damage);
                    target.GetComponent<PlayerMovement>().ApplyExternalForce(m_Manager.GetDir() * 100);
                    break;
            }
        }
    }
    #endregion
}
