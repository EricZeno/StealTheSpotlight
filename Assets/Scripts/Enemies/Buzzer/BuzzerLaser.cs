using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class BuzzerLaser : MonoBehaviour {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much additional damage the laser attack will deal")]
    private float m_LaserDamageMultiplier;
    #endregion

    #region Private Variables
    private LineRenderer m_LineRederer;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_LineRederer = GetComponent<LineRenderer>();
    }

    public void SetupAndFire(Vector2 origin, float distance, float width, Vector2 direction, int baseDamage) {
        m_LineRederer.startWidth = width;
        m_LineRederer.endWidth = width;

        // Need to add playerhitbox layermask
        Vector2 boxDimensions = new Vector2(width, width);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, boxDimensions, 0, direction, distance);

        Vector2 endPosition = origin + direction * distance;

        m_LineRederer.SetPosition(0, origin);
        m_LineRederer.SetPosition(1, endPosition);

        int damage = (int)(baseDamage * m_LaserDamageMultiplier);

        List<PlayerManager> playersHit = new List<PlayerManager>();

        foreach (RaycastHit2D hit in hits) {
            if (hit.collider.CompareTag("Player")) {
                PlayerManager player = hit.collider.gameObject.GetComponent<PlayerManager>();
                if (!playersHit.Contains(player)) {
                    player.TakeDamage(damage);
                    playersHit.Add(player);
                }
            }
        }
    }
    #endregion
}
