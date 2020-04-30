using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Room : MonoBehaviour {
    #region Events and Delegates
    public delegate void MobKilled(int player, float enemypoints);
    public static event MobKilled MobKilledEvent;
    #endregion

    #region Private Variables
    private List<PlayerManager> m_Players;
    private List<EnemyManager> m_Enemies;
    private List<GameObject> m_Doors;
    private int[] m_mobkills;
    private int m_totalmobs;
    private AudioManager m_AudioManager;
    #endregion

    #region Initialization
    private void Awake() {
        m_Players = new List<PlayerManager>();
        m_Doors = new List<GameObject>();
        m_AudioManager = GetComponent<AudioManager>();
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
            StartCoroutine(AddPlayer(player));
        }
    }

    private IEnumerator AddPlayer(PlayerManager player) {
        yield return new WaitForSeconds(1f);
        m_Players.Add(player);
        CancelReset();
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Consts.PLAYER_TAG)) {
            PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();
            StartCoroutine(RemovePlayer(player));
        }
    }

    private IEnumerator RemovePlayer(PlayerManager player) {
        yield return new WaitForSeconds(1f);
        if (m_Players.Contains(player)) {
            m_Players.Remove(player);
            if (m_Players.Count == 0) {
                ResetEnemies();
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

    private void CancelReset() {
        foreach (EnemyManager enemy in m_Enemies) {
            enemy.CancelReset();
        }
    }

    public void EnemyDeath(EnemyManager enemy, int playerID) {
        // Give player credit for kill
        m_mobkills[playerID]++;
        m_Enemies.Remove(enemy);
        GiveCredit(playerID, enemy.GetEnemyData().Points);
        if (m_Enemies.Count == 0) {
            // Give room clear points
            OpenDoors();
        }
    }

    public void ObjectiveEnemyDeath(EnemyManager enemy, int playerID) {
        GetComponentInChildren<ZoneDefense>().KillEnemy();
    }

    private void GiveCredit(int playerID, float points) {
        MobKilledEvent(playerID, points);
    }

    public void AddEnemy(EnemyManager enemy) {
        m_Enemies.Add(enemy);
    }
    #endregion

    #region Doors
    public void OpenDoors() {
        m_AudioManager.Play("Opendoor");
        foreach (GameObject door in m_Doors) {
            // Play door opening animation
            door.SetActive(false);
        }


    }
    #endregion
}
