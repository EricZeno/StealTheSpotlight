﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DisallowMultipleComponent]
public class DungeonGenerator : MonoBehaviour {
    #region Node Struct
    private struct Node {
        #region Private Variables
        private Dictionary<Node, int> m_SpecialRoomDistances;
        #endregion

        #region Constructor
        public Node(int row, int col) {
            Row = row;
            Col = col;
            m_SpecialRoomDistances = new Dictionary<Node, int>();
        }

        public Node(int row, int col, List<Node> specialRooms) {
            Row = row;
            Col = col;
            m_SpecialRoomDistances = new Dictionary<Node, int>();
            CalculateSpecialRoomDistances(specialRooms);
        }
        #endregion

        #region Accessors and Editors
        public int Row { get; }
        public int Col { get; }

        public int MinDistance() {
            Dictionary<Node, int>.ValueCollection values = m_SpecialRoomDistances.Values;
            if (values.Count == 0) {
                return 0;
            }
            return m_SpecialRoomDistances.Values.Min();
        }

        public void RemoveSpecialRoom(Node specialRoom) {
            m_SpecialRoomDistances.Remove(specialRoom);
        }
        #endregion

        #region Helper Methods
        private void CalculateSpecialRoomDistances(List<Node> specialRooms) {
            foreach (Node specialRoom in specialRooms) {
                int distance = ManhattanDistance(this, specialRoom);
                m_SpecialRoomDistances.Add(specialRoom, distance);
            }
        }

        private int ManhattanDistance(Node node, Node other) {
            return Mathf.Abs(node.Row - other.Row) + Mathf.Abs(node.Col - other.Col);
        }
        #endregion
    }
    #endregion

    #region Constants
    #region Misc.
    private const int DUNGEON_SIZE = 7;
    private const int NUM_OBJECTIVES = 2;
    private const float VERTICAL_ROOM_SIZE = 28;
    private const float HORIZONTAL_ROOM_SIZE = 36;
    private const float MIN_ROOM_DENSITY_PERC = .12f;
    private const float MAX_ROOM_DENSITY_PERC = .19f;
    private const float ROW_MULTIPLIER = 5.2f;
    private const float COL_MULTIPLIER = 3299.653f;
    #endregion

    #region Room IDs
    private const int NO_ROOM_ID = 0;
    private const int SPAWN_ID = 1;
    private const int BOSS_ID = 2;
    private const int SHOP_ID = 3;
    private const int OBJECTIVE_ID = 4;
    private const int GENERIC_ROOM_ID = 5;
    private const int POSSIBLE_ROOM_ID = 6;
    private const int CONNECTED_SPECIAL_ROOM_MOD = 50;
    #endregion

    #region Placement Rules
    private const int SPAWN_ROW = 0;
    private const int MIN_SPAWN_COL = 2;
    private const int MAX_SPAWN_COL = 5;

    private const int MIN_BOSS_ROW = 5;
    private const int MAX_BOSS_ROW = 6;

    private const int MIN_SHOP_ROW = 0;
    private const int MAX_SHOP_ROW = 5;

    private const int MIN_OBJECTIVE_ROW = 0;
    private const int MAX_OBJECTIVE_ROW = 6;
    #endregion
    #endregion

    #region Events and Delegates
    public delegate void DungeonLoaded(Vector3[] spawnPostions);
    public static event DungeonLoaded DungeonLoadedEvent;
    #endregion

    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("All possible spawn room designs")]
    private List<GameObject> m_SpawnRooms;

    [SerializeField]
    [Tooltip("All possible boss room designs")]
    private List<GameObject> m_BossRooms;

    [SerializeField]
    [Tooltip("All possible shop designs")]
    private List<GameObject> m_Shops;

    [SerializeField]
    [Tooltip("All possible objective designs")]
    private List<GameObject> m_ObjectiveRooms;

    [SerializeField]
    [Tooltip("All possible generic room designs")]
    private List<GameObject> m_GenericRooms;

    [SerializeField]
    [Tooltip("The position of the bottom left room in the dungeon")]
    private Vector3 m_InitialPosition;

    [SerializeField]
    [Tooltip("Horizontal hallway")]
    private GameObject m_HorizontalHallway;

    [SerializeField]
    [Tooltip("Vertical hallway")]
    private GameObject m_VerticalHallway;

    [SerializeField]
    [Tooltip("Top wall")]
    private GameObject m_TopWall;

    [SerializeField]
    [Tooltip("Right wall")]
    private GameObject m_RightWall;

    [SerializeField]
    [Tooltip("Bottom wall")]
    private GameObject m_BottomWall;

    [SerializeField]
    [Tooltip("Left wall")]
    private GameObject m_LeftWall;

    [SerializeField]
    [Tooltip("Top door")]
    private GameObject m_TopDoor;

    [SerializeField]
    [Tooltip("Right door")]
    private GameObject m_RightDoor;

    [SerializeField]
    [Tooltip("Bottom door")]
    private GameObject m_BottomDoor;

    [SerializeField]
    [Tooltip("Left wall")]
    private GameObject m_LeftDoor;
    #endregion

    #region Private Variable
    private int[,] m_Dungeon;
    private System.Random m_Random;
    private List<Node> m_SpecialRooms;
    private List<Node> m_PossibleRooms;
    private Dictionary<int, List<Node>> m_MinDistances;
    private int m_NumRooms;
    private Dictionary<float, GameObject> m_RoomMap;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_Dungeon = new int[DUNGEON_SIZE, DUNGEON_SIZE];
        m_Random = new System.Random();
        m_SpecialRooms = new List<Node>();
        m_PossibleRooms = new List<Node>();
        m_MinDistances = new Dictionary<int, List<Node>>();
        m_NumRooms = 0;
        m_RoomMap = new Dictionary<float, GameObject>();
    }

    private void Start() {
        GenerateDungeon();
    }
    #endregion

    #region Generation
    private void GenerateDungeon() {
        Node spawn = PlaceSpawn();
        PlaceSpecialRooms();
        ConnectSpecialRooms(spawn);
        PlaceRandomRooms();
        InstantiateDungeon();
        InstantiateDoorsHallwaysAndWalls();
    }

    #region Special Room Placement
    private Node PlaceSpawn() {
        int spawnColumn = m_Random.Next(MIN_SPAWN_COL, MAX_SPAWN_COL);
        m_Dungeon[SPAWN_ROW, spawnColumn] = SPAWN_ID;
        m_NumRooms++;
        return new Node(SPAWN_ROW, spawnColumn);
    }

    private void PlaceSpecialRooms() {
        PlaceBossRoom();
        PlaceShop();
        PlaceObjectiveRooms();
    }

    private void PlaceBossRoom() {
        bool validPlacement = false;
        int bossRow = 0;
        int bossCol = 0;

        while (!validPlacement) {
            bossRow = m_Random.Next(MIN_BOSS_ROW, MAX_BOSS_ROW + 1);
            bossCol = m_Random.Next(0, DUNGEON_SIZE);

            if (m_Dungeon[bossRow, bossCol] == NO_ROOM_ID) {
                validPlacement = true;
            }
        }

        m_Dungeon[bossRow, bossCol] = BOSS_ID;
        m_SpecialRooms.Add(new Node(bossRow, bossCol));
        m_NumRooms++;
    }

    private void PlaceShop() {
        bool validPlacement = false;
        int shopRow = 0;
        int shopCol = 0;

        while (!validPlacement) {
            shopRow = m_Random.Next(MIN_SHOP_ROW, MAX_SHOP_ROW + 1);
            shopCol = m_Random.Next(0, DUNGEON_SIZE);

            if (m_Dungeon[shopRow, shopCol] == NO_ROOM_ID) {
                validPlacement = true;
            }
        }

        m_Dungeon[shopRow, shopCol] = SHOP_ID;
        m_SpecialRooms.Add(new Node(shopRow, shopCol));
        m_NumRooms++;
    }

    private void PlaceObjectiveRooms() {
        for (int i = 0; i < NUM_OBJECTIVES; i++) {
            bool validPlacement = false;
            int objRow = 0;
            int objCol = 0;

            while (!validPlacement) {
                objRow = m_Random.Next(MIN_OBJECTIVE_ROW, MAX_OBJECTIVE_ROW + 1);
                objCol = m_Random.Next(0, DUNGEON_SIZE);

                if (m_Dungeon[objRow, objCol] == NO_ROOM_ID) {
                    validPlacement = true;
                }
            }

            m_Dungeon[objRow, objCol] = OBJECTIVE_ID;
            m_SpecialRooms.Add(new Node(objRow, objCol));
            m_NumRooms++;
        }
    }
    #endregion

    #region Generic Room Placement
    private void ConnectSpecialRooms(Node spawn) {
        AddNeighbors(spawn);

        while (m_SpecialRooms.Count > 0) {
            Dictionary<int, List<Node>>.KeyCollection keys = m_MinDistances.Keys;
            if (keys.Count == 0) {
                throw new System.AccessViolationException("Empty Min Distances");
            }

            int minDist = keys.Min();
            int chosenIndex = m_Random.Next(0, m_MinDistances[minDist].Count);
            Node chosenRoom = m_MinDistances[minDist][chosenIndex];

            m_Dungeon[chosenRoom.Row, chosenRoom.Col] = GENERIC_ROOM_ID;
            m_PossibleRooms.Remove(chosenRoom);
            m_MinDistances[minDist].Remove(chosenRoom);

            if (m_MinDistances[minDist].Count == 0) {
                m_MinDistances.Remove(minDist);
            }

            m_NumRooms++;
            AddNeighbors(chosenRoom);
        }
    }

    private void PlaceRandomRooms() {
        float roomDensityPerc = (float)(m_Random.NextDouble() *
            (MAX_ROOM_DENSITY_PERC - MIN_ROOM_DENSITY_PERC) + MIN_ROOM_DENSITY_PERC);

        int maxRooms = (int)(roomDensityPerc * (DUNGEON_SIZE * DUNGEON_SIZE));

        while (m_NumRooms < maxRooms) {
            int chosenIndex = m_Random.Next(0, m_PossibleRooms.Count);
            Node chosenRoom = m_PossibleRooms[chosenIndex];

            m_Dungeon[chosenRoom.Row, chosenRoom.Col] = GENERIC_ROOM_ID;
            m_PossibleRooms.Remove(chosenRoom);
            m_NumRooms++;
            AddNeighbors(chosenRoom);
        }
    }

    #region Generic Room Placement Helper Methods
    private void AddNeighbors(Node chosenRoom) {
        int row = chosenRoom.Row;
        int col = chosenRoom.Col;
        int nodeCheck;

        if (col != 0) {
            nodeCheck = m_Dungeon[row, col - 1];
            if (nodeCheck == NO_ROOM_ID) {
                AddPossibleRoom(row, col - 1);
            }
            else if (IsUnconnectedSpecialRoom(nodeCheck)) {
                ConnectSpecialRoom(row, col - 1);
            }
        }

        if (col != DUNGEON_SIZE - 1) {
            nodeCheck = m_Dungeon[row, col + 1];
            if (nodeCheck == NO_ROOM_ID) {
                AddPossibleRoom(row, col + 1);
            }
            else if (IsUnconnectedSpecialRoom(nodeCheck)) {
                ConnectSpecialRoom(row, col + 1);
            }
        }

        if (row != 0) {
            nodeCheck = m_Dungeon[row - 1, col];
            if (nodeCheck == NO_ROOM_ID) {
                AddPossibleRoom(row - 1, col);
            }
            else if (IsUnconnectedSpecialRoom(nodeCheck)) {
                ConnectSpecialRoom(row - 1, col);
            }
        }

        if (row != DUNGEON_SIZE - 1) {
            nodeCheck = m_Dungeon[row + 1, col];
            if (nodeCheck == NO_ROOM_ID) {
                AddPossibleRoom(row + 1, col);
            }
            else if (IsUnconnectedSpecialRoom(nodeCheck)) {
                ConnectSpecialRoom(row + 1, col);
            }
        }
    }

    private bool IsUnconnectedSpecialRoom(int roomID) {
        switch (roomID) {
            case BOSS_ID:
            case SHOP_ID:
            case OBJECTIVE_ID:
                return true;
            default:
                return false;
        }
    }

    private void ConnectSpecialRoom(int connectedRow, int connectedCol) {
        m_Dungeon[connectedRow, connectedCol] += CONNECTED_SPECIAL_ROOM_MOD;

        for (int i = 0; i < m_SpecialRooms.Count; i++) {
            Node specialRoom = m_SpecialRooms[i];

            if (specialRoom.Row == connectedRow && specialRoom.Col == connectedCol) {
                m_SpecialRooms.Remove(specialRoom);

                foreach (Node possibleRoom in m_PossibleRooms) {
                    int oldDistance = possibleRoom.MinDistance();
                    possibleRoom.RemoveSpecialRoom(specialRoom);
                    int newDistance = possibleRoom.MinDistance();

                    if (oldDistance != newDistance) {
                        m_MinDistances[oldDistance].Remove(possibleRoom);

                        if (m_MinDistances[oldDistance].Count == 0) {
                            m_MinDistances.Remove(oldDistance);
                        }

                        UpdateMinDistances(possibleRoom);
                    }
                }
            }
        }
    }

    private void AddPossibleRoom(int row, int col) {
        m_Dungeon[row, col] = POSSIBLE_ROOM_ID;
        Node possibleRoom = new Node(row, col, m_SpecialRooms);
        m_PossibleRooms.Add(possibleRoom);
        UpdateMinDistances(possibleRoom);
    }

    private void UpdateMinDistances(Node possibleRoom) {
        int minDistance = possibleRoom.MinDistance();
        if (m_MinDistances.ContainsKey(minDistance)) {
            m_MinDistances[minDistance].Add(possibleRoom);
        }
        else {
            List<Node> distList = new List<Node> { possibleRoom };
            m_MinDistances[minDistance] = distList;
        }
    }
    #endregion
    #endregion

    #region Door/Hallway/Wall Placement
    private void InstantiateDoorsHallwaysAndWalls() {
        for (int row = DUNGEON_SIZE - 1; row >= 0; row--) {
            for (int col = 0; col < DUNGEON_SIZE; col++) {
                // Check if there is a room
                if (m_Dungeon[row, col] != NO_ROOM_ID && m_Dungeon[row, col] != POSSIBLE_ROOM_ID) {
                    // Check if there is a room to the right; if so, instantiate a horizontal hallway; if not, instatiate a wall
                    if (col < DUNGEON_SIZE - 1 && m_Dungeon[row, col + 1] != NO_ROOM_ID && m_Dungeon[row, col + 1] != POSSIBLE_ROOM_ID) {
                        Instantiate(m_HorizontalHallway, RoomPosition(row, col), transform.rotation);
                        GameObject RightDoor = Instantiate(m_RightDoor, DoorPosition(row, col, m_RightDoor), transform.rotation);
                        GameObject LeftDoor = Instantiate(m_LeftDoor, DoorPosition(row, col + 1, m_LeftDoor), transform.rotation);

                        List<GameObject> doors = new List<GameObject> { RightDoor, LeftDoor };

                        float room1 = row * ROW_MULTIPLIER + col * COL_MULTIPLIER;
                        m_RoomMap[room1].GetComponent<Room>().AddDoors(doors);
                        float room2 = row * ROW_MULTIPLIER + (col + 1) * COL_MULTIPLIER;
                        m_RoomMap[room2].GetComponent<Room>().AddDoors(doors);
                    }
                    else {
                        Instantiate(m_RightWall, RoomPosition(row, col), transform.rotation);
                    }
                    // Check if there is a room to the top; if so, instantiate a vertical hallway; if not, instantiate a wall
                    if (row < DUNGEON_SIZE - 1 && m_Dungeon[row + 1, col] != NO_ROOM_ID && m_Dungeon[row + 1, col] != POSSIBLE_ROOM_ID) {
                        Instantiate(m_VerticalHallway, RoomPosition(row, col), transform.rotation);
                        GameObject TopDoor = Instantiate(m_TopDoor, DoorPosition(row, col, m_TopDoor), transform.rotation);
                        GameObject BottomDoor = Instantiate(m_BottomDoor, DoorPosition(row + 1, col, m_BottomDoor), transform.rotation);

                        List<GameObject> doors = new List<GameObject> { TopDoor, BottomDoor };

                        float room1 = row * ROW_MULTIPLIER + col * COL_MULTIPLIER;
                        m_RoomMap[room1].GetComponent<Room>().AddDoors(doors);
                        float room2 = (row + 1) * ROW_MULTIPLIER + col * COL_MULTIPLIER;
                        m_RoomMap[room2].GetComponent<Room>().AddDoors(doors);
                    }
                    else {
                        Instantiate(m_TopWall, RoomPosition(row, col), transform.rotation);
                    }
                    // Check if there is a room to the bottom; if not, instantiate a wall
                    if (row == 0 || m_Dungeon[row - 1, col] == NO_ROOM_ID || m_Dungeon[row - 1, col] == POSSIBLE_ROOM_ID) {
                        Instantiate(m_BottomWall, RoomPosition(row, col), transform.rotation);
                    }
                    // Check if there is a room to the left if not, instantiate a wall
                    if (col == 0 || m_Dungeon[row, col - 1] == NO_ROOM_ID || m_Dungeon[row, col - 1] == POSSIBLE_ROOM_ID) {
                        Instantiate(m_LeftWall, RoomPosition(row, col), transform.rotation);
                    }
                }
            }
        }
    }

    private Vector3 DoorPosition(int row, int col, GameObject door) {
        return RoomPosition(row, col) + door.transform.position;
    }
    #endregion

    #region Instantiation
    private void InstantiateDungeon() {
        Vector3[] playerSpawns = null;
        for (int row = 0; row < DUNGEON_SIZE; row++) {
            for (int col = 0; col < DUNGEON_SIZE; col++) {
                float roomMapID = row * ROW_MULTIPLIER + col * COL_MULTIPLIER;
                GameObject room;
                switch (m_Dungeon[row, col]) {
                    case SPAWN_ID:
                        int chosenIndex = m_Random.Next(0, m_SpawnRooms.Count);
                        GameObject spawn = m_SpawnRooms[chosenIndex];
                        Vector3 spawnPos = RoomPosition(row, col);
                        room = Instantiate(spawn, spawnPos, transform.rotation);
                        playerSpawns = CalculatePlayerSpawns(spawn, spawnPos);
                        m_RoomMap.Add(roomMapID, room);
                        break;
                    case BOSS_ID + CONNECTED_SPECIAL_ROOM_MOD:
                        chosenIndex = m_Random.Next(0, m_BossRooms.Count);
                        GameObject bossRoom = m_BossRooms[chosenIndex];
                        room = Instantiate(bossRoom, RoomPosition(row, col), transform.rotation);
                        m_RoomMap.Add(roomMapID, room);
                        break;
                    case SHOP_ID + CONNECTED_SPECIAL_ROOM_MOD:
                        chosenIndex = m_Random.Next(0, m_Shops.Count);
                        GameObject shop = m_Shops[chosenIndex];
                        room = Instantiate(shop, RoomPosition(row, col), transform.rotation);
                        m_RoomMap.Add(roomMapID, room);
                        break;
                    case OBJECTIVE_ID + CONNECTED_SPECIAL_ROOM_MOD:
                        chosenIndex = m_Random.Next(0, m_ObjectiveRooms.Count);
                        GameObject objectiveRoom = m_ObjectiveRooms[chosenIndex];
                        room = Instantiate(objectiveRoom, RoomPosition(row, col), transform.rotation);
                        m_RoomMap.Add(roomMapID, room);
                        break;
                    case GENERIC_ROOM_ID:
                        chosenIndex = m_Random.Next(0, m_GenericRooms.Count);
                        GameObject genericRoom = m_GenericRooms[chosenIndex];
                        room = Instantiate(genericRoom, RoomPosition(row, col), transform.rotation);
                        m_RoomMap.Add(roomMapID, room);
                        break;
                }
            }
        }

        if (playerSpawns == null) {
            throw new System.EntryPointNotFoundException("Spawn not instantiated");
        }

        DungeonLoadedEvent(playerSpawns);
    }

    private Vector3 RoomPosition(int row, int col) {
        float newX = m_InitialPosition.x + col * HORIZONTAL_ROOM_SIZE;
        float newY = m_InitialPosition.y + row * VERTICAL_ROOM_SIZE;
        return new Vector3(newX, newY);
    }

    private Vector3[] CalculatePlayerSpawns(GameObject spawn, Vector3 spawnPos) {
        Vector3[] playerSpawns = new Vector3[Consts.MAX_NUM_PLAYERS];
        Vector3[] spawnPresets = spawn.GetComponent<SpawnRoom>().GetSpawnLocations();

        System.Array.Copy(spawnPresets, playerSpawns, spawnPresets.Length);

        for (int i = 0; i < playerSpawns.Length; i++) {
            playerSpawns[i] += spawnPos;
        }

        return playerSpawns;
    }
    #endregion
    #endregion
}
