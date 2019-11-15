using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour {
    #region Events and Delegates
    public delegate void StartGame();
    public static event StartGame StartGameEvent;
    #endregion

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
    public static GameManager GetSingleton() {
        return m_Singleton;
    }

    private bool[] m_PlayersReady;

    private int m_NumPlayers;
    private int m_NumReady;
    private bool m_GameInProgress;
    #endregion

    #region Initialization
    private void Awake() {
        if (!m_Singleton) {
            m_Singleton = this;
        } else if (m_Singleton != this) {
            Destroy(m_Singleton.gameObject);
        }

        m_Players = new PlayerManager[4];
        m_PlayersReady = new bool[4];
        m_NumPlayers = 0;
        m_GameInProgress = false;
    }

    private void OnEnable() {
        PlayerManager.DeathEvent += Respawn;
        PlayerManager.PlayerReadyEvent += PlayerReady;
    }
    #endregion

    #region Accessors
    public PlayerManager getPlayer(int playerID) {
        return m_Players[playerID];
    }
    #endregion

    #region Player Joining/Leaving
    private void OnPlayerJoined(PlayerInput input) {
        PlayerManager newPlayer = input.gameObject.GetComponent<PlayerManager>();
        if (m_GameInProgress) {
            Destroy(newPlayer.gameObject);
            m_NumPlayers++;
            return;
        }
        AddPlayer(newPlayer);
    }

    private void OnPlayerLeft(PlayerInput player) {
        if (!(m_NumPlayers == 0)) {
            m_NumPlayers--;
        }
    }

    private void PlayerReady(int playerID, bool ready) {
        PlayerManager player = m_Players[playerID];
        if (m_PlayersReady[playerID]) {
            if (!ready) {
                m_PlayersReady[playerID] = false;
                m_NumReady--;
            }
        } else if (ready) {
            m_PlayersReady[playerID] = true;
            m_NumReady++;
            if (m_NumPlayers == m_NumReady) {
                m_GameInProgress = true;
                StartGameEvent();
            }  
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

    #region Adding Players
    private void AddPlayer(PlayerManager player) {
        for (int i = 0; i < m_Players.Length; i++) {
            if (m_Players[i] == null) {
                m_Players[i] = player;
                player.SetID(i);
                player.transform.position = m_spawnPositions[i];
                m_NumPlayers++;
                break;
            }
        }
    }
    #endregion

    #region Disable/Enders
    private void OnDisable() {
        PlayerManager.DeathEvent -= Respawn;
        PlayerManager.PlayerReadyEvent -= PlayerReady;
    }
    #endregion
}
