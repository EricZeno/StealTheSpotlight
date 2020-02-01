﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct PlayerSprites {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The body of the player.")]
    private Sprite m_Body;

    [SerializeField]
    [Tooltip("The leg of the player that should be in the foreground.")]
    private Sprite m_FrontLeg;

    [SerializeField]
    [Tooltip("The leg of the player that should be in the background.")]
    private Sprite m_BackLeg;
    #endregion

    #region Accessors
    public Sprite Body {
        get {
            return m_Body;
        }
    }

    public Sprite FrontLeg {
        get {
            return m_FrontLeg;
        }
    }

    public Sprite BackLeg {
        get {
            return m_BackLeg;
        }
    }
    #endregion
}

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour {
    #region Constants
    private const float TIME_TO_WAIT_BEFORE_PLAYER_SETUP = .2f;
    #endregion

    #region Events and Delegates
    public delegate void StartGame();
    public static event StartGame StartGameEvent;
    public delegate void NewFloor();
    public static event NewFloor NewFloorEvent;
    #endregion

    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("The spawn locations for each player")]
    private Vector3[] m_SpawnPositions;

    [SerializeField]
    [Tooltip("All of the layers that can be hit.")]
    private LayerMask m_HittableLayers;

    [SerializeField]
    [Tooltip("The sprites corresponding to each player.")]
    private PlayerSprites[] m_PlayerSprites;
    #endregion

    #region Private Variables
    private static GameManager m_Singleton;
    public static GameManager GetSingleton() {
        return m_Singleton;
    }

    private bool[] m_PlayersReady;
    private static int m_NumPlayers;
    private int m_NumReady;
    private bool m_GameInProgress;

    private PlayerManager[] m_Players;
    private GameObject[] m_PlayerObjs;
    #endregion

    #region Cached Components
    private AudioManager m_AudioManager;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        if (m_Singleton != null) {
            Destroy(gameObject);
            return;
        }
        m_Singleton = this;

        m_Players = new PlayerManager[Consts.MAX_NUM_PLAYERS];
        m_PlayerObjs = new GameObject[Consts.MAX_NUM_PLAYERS];
        m_PlayersReady = new bool[Consts.MAX_NUM_PLAYERS];
        m_NumPlayers = 0;
        m_GameInProgress = false;
        m_AudioManager = GetComponent<AudioManager>();

        DontDestroyOnLoad(this);
    }

    private void OnEnable() {
        PlayerManager.DeathEvent += Respawn;
        PlayerManager.PlayerReadyEvent += PlayerReady;
        CollisionTrigger.SceneChangeEvent += ResetPlayerLocation;
        CollisionTrigger.LoadDungeonEvent += DisablePlayers;
        CollisionTrigger.LoadDungeonEvent += PlaySoundtrack;
        DungeonGenerator.DungeonLoadedEvent += StartFloor;
    }
    #endregion

    #region Accessors
    public PlayerManager getPlayer(int playerID) {
        return m_Players[playerID];
    }

    public static int getNumPlayers() {
        return m_NumPlayers;
    }

    public LayerMask HittableLayers {
        get {
            return m_HittableLayers;
        }
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
        }
        else if (ready) {
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
        m_Players[playerID].transform.position = m_SpawnPositions[playerID];
        m_Players[playerID].enabled = true;
        m_Players[playerID].Heal(100f);
    }
    #endregion

    #region Adding Players
    private void AddPlayer(PlayerManager player) {
        for (int i = 0; i < m_Players.Length; i++) {
            if (m_Players[i] == null) {
                StartCoroutine(SetupPlayer(player, i));
                m_PlayerObjs[i] = player.gameObject;
                m_NumPlayers++;
                DontDestroyOnLoad(m_PlayerObjs[i]);
                break;
            }
        }
    }

    private IEnumerator SetupPlayer(PlayerManager player, int index) {
        yield return new WaitForSeconds(TIME_TO_WAIT_BEFORE_PLAYER_SETUP);
        m_Players[index] = player;
        player.SetID(index);
        player.transform.position = m_SpawnPositions[index];
        string playerLayer = Consts.NO_ID_PLAYER_LAYER + (index + 1).ToString();
        player.gameObject.layer = LayerMask.NameToLayer(playerLayer);
        player.InitialSetup(m_PlayerSprites[index]);
    }
    #endregion

    #region Changing Floors
    private void StartFloor(Vector3[] spawnPositions) {
        m_SpawnPositions = spawnPositions;
        ResetPlayerLocation();
        EnablePlayers();
    }

    private void ResetPlayerLocation() {
        for (int i = 0; i < m_Players.Length; i++) {
            if (m_Players[i]) {
                m_Players[i].transform.position = m_SpawnPositions[i];
            }
        }
    }

    private void DisablePlayers() {
        for (int i = 0; i < m_Players.Length; i++) {
            if (m_Players[i]) {
                m_Players[i].enabled = false;
            }
        }
    }

    private void EnablePlayers() {
        for (int i = 0; i < m_Players.Length; i++) {
            if (m_Players[i]) {
                m_Players[i].enabled = true;
            }
        }
    }

    private void PlaySoundtrack() {
        m_AudioManager.Play("Soundtrack");
    }
    #endregion

    #region Disable/Enders
    private void OnDisable() {
        PlayerManager.DeathEvent -= Respawn;
        PlayerManager.PlayerReadyEvent -= PlayerReady;
        CollisionTrigger.SceneChangeEvent -= ResetPlayerLocation;
        CollisionTrigger.LoadDungeonEvent -= DisablePlayers;
        CollisionTrigger.LoadDungeonEvent -= PlaySoundtrack;
        DungeonGenerator.DungeonLoadedEvent -= StartFloor;
    }
    #endregion
}
