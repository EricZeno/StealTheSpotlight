using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PointManager : MonoBehaviour {

    #region Events and Delegates
    public delegate void Spotlight(int player, int playerlost = DEFAULT_SPOTLIGHT);
    public static event Spotlight SpotlightEvent;
    public delegate void PointsUI(int player, float currpoints, int endpoints);
    public static event PointsUI PointsUIEvent;
    #endregion

    #region Constant
    private const int DEFAULT_SPOTLIGHT = -1;
    private const int FLOOR_CLEARED_POINTS = 20;
    private const int PK_POINTS = 10;
    #endregion

    #region Private Variables
    private static PointManager m_Singleton;
    private float[] m_PlayersPoints;
    private int m_SpotlightPlayer;
    private GameObject m_dropped;
    private int m_floor;
    private AudioManager m_AudioManager;
    #endregion

    #region Editor Variables
    [SerializeField]
    [Tooltip("Multiplier for points")]
    private float m_Multiplier;

    [SerializeField]
    [Tooltip("Point Goal for players to win")]
    private int m_PointGoal;

    [SerializeField]
    [Tooltip("Point to give spotlight to a player")]
    private int m_SpotlightPoint;

    [SerializeField]
    [Tooltip("This is the dropped spotlight")]
    private GameObject m_Spotlight;

    [SerializeField]
    [Tooltip("This is the text for floors")]
    private Text m_floorText;

    [SerializeField]
    [Tooltip("This is the timer for the text to fade")]
    private float m_fadetime;
    #endregion

    #region Initialization
    // Start is called before the first frame update
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

        m_PlayersPoints = new float[Consts.NUM_MAX_PLAYERS];
        m_SpotlightPlayer = DEFAULT_SPOTLIGHT;
        m_floor = 0;
        m_AudioManager = GetComponent<AudioManager>();

        DontDestroyOnLoad(this);
    }

    private void OnEnable() {
        //Event for moving to the next floor
        CollisionTrigger.FloorChangeEvent += FloorComplete;
        CollisionTrigger.FloorTextEvent += StartFloor;
        //Event for when a player kills another player
        PlayerManager.PKEvent += GivePK;
        //Event for when a player clears a room
        Room.MobKilledEvent += GiveMobKill;
        //Event for when a player dies to something that's not a player
        PlayerManager.DropSpotlightEvent += DropSpotlight;
        //Event for when a player picks up the spotlight
        DroppedSpotlight.PickUpSpotlightEvent += GiveSpotlight;
    }
    #endregion

    #region Points
    private void GiveSpotlight(int player) {
        m_SpotlightPlayer = player;
        Destroy(m_dropped);
        m_dropped = null;
        SpotlightEvent(player);
    }

    private void GivePK(int player, int playerkilled) {
        if (m_SpotlightPlayer == playerkilled) {
            m_SpotlightPlayer = player;
            SpotlightEvent(player, playerkilled);
        }
        m_PlayersPoints[player] += PK_POINTS;
        m_AudioManager.Play("PointGain");
        PointsUIEvent(player, m_PlayersPoints[player], m_PointGoal);
        if (m_PlayersPoints[player] >= m_SpotlightPoint && m_SpotlightPlayer == DEFAULT_SPOTLIGHT && m_dropped == null) {
            m_SpotlightPlayer = player;
            SpotlightEvent(player);
        }
        if (m_PlayersPoints[player] >= m_PointGoal) {
            //Endgame
        }
    }

    private void GiveMobKill(int player, float points) {
        if (player == m_SpotlightPlayer) {
            points *= m_Multiplier;
        }
        m_PlayersPoints[player] += points;
        m_AudioManager.Play("PointGain");
        PointsUIEvent(player, m_PlayersPoints[player], m_PointGoal);
        if (m_PlayersPoints[player] >= m_SpotlightPoint && m_SpotlightPlayer == DEFAULT_SPOTLIGHT && m_dropped == null) {
            m_SpotlightPlayer = player;
            SpotlightEvent(player);
        }
        if (m_PlayersPoints[player] >= m_PointGoal) {
            //Endgame
        }
    }

    private void DropSpotlight(float x, float y) {
        if (m_SpotlightPlayer == DEFAULT_SPOTLIGHT) {
            return;
        }
        SpotlightEvent(DEFAULT_SPOTLIGHT, m_SpotlightPlayer);
        m_SpotlightPlayer = DEFAULT_SPOTLIGHT;
        m_dropped = Instantiate(m_Spotlight);
        m_dropped.transform.position = new Vector3(x, y, 0);
    }
    #endregion

    #region Floor Reset
    private void StartFloor() {
        m_floor += 1;
        m_floorText.text = $"Floor {m_floor}";
        m_floorText.color = new Color(1, 1, 1, 1);
        StartCoroutine(FadeOut(m_fadetime));
    }

    private void FloorComplete(int player) {
        m_PlayersPoints[player] += FLOOR_CLEARED_POINTS;
        m_AudioManager.Play("PointGain");
        m_AudioManager.Play("Win1");
        if (m_PlayersPoints[player] >= m_PointGoal) {
            //Endgame
        }
    }

    IEnumerator FadeOut(float lerpTime) {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true) {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(1, 0, percentageComplete);

            m_floorText.color = new Color(1, 1, 1, currentValue);

            if (percentageComplete >= 1) {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region Disable
    private void OnDisable() {
        CollisionTrigger.FloorChangeEvent -= FloorComplete;
        CollisionTrigger.LoadDungeonEvent -= StartFloor;
        PlayerManager.PKEvent -= GivePK;
        Room.MobKilledEvent -= GiveMobKill;
        PlayerManager.DropSpotlightEvent -= DropSpotlight;
    }
    #endregion
}
