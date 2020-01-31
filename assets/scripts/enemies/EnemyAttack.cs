using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAttack : MonoBehaviour {

    #region Private Variables
    // This vector holds the player's current movement input before it's
    // translated to the rigidbody
    private float m_AttackCD;
    private float m_CurrCD;
    private float m_RotationTime = 0.15f;
    private bool m_AttackLocked;
    private bool m_CanAttack;
    public bool CanAttack {
        set {
            m_CanAttack = value;
        }
    }
    #endregion

    #region Cached Components
    private EnemyManager m_Manager;
    private BoxCollider2D m_AttackCollider;
    #endregion

    #region Initialization
    private void Awake() {
        m_Manager = GetComponentInParent<EnemyManager>();
        m_AttackCD = 1 / m_Manager.GetEnemyData().AttackSpeed;
    }

    public void InitializeAttackCollider(float radius) {
        m_AttackCollider = gameObject.AddComponent<BoxCollider2D>();
        m_AttackCollider.offset = new Vector2(radius / 2, 0);
        m_AttackCollider.size = new Vector2(radius, 0.01f);
        m_AttackCollider.isTrigger = true;
        m_AttackCollider.enabled = true;
    }
    #endregion

    #region Update
    private void Update() {
        if (m_CanAttack && m_CurrCD <= 0) {
            StartCoroutine(Attack());
        }
        else if (!m_AttackLocked && m_CurrCD > 0) {
            m_CurrCD -= Time.deltaTime;
        }
    }
    #endregion

    #region Attack
    private IEnumerator Attack() {
        m_AttackLocked = true;
        m_CurrCD = 2 * m_AttackCD / 3;

        yield return new WaitForSeconds(m_AttackCD / 3);

        Vector2 dir = m_Manager.GetDir();
        Debug.Log(dir);
        float attackCone = m_Manager.GetEnemyData().AttackCone;
        transform.rotation = Quaternion.Euler(0, 0, Vector3.Angle(Vector3.right, dir));

        transform.Rotate(0, 0, attackCone / 2);
        Quaternion target = transform.rotation;
        transform.Rotate(0, 0, -attackCone);

        m_AttackCollider.enabled = true;
        Quaternion r = transform.rotation;
        float t = 0f;
        while (t < m_RotationTime) {
            transform.rotation = Quaternion.Lerp(r, target, t / m_RotationTime);
            t += Time.deltaTime;
            yield return null;
        }

        m_AttackCollider.enabled = false;

        m_AttackLocked = false;
    }
    #endregion

    #region Attack Colliders
    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject target = collision.gameObject;
        if (target.CompareTag(Consts.PLAYER_TAG)) {
            target.GetComponent<PlayerManager>().TakeDamage(m_Manager.GetEnemyData().Damage);
        }
    }
    #endregion
}
