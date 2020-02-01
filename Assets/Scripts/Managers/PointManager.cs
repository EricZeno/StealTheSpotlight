using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct PointObjective {

    #region Private Variables
    private string m_objtype;
    private string m_medalplace;
    private int m_pointvalue;
    #endregion

    #region Constructor
    public PointObjective(string type, int points, string place = "") {
        m_objtype = type;
        m_medalplace = place;
        m_pointvalue = points;
    }
    #endregion

    #region Accessors
    public string getObjType() {
        return m_objtype;
    }

    public string getMedalPlace() {
        return m_medalplace;
    }

    public int getPointValue() {
        return m_pointvalue;
    }
    #endregion
}

public class PointManager : MonoBehaviour {

    #region Private Variables
    private static Dictionary<string, PointObjective> m_Objectives;
    private static PointManager m_Singleton;
    private int[] m_PlayersPoints;
    private int[] m_PlayersKills;
    private int[] m_PlayersRoomsCleared;
    private int[] m_PlayersRemainingGold;
    private List<PointObjective>[] m_PlayersObjectives;
    #endregion

    #region Initialization
    // Start is called before the first frame update
    private void Awake() {
        if (m_Singleton != null) {
            Destroy(gameObject);
            return;
        }
        m_Singleton = this;

        m_Objectives = new Dictionary<string, PointObjective>();
        m_PlayersPoints = new int[Consts.NUM_MAX_PLAYERS];
        m_PlayersKills = new int[Consts.NUM_MAX_PLAYERS];
        m_PlayersRoomsCleared = new int[Consts.NUM_MAX_PLAYERS];
        m_PlayersRemainingGold = new int[Consts.NUM_MAX_PLAYERS];
        m_PlayersObjectives = new List<PointObjective>[Consts.NUM_MAX_PLAYERS];

        for (int i = 0; i < Consts.NUM_MAX_PLAYERS; i++) {
            m_PlayersObjectives[i] = new List<PointObjective>();
        }

        CreateObjectives();

        DontDestroyOnLoad(this);
    }

    private void OnEnable() {
        //Event for moving to the next floor
        CollisionTrigger.LoadDungeonEvent += FloorComplete;
        //Event for when an objective is triggered
        ObjectiveManager.ObjectiveEvent += GiveObjective;
        //Event for when a player kills another player
        PlayerManager.PKEvent += GivePK;
        //Event for when a player clears a room
        Room.RoomClearedEvent += GiveRooms;
    }
    #endregion

    #region Objectives
    private void CreateObjectives() {
        PointObjective boss = new PointObjective(Consts.BOSS_OBJECTIVE_TYPE, Consts.BOSS_OBJECTIVE_POINTS);
        PointObjective obj = new PointObjective(Consts.OBJROOM_OBJECTIVE_TYPE, Consts.OBJROOM_OBJECTIVE_POINTS);
        PointObjective pk_gold = new PointObjective(Consts.MEDAL_PK_OBJECTIVE_TYPE, Consts.MEDAL_GOLD_OBJECTIVE_POINTS, Consts.MEDAL_FIRST);
        PointObjective pk_silver = new PointObjective(Consts.MEDAL_PK_OBJECTIVE_TYPE, Consts.MEDAL_SILVER_OBJECTIVE_POINTS, Consts.MEDAL_SECOND);
        PointObjective pk_bronze = new PointObjective(Consts.MEDAL_PK_OBJECTIVE_TYPE, Consts.MEDAL_BRONZE_OBJECTIVE_POINTS, Consts.MEDAL_THIRD);
        PointObjective money_gold = new PointObjective(Consts.MEDAL_MONEY_OBJECTIVE_TYPE, Consts.MEDAL_GOLD_OBJECTIVE_POINTS, Consts.MEDAL_FIRST);
        PointObjective money_silver = new PointObjective(Consts.MEDAL_MONEY_OBJECTIVE_TYPE, Consts.MEDAL_SILVER_OBJECTIVE_POINTS, Consts.MEDAL_SECOND);
        PointObjective money_bronze = new PointObjective(Consts.MEDAL_MONEY_OBJECTIVE_TYPE, Consts.MEDAL_BRONZE_OBJECTIVE_POINTS, Consts.MEDAL_THIRD);
        PointObjective rooms_gold = new PointObjective(Consts.MEDAL_ROOMS_OBJECTIVE_TYPE, Consts.MEDAL_GOLD_OBJECTIVE_POINTS, Consts.MEDAL_FIRST);
        PointObjective rooms_silver = new PointObjective(Consts.MEDAL_ROOMS_OBJECTIVE_TYPE, Consts.MEDAL_SILVER_OBJECTIVE_POINTS, Consts.MEDAL_SECOND);
        PointObjective rooms_bronze = new PointObjective(Consts.MEDAL_ROOMS_OBJECTIVE_TYPE, Consts.MEDAL_BRONZE_OBJECTIVE_POINTS, Consts.MEDAL_THIRD);

        m_Objectives.Add(Consts.BOSS_NAME, boss);
        m_Objectives.Add(Consts.OBJROOM_NAME, obj);
        m_Objectives.Add(Consts.PK_GOLD_NAME, pk_gold);
        m_Objectives.Add(Consts.PK_SILVER_NAME, pk_silver);
        m_Objectives.Add(Consts.PK_BRONZE_NAME, pk_bronze);
        m_Objectives.Add(Consts.MONEY_GOLD_NAME, money_gold);
        m_Objectives.Add(Consts.MONEY_SILVER_NAME, money_silver);
        m_Objectives.Add(Consts.MONEY_BRONZE_NAME, money_bronze);
        m_Objectives.Add(Consts.ROOMS_GOLD_NAME, rooms_gold);
        m_Objectives.Add(Consts.ROOMS_SILVER_NAME, rooms_silver);
        m_Objectives.Add(Consts.ROOMS_BRONZE_NAME, rooms_bronze);
    }
    #endregion

    #region Give Points/Obj
    private void GiveObjective(int player, string objective) {
        m_PlayersPoints[player] += m_Objectives[objective].getPointValue();
        m_PlayersObjectives[player].Add(m_Objectives[objective]);
        Debug.Log($"Player {player} has the Objective {objective}.");
    }

    private void GivePK(int player) {
        m_PlayersKills[player] += 1;
        Debug.Log($"Player {player} has {m_PlayersKills[player]} player kills.");
    }

    private void GiveRooms(int player) {
        m_PlayersRoomsCleared[player] += 1;
        Debug.Log($"Player {player} has cleared {m_PlayersRoomsCleared[player]} rooms.");
    }

    private void GiveGoldMedal(List<int> playerids, string type) {
        foreach (var playerid in playerids) {
            switch (type) {
                case Consts.MEDAL_PK_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.PK_GOLD_NAME);
                    break;
                case Consts.MEDAL_MONEY_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.MONEY_GOLD_NAME);
                    break;
                case Consts.MEDAL_ROOMS_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.ROOMS_GOLD_NAME);
                    break;
            }
        }
    }

    private void GiveSilverMedal(List<int> playerids, string type) {
        foreach (var playerid in playerids) {
            switch (type) {
                case Consts.MEDAL_PK_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.PK_SILVER_NAME);
                    break;
                case Consts.MEDAL_MONEY_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.MONEY_SILVER_NAME);
                    break;
                case Consts.MEDAL_ROOMS_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.ROOMS_SILVER_NAME);
                    break;
            }
        }
    }

    private void GiveBronzeMedal(List<int> playerids, string type) {
        foreach (var playerid in playerids) {
            switch (type) {
                case Consts.MEDAL_PK_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.PK_BRONZE_NAME);
                    break;
                case Consts.MEDAL_MONEY_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.MONEY_BRONZE_NAME);
                    break;
                case Consts.MEDAL_ROOMS_OBJECTIVE_TYPE:
                    GiveObjective(playerid, Consts.ROOMS_BRONZE_NAME);
                    break;
            }
        }
    }

    #endregion

    #region Floor Reset
    public void FloorComplete() {
        AssignMedal(m_PlayersKills, Consts.MEDAL_PK_OBJECTIVE_TYPE);
        AssignMedal(m_PlayersRoomsCleared, Consts.MEDAL_ROOMS_OBJECTIVE_TYPE);
        AssignMedal(m_PlayersRemainingGold, Consts.MEDAL_MONEY_OBJECTIVE_TYPE);
        m_PlayersKills = new int[Consts.NUM_MAX_PLAYERS];
        m_PlayersRoomsCleared = new int[Consts.NUM_MAX_PLAYERS];
        m_PlayersRemainingGold = new int[Consts.NUM_MAX_PLAYERS];

        for (int i = 0; i < Consts.NUM_MAX_PLAYERS; i++) {
            m_PlayersObjectives[i].Clear();
        }
    }

    public void AssignMedal(int[] medalscores, string type) {
        //Key is core and Value is the list of players with that score
        Dictionary<int, List<int>> playerscores = new Dictionary<int, List<int>>();

        int playerassigned = 0;
        int playeramount = GameManager.getNumPlayers();
        for (int i = 0; i < playeramount; i++) {
            if (playerscores.ContainsKey(medalscores[i])) {
                playerscores[medalscores[i]].Add(i);
            }
            else {
                List<int> score = new List<int>();
                score.Add(i);
                playerscores.Add(medalscores[i], score);
            }
        }

        if (playerassigned < playeramount) {
            List<int> goldplayers = playerscores[playerscores.Keys.Max()];
            playerscores.Remove(playerscores.Keys.Max());
            GiveGoldMedal(goldplayers, type);
            playerassigned += goldplayers.Count();
        }

        if (playerassigned < playeramount) {
            List<int> silverplayers = playerscores[playerscores.Keys.Max()];
            playerscores.Remove(playerscores.Keys.Max());
            GiveSilverMedal(silverplayers, type);
            playerassigned += silverplayers.Count();
        }
        else {
            return;
        }

        if (playerassigned < playeramount) {
            List<int> bronzeplayers = playerscores[playerscores.Keys.Max()];
            GiveBronzeMedal(bronzeplayers, type);
            playerassigned += bronzeplayers.Count();
        }
        else {
            return;
        }
    }
    #endregion

    #region Disable
    private void OnDisable() {
        CollisionTrigger.SceneChangeEvent -= FloorComplete;
        ObjectiveManager.ObjectiveEvent -= GiveObjective;
        PlayerManager.PKEvent -= GivePK;
        Room.RoomClearedEvent -= GiveRooms;
    }
    #endregion
}
