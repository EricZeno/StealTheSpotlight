using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    #region Variables
    #region Editor Variables
    [Header("Raycast for players in the room")]
    [SerializeField]
    [Tooltip("Box cast dimensions")]
    private Vector3 m_castDimensions = new Vector3(26, 14, 0);

    [SerializeField]
    [Tooltip("Player layer")]
    private LayerMask m_Layer;
    #endregion

    #region Private Variables
    private List<int> m_Players;

    //private List<Enemies> m_Enemies;
    #endregion
    #endregion

    private void Start() {
        //m_Enemies = new List<Enemies>();
        foreach(Transform child in transform) {
            //Check if child is an enemy and if so, add it to m_Enemies
        }
    }

    private void Update() {
        GetPlayersInRoom();

        if (m_Players.Count == 0) {
            ResetEnemies();
        }
    }

    private void GetPlayersInRoom() {
        m_Players = new List<int>();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, m_castDimensions, 0, Vector2.zero);

        foreach (RaycastHit2D hit in hits) {
            m_Players.Add(hit.transform.gameObject.GetComponent<PlayerManager>().GetID());
        }
    }

    private void ResetEnemies() {
        /*foreach(Enemy enemy in m_Enemies) {
            enemy.ResetPosition();
        }*/
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, m_castDimensions);
    }
}
