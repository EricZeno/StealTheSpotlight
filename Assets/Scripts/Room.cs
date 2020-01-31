using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Room : MonoBehaviour {
    #region Private Variables
    private List<PlayerManager> m_Players;
    private List<EnemyManager> m_Enemies;
    private List<GameObject> m_Doors;
    #endregion

    #region Initialization
    private void Awake() {
        m_Players = new List<PlayerManager>();
        m_Doors = new List<GameObject>();
    }

    private void Start() {
        EnemyManager[] enemyChildren = GetComponentsInChildren<EnemyManager>();
        if (enemyChildren.Length == 0) {
            m_Enemies = new List<EnemyManager>();
            OpenDoors();
        }
        else {
            m_Enemies = new List<EnemyManager>(enemyChildren);
        }
    }

    public void AddDoors(List<GameObject> doorList) {
        foreach (GameObject door in doorList) {
            if (!m_Doors.Contains(door)) {
                m_Doors.Add(door);
            }
        }
    }
    #endregion

    #region Accessors
    public List<PlayerManager> GetPlayers() {
        return m_Players;
    }
    #endregion

    #region Player Detection
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Consts.PLAYER_TAG)) {
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
            m_Players.Add(player);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Consts.PLAYER_TAG)) {
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
            if (m_Players.Contains(player)) {
                m_Players.Remove(player);
                if (m_Players.Count == 0) {
                    ResetEnemies();
                }
            }
        }
    }
    #endregion

    #region Enemies
    private void ResetEnemies() {
        foreach (EnemyManager enemy in m_Enemies) {
            enemy.Reset();
        }
    }

    public void EnemyDeath(EnemyManager enemy, int playerID) {
        // Give player credit for kill
        m_Enemies.Remove(enemy);
        if (m_Enemies.Count == 0) {
            // Give room clear points
            OpenDoors();
        }
    }
    #endregion

    #region Doors
    private void OpenDoors() {
        foreach (GameObject door in m_Doors) {
            // Play door opening animation
            door.SetActive(false);
        }
    }
    #endregion
}
