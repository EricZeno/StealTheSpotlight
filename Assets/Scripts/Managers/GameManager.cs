using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private const int NULL = -1;
    private const int STARTSCENE = 0;
    #endregion

    #region Events and Delegates
    public delegate void NewFloor();
    public static event NewFloor NewFloorEvent;
    public delegate void PlayerAdded(int m_NumPlayers);
    public static event PlayerAdded PlayerAddedEvent;
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

    private static int m_NumPlayers;
    private bool m_GameInProgress;

    private PlayerManager[] m_Players;
    private GameObject[] m_PlayerObjs;

    private string[] m_commands;

    private bool m_paused;
    private int m_select;

    private RectTransform LeftCurtain;
    private RectTransform RightCurtain;
    private bool transitioning;
    #endregion

    #region Cached Components
    private AudioManager m_AudioManager;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        if (m_Singleton != null) {
            if (SceneManager.GetActiveScene().buildIndex != 0) {
                Destroy(gameObject);
                return;
            }
            else {
                Destroy(m_Singleton.gameObject);
            }
        }
        m_Singleton = this;

        m_Players = new PlayerManager[Consts.MAX_NUM_PLAYERS];
        m_PlayerObjs = new GameObject[Consts.MAX_NUM_PLAYERS];
        m_NumPlayers = 0;
        m_GameInProgress = false;
        m_AudioManager = GetComponent<AudioManager>();
        m_paused = false;
        m_select = 0;

        m_commands = new string[2];
        m_commands[0] = "resume";
        m_commands[1] = "exit";

        LeftCurtain = transform.GetChild(0).GetChild(4).GetComponent<RectTransform>();
        RightCurtain = transform.GetChild(0).GetChild(5).GetComponent<RectTransform>();
        transitioning = false;

        PlaySoundtrack();
        DontDestroyOnLoad(this);
    }

    private void OnEnable() {
        PlayerManager.DeathEvent += Respawn;
        PointManager.GameEndEvent += RankPlayers;
        CollisionTrigger.FloorChangeEvent += ResetPlayerLocation;
        DungeonGenerator.DungeonLoadedEvent += StartFloor;
        PointManager.SpotlightEvent += SpotlightChange;
        PointManager.PointsUIEvent += PlayerPointUI;
        PlayerManager.PauseEvent += Pause;
        PlayerManager.SelectEvent += Select;
        StartCoroutine(BeginOpeningCurtains());
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

    #region Pause Screen
    private void Pause() {
        if (SceneManager.GetActiveScene().buildIndex != STARTSCENE) {
            if (!m_paused) {
                for (int i = 0; i < m_NumPlayers; i++) {
                    m_Players[i].PauseMap();
                }
                m_paused = true;
                Time.timeScale = 0;
                GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
                GetComponentInChildren<Canvas>(true).transform.GetChild(0).gameObject.SetActive(true);
                GetComponentInChildren<Canvas>(true).transform.GetChild(1).gameObject.SetActive(true);
                GetComponentInChildren<Canvas>(true).transform.GetChild(2).gameObject.SetActive(true);
                GetComponentInChildren<Canvas>(true).transform.GetChild(3).gameObject.SetActive(true);
            }
            else if (m_paused) {
                for (int i = 0; i < m_NumPlayers; i++) {
                    m_Players[i].RegularMap();
                }
                m_paused = false;
                Time.timeScale = 1;
                GetComponentInChildren<Canvas>(true).transform.GetChild(0).gameObject.SetActive(false);
                GetComponentInChildren<Canvas>(true).transform.GetChild(1).gameObject.SetActive(false);
                GetComponentInChildren<Canvas>(true).transform.GetChild(2).gameObject.SetActive(false);
                GetComponentInChildren<Canvas>(true).transform.GetChild(3).gameObject.SetActive(false);
                GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
            }
        }
    }

    private void Select(string input) {
        if (input == "up") {
            GetComponentInChildren<Canvas>(true).gameObject.GetComponentsInChildren<Image>(true)[2 * (m_select + 1)].enabled = false;
            m_select--;
            if (m_select < 0) {
                m_select += m_commands.Length;
            }
            GetComponentInChildren<Canvas>(true).gameObject.GetComponentsInChildren<Image>(true)[2 * (m_select + 1)].enabled = true;
        }
        else if (input == "down") {
            GetComponentInChildren<Canvas>(true).gameObject.GetComponentsInChildren<Image>(true)[2 * (m_select + 1)].enabled = false;
            m_select++;
            if (m_select > m_commands.Length - 1) {
                m_select -= m_commands.Length;
            }
            GetComponentInChildren<Canvas>(true).gameObject.GetComponentsInChildren<Image>(true)[2 * (m_select + 1)].enabled = true;
        }
        else if (input == "choose") {
            switch (m_commands[m_select]) {
                case "resume":
                    Pause();
                    break;
                case "exit":
                    Exit();
                    break;
            }
        }
    }

    private void Exit() {
        Time.timeScale = 1;
        Pause();
        for (int i = 0; i < m_Players.Length; i++) {
            if (m_PlayerObjs[i] != null) {
                Destroy(m_PlayerObjs[i].gameObject);
            }
        }
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Curtain Logic
    public void CallCurtainsCoroutine() {
        StartCoroutine(BeginClosingCurtains());
    }
    IEnumerator BeginClosingCurtains() {
        transitioning = true;
        bool startedClose = false;
        transform.GetChild(0).gameObject.SetActive(true);
        while (transitioning) {
            if (!startedClose) {
                StartCoroutine(CloseCurtains());
                startedClose = true;
            }
            yield return null;
        }
        StartCoroutine(BeginOpeningCurtains());
    }

    IEnumerator BeginOpeningCurtains() {
        transitioning = true;
        bool startedOpen = false;
        while (transitioning) {
            if (!startedOpen) {
                StartCoroutine(OpenCurtains());
                startedOpen = true;
            }
            yield return null;
        }
        transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator OpenCurtains() {
        yield return new WaitForSeconds(1);
        float elapsedTime = 0;
        Vector3 minScale = LeftCurtain.localScale;
        Vector3 maxScale = new Vector3(0, 1, 0);
        while (LeftCurtain.localScale.x > 0) {
            LeftCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
            RightCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transitioning = false;
    }

    IEnumerator CloseCurtains() {
        float elapsedTime = 0;
        Vector3 minScale = LeftCurtain.localScale;
        Vector3 maxScale = new Vector3(1, 1, 0);
        while (LeftCurtain.localScale.x < 1) {
            LeftCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
            RightCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transitioning = false;
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
        if (PlayerAddedEvent != null) {
            StartCoroutine(Resize());
        }
    }

    IEnumerator Resize() {
        yield return new WaitForSeconds(0.1f);
        PlayerAddedEvent(m_NumPlayers);
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
        m_Players[playerID].transform.position = m_SpawnPositions[playerID];
        m_Players[playerID].enabled = true;
        m_Players[playerID].Heal(100f);
        m_AudioManager.Play("Respawn");
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
        player.EquipStartingWeapon();
    }
    #endregion

    #region Spotlight
    private void SpotlightChange(int player, int playerlost) {
        if (playerlost != NULL) {
            //Add sound here
            m_PlayerObjs[playerlost].GetComponentsInChildren<SpriteRenderer>(true)[0].gameObject.SetActive(false);
        }
        if (player == NULL) {
            return;
        }
        //Add sound here
        m_PlayerObjs[player].GetComponentsInChildren<SpriteRenderer>(true)[0].gameObject.SetActive(true);
    }
    #endregion

    #region Points
    private void PlayerPointUI(int player, float curr, int end) {
        m_Players[player].PointUI(curr, end);
    }

    private void RankPlayers(float[] playerpoints) {
        int[] sorted = new int[4];
        float highest = 0;
        int highestplayer = -1;
        for (int i = 0; i < m_NumPlayers; i++) {
            if (playerpoints[i] >= highest) {
                highest = playerpoints[i];
                highestplayer = i;
            }
        }
        playerpoints[0] = highestplayer;
        highest = 0;
        highestplayer = -1;
        for (int i = 0; i < m_NumPlayers; i++) {
            if (playerpoints[i] >= highest && i != sorted[0]) {
                highest = playerpoints[i];
                highestplayer = i;
            }
        }
        sorted[1] = highestplayer;
        highest = 0;
        highestplayer = -1;
        for (int i = 0; i < m_NumPlayers; i++) {
            if (playerpoints[i] >= highest && i != sorted[0] && i != sorted[1]) {
                highest = playerpoints[i];
                highestplayer = i;
            }
        }
        sorted[2] = highestplayer;
        highest = 0;
        highestplayer = -1;
        for (int i = 0; i < m_NumPlayers; i++) {
            if (playerpoints[i] >= highest && i != sorted[0] && i != sorted[1] && i != sorted[2]) {
                highest = playerpoints[i];
                highestplayer = i;
            }
        }
        sorted[3] = highestplayer;
        End(sorted[0], sorted[1], sorted[2], sorted[3]);
    }
    #endregion

    #region Changing Floors
    private void StartFloor(Vector3[] spawnPositions) {
        m_SpawnPositions = spawnPositions;
        ResetPlayerLocation();
    }

    private void ResetPlayerLocation(int placeholder = -1) {
        PointManager pointManager = FindObjectOfType<PointManager>();
        if (pointManager.GetSingleton().GoalReached()) {
            return;
        }

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

    private void End(int first, int second, int third, int fourth) {
        m_AudioManager.Fade("Soundtrack", 2.5f);
        StartCoroutine(EndCoroutine(first, second, third, fourth));
    }

    private IEnumerator EndCoroutine(int first, int second, int third, int fourth) {
        CallCurtainsCoroutine();
        yield return new WaitForSeconds(3f);

        m_Players[first].transform.position = new Vector3(1.54f, 1.14f, 0);
        m_PlayerObjs[first].GetComponentInChildren<Camera>().enabled = false;
        m_PlayerObjs[first].GetComponentInChildren<CapsuleCollider2D>().gameObject.SetActive(false);
        m_PlayerObjs[first].GetComponentsInChildren<Canvas>()[1].gameObject.SetActive(false);
        if (second != -1) {
            m_Players[second].transform.position = new Vector3(-2.03f, -0.17f, 0);
            m_PlayerObjs[second].GetComponentInChildren<Camera>().enabled = false;
            m_PlayerObjs[second].GetComponentInChildren<CapsuleCollider2D>().gameObject.SetActive(false);
            m_PlayerObjs[second].GetComponentsInChildren<Canvas>()[1].gameObject.SetActive(false);
        }
        if (third != -1) {
            m_Players[third].transform.position = new Vector3(5.04f, -0.66f, 0);
            m_PlayerObjs[third].GetComponentInChildren<Camera>().enabled = false;
            m_PlayerObjs[third].GetComponentInChildren<CapsuleCollider2D>().gameObject.SetActive(false);
            m_PlayerObjs[third].GetComponentsInChildren<Canvas>()[1].gameObject.SetActive(false);
        }
        if (fourth != -1) {
            m_Players[fourth].transform.position = new Vector3(-5.18f, -1.05f, 0);
            m_PlayerObjs[fourth].GetComponentInChildren<Camera>().enabled = false;
            m_PlayerObjs[fourth].GetComponentInChildren<CapsuleCollider2D>().gameObject.SetActive(false);
            m_PlayerObjs[fourth].GetComponentsInChildren<Canvas>()[1].gameObject.SetActive(false);
        }
        m_AudioManager.Play("WinMusic");
        SceneManager.LoadScene(2);
    }
    #endregion

    #region Disable/Enders
    private void OnDisable() {
        PlayerManager.DeathEvent -= Respawn;
        PointManager.GameEndEvent -= RankPlayers;
        CollisionTrigger.FloorChangeEvent -= ResetPlayerLocation;
        DungeonGenerator.DungeonLoadedEvent -= StartFloor;
        PointManager.SpotlightEvent -= SpotlightChange;
        PointManager.PointsUIEvent -= PlayerPointUI;
        PlayerManager.PauseEvent -= Pause;
        PlayerManager.SelectEvent -= Select;
    }
    #endregion
}
