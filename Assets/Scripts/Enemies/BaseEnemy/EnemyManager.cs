using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyGraphics))]
public class EnemyManager : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The starting data for the enemy.")]
    private EnemyData m_Data;

    [SerializeField]
    [Tooltip("The drop category of the enemy.")]
    private EDropSourceCategory m_DropCategory;
    #endregion

    #region Private Variables
    private EnemyMovement m_Movement;
    private Room m_Room;

    private bool m_IsFlashing;
    #endregion

    #region Cached Components
    private EnemyGraphics m_Graphics;
    #endregion

    #region Initialization
    private void Start() {
        m_Movement = GetComponent<EnemyMovement>();
        m_Room = GetComponentInParent<Room>();
        m_Data.ResetAllStatsDefault();
        m_Graphics = GetComponent<EnemyGraphics>();
    }
    #endregion

    #region Accessors and Setters
    public float GetMoveSpeed() {
        return m_Data.CurrMovementSpeed;
    }

    public Vector2 GetDir() {
        return m_Movement.Dir;
    }

    public EnemyData GetEnemyData() {
        return m_Data;
    }

    public Room GetRoom() {
        return m_Room;
    }
    #endregion

    #region Drops
    private void DropItem() {
        int itemId = DropManager.GetDrop(m_DropCategory);
        if (itemId == Consts.NULL_ITEM_ID) {
            return;
        }
        DropManager.DropItem(itemId, transform.position);
    }
    #endregion

    #region Health Methods
    public void TakeDamage(int damage, int playerID) {
        m_Data.TakeDamage(damage);

        if (!m_IsFlashing) {
            StartCoroutine(DamageFlash());
        }

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

    private IEnumerator DamageFlash() {
        m_IsFlashing = true;

        float flashDelay = .1f;
        Color flashColor = Color.red;
        int numOfFlashes = 1;
        for (int i = 0; i < numOfFlashes; i++) {
            m_Graphics.SetColor(flashColor);
            yield return new WaitForSeconds(flashDelay);
            m_Graphics.SetColor(Color.white);
            yield return new WaitForSeconds(flashDelay);
        }

        m_IsFlashing = false;
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
