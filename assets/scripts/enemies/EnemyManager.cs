using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyManager : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The starting data for the enemy.")]
    private EnemyData m_Data;

    [SerializeField]
    [Tooltip("The attack functionality for the enemy.")]
    private EnemyAttack m_Attack;

    [SerializeField]
    [Tooltip("The drop category of the enemy.")]
    private EDropSourceCategory m_DropCategory;
    #endregion

    #region Private Variables
    private EnemyMovement m_Movement;
    private Room m_Room;
    #endregion

    #region Initialization
    private void Start() {
        m_Movement = GetComponent<EnemyMovement>();
        m_Room = GetComponentInParent<Room>();
        m_Data.ResetAllStatsDefault();
        float range = m_Data.AttackRange;
        m_Attack.InitializeAttackCollider(range);

    }
    #endregion

    #region Accessors and Setters
    public float GetMoveSpeed() {
        return m_Data.CurrMovementSpeed;
    }

    public Vector2 GetDir() {
        return m_Movement.dir;
    }

    public EnemyData GetEnemyData() {
        return m_Data;
    }

    public EnemyAttack GetEnemyAttack() {
        return m_Attack;
    }

    public Room GetRoom() {
        return m_Room;
    }
    #endregion

    #region Drops
    private void DropItem() {
        int itemId = DropManager.GetDrop(m_DropCategory);
        if (itemId == -1) {
            return;
        }
        DropManager.DropItem(itemId, transform.position);
    }
    #endregion

    #region Health Methods
    public void TakeDamage(int damage, int playerID) {
        m_Data.TakeDamage(damage);
        if (m_Data.CurrHealth <= 0) {
            GetComponentInParent<Room>().EnemyDeath(this, playerID);
            DropItem();
            Destroy(gameObject);
        }
    }

    public void Heal(float m_HealPercent) {
        int healing = (int)(m_HealPercent * m_Data.MaxHealth);
        m_Data.Heal(healing);
    }

    private IEnumerator HealToFull() {
        float t = 0f;
        float dur = 3f;
        while (t < dur) {
            Heal(10f);
            t += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Status Effects
    public void AddStatusEffectForXSec(Status status, float effectLength) {
        StartCoroutine(AddTempStatusEffect(status, effectLength));
    }

    private IEnumerator AddTempStatusEffect(Status status, float effectLength) {
        m_Data.AddStatus(status);
        yield return new WaitForSeconds(effectLength);
        m_Data.RemoveStatus(status);
    }
    #endregion

    #region Room Logic
    public void Reset() {
        m_Movement.Reset();
        StartCoroutine(HealToFull());
    }
    #endregion
}
