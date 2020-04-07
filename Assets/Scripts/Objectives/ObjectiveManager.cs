using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("Objective to start")]
    private GameObject m_Objective;
    #endregion

    #region Private Variables
    private List<int> m_Players;
    #endregion

    #region Delegates
    public delegate void Objective(int player, string objective);
    public static event Objective ObjectiveEvent;
    #endregion

    #region Cached Components
    private Room m_Room;
    #endregion
    #endregion

    private void Start() {
        m_Room = GetComponentInParent<Room>();
    }

    #region Objective State Funcs
    public void StartObjective() {
        Instantiate(m_Objective, transform);

        FindPlayers();
    }

    public void FailObjective() {
        foreach (int player in m_Players) {
            GameManager.GetSingleton().getPlayer(player).Die();
        }

        ResetRoom();
    }
    #endregion

    private void FindPlayers() {
        List<PlayerManager> playerManagers = m_Room.GetPlayers();
        m_Players = new List<int>();

        foreach (PlayerManager player in playerManagers) {
            m_Players.Add(player.GetID());
        }
    }

    private void ResetRoom() {
        GetComponent<ObjectiveStarter>().enabled = true;
    }
}
