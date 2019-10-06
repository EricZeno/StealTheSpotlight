using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("Each of the players currently in the game.")]
    private PlayerManager[] m_Players;

    [SerializeField]
    [Tooltip("The spawn locations for each player")]
    private Vector3[] m_spawnPositions;
    #endregion

    #region Private Variables
    private static GameManager m_Singleton;
    public static GameManager getSingleton() {
        return m_Singleton;
    }
    private int m_NumPlayers;
    #endregion

    #region Initialization
    private void Awake() {
        if (!m_Singleton) {
            m_Singleton = this;
        } else if (m_Singleton != this) {
            Destroy(m_Singleton.gameObject);
        }

        m_Players = new PlayerManager[4];
        m_NumPlayers = 0;
    }

    private void OnEnable() {
        PlayerManager.DeathEvent += Respawn;
    }
    #endregion

    #region Accessors
    public PlayerManager getPlayer(int playerID) {
        return m_Players[playerID];
    }

    public int NumPlayers { get; private set; }
    #endregion

    #region Player Joining/Leaving
    private void OnPlayerJoined(PlayerInput input) {
        PlayerManager newPlayer = input.gameObject.GetComponent<PlayerManager>();
        m_Players[m_NumPlayers] = newPlayer;
        newPlayer.SetID(m_NumPlayers);
        newPlayer.transform.position = m_spawnPositions[m_NumPlayers];
        m_NumPlayers++;
    }

    private void OnPlayerLeft(PlayerInput player) {
        if (!(m_NumPlayers == 0)) {
            m_NumPlayers--;
        }
    }
    #endregion

    #region Respawn
    private void Respawn(int playerID, int respawnTime) {
        StartCoroutine(RespawnCoroutine(playerID, respawnTime));
    }

    private IEnumerator RespawnCoroutine(int playerID, int respawnTime) {
        m_Players[playerID].enabled = false;
        yield return new WaitForSeconds(respawnTime);
        m_Players[playerID].transform.position = m_spawnPositions[playerID];
        m_Players[playerID].enabled = true;
        m_Players[playerID].Heal(100f);
    }
    #endregion

    #region Disable/Enders
    private void OnDisable() {
        PlayerManager.DeathEvent -= Respawn;
    }
    #endregion
}
