using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Room : MonoBehaviour {
    #region Events and Delegates
    public delegate void RoomCleared(int player);
    public static event RoomCleared RoomClearedEvent;
    #endregion

    #region Private Variables
    private List<PlayerManager> m_Players;
    private List<EnemyManager> m_Enemies;
    private List<GameObject> m_Doors;
    private int[] m_mobkills;
    private int m_totalmobs;
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

        m_totalmobs = m_Enemies.Count;
        m_mobkills = new int[4];
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
        m_mobkills[playerID]++;
        m_Enemies.Remove(enemy);
        if (m_Enemies.Count == 0) {
            // Give room clear points
            OpenDoors();
            Cleared();
        }
    }

    public void ObjectiveEnemyDeath(EnemyManager enemy, int playerID) {
        GetComponentInChildren<ZoneDefense>().KillEnemy();
    }
    #endregion

    #region Doors
    public void OpenDoors() {
        foreach (GameObject door in m_Doors) {
            // Play door opening animation
            door.SetActive(false);
        }
    }
    #endregion

    #region Clearing
    private void Cleared() {
        for (int i = 0; i < GameManager.getNumPlayers(); i++) {
            if (m_mobkills[i] >= m_totalmobs / Consts.MOB_PARTICIPATION) {
                GiveCredit(i);
            }
        }
    }

    private void GiveCredit(int playerID) {
        RoomClearedEvent(playerID);
    }

    #endregion
}
