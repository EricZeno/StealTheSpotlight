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
    private Vector2[] m_spawnPositions;
    #endregion

    #region Private Variables
    private static GameManager p_Singleton;
    public static GameManager getSingleton() {
        return p_Singleton;
    }
    private int p_NumPlayers;
    #endregion

    #region Initialization
    private void Awake() {
        if (!p_Singleton) {
            p_Singleton = this;
        } else if (p_Singleton != this) {
            Destroy(p_Singleton.gameObject);
        }

        m_Players = new PlayerManager[4];
        m_spawnPositions = new Vector2[4];
        p_NumPlayers = 0;
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
        m_Players[p_NumPlayers] = newPlayer;
        newPlayer.SetID(p_NumPlayers);
        newPlayer.transform.position = m_spawnPositions[p_NumPlayers];
        p_NumPlayers++;
    }

    private void OnPlayerLeft(PlayerInput player) {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Respawn
    private void Respawn(int playerID) {

    }
    #endregion

    #region Disable/Enders
    private void OnDisable() {
        PlayerManager.DeathEvent -= Respawn;
    }
    #endregion
}
