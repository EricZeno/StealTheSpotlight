using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointManager : MonoBehaviour {

    #region Events and Delegates
    public delegate void Spotlight(int player, int playerlost = DEFAULT_SPOTLIGHT);
    public static event Spotlight SpotlightEvent;
    #endregion

    #region Constant
    private const int DEFAULT_SPOTLIGHT = -1;
    private const int FLOOR_CLEARED_POINTS = 20;
    private const int PK_POINTS = 10;
    #endregion

    #region Private Variables
    private static PointManager m_Singleton;
    private float[] m_PlayersPoints;
    private int[] m_EnemiesKilled;
    private int m_SpotlightPlayer;
    #endregion

    #region Editor Variables
    [SerializeField]
    [Tooltip("Multiplier for points")]
    private float m_Multiplier;

    [SerializeField]
    [Tooltip("Point Goal for players to win")]
    private int m_PointGoal;

    [SerializeField]
    [Tooltip("Combo to get spotlight")]
    private int m_SpotlightCombo;
    #endregion

    #region Initialization
    // Start is called before the first frame update
    private void Awake() {
        if (m_Singleton != null) {
            Destroy(gameObject);
            return;
        }
        m_Singleton = this;

        m_PlayersPoints = new float[Consts.NUM_MAX_PLAYERS];
        m_EnemiesKilled = new int[Consts.NUM_MAX_PLAYERS];
        m_SpotlightPlayer = DEFAULT_SPOTLIGHT;

        DontDestroyOnLoad(this);
    }

    private void OnEnable() {
        //Event for moving to the next floor
        CollisionTrigger.FloorChangeEvent += FloorComplete;
        //Event for when a player kills another player
        PlayerManager.PKEvent += GivePK;
        //Event for when a player clears a room
        Room.MobKilledEvent += GiveMobKill;
    }
    #endregion

    #region Points
    private void GivePK(int player, int playerkilled) {
        if (m_SpotlightPlayer == playerkilled) {
            Debug.Log($"Player {player} has stolen the spotlight.");
            m_SpotlightPlayer = player;
            SpotlightEvent(player, playerkilled);
        }
        m_PlayersPoints[player] += PK_POINTS;
        if (m_PlayersPoints[player] >= m_PointGoal) {
            //Endgame
        }
    }

    private void GiveMobKill(int player, float points) {
        if (player == m_SpotlightPlayer) {
            points = points * m_Multiplier;
        }
        m_EnemiesKilled[player]++;
        m_PlayersPoints[player] += points;
        if (m_EnemiesKilled[player] == m_SpotlightCombo && m_SpotlightPlayer == DEFAULT_SPOTLIGHT) {
            Debug.Log($"Player {player} has the spotlight.");
            m_SpotlightPlayer = player;
            SpotlightEvent(player);
        }
        if (m_PlayersPoints[player] >= m_PointGoal) {
            //Endgame
        }
    }
    #endregion

    #region Floor Reset
    public void FloorComplete(int player) {
        m_PlayersPoints[player] += FLOOR_CLEARED_POINTS;
        if (m_PlayersPoints[player] >= m_PointGoal) {
            //Endgame
        }
    }
    #endregion

    #region Disable
    private void OnDisable() {
        CollisionTrigger.FloorChangeEvent -= FloorComplete;
        PlayerManager.PKEvent -= GivePK;
        Room.MobKilledEvent -= GiveMobKill;
    }
    #endregion
}
