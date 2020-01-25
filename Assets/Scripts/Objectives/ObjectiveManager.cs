using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("Points awarded upon completion")]
    private int m_Points;

    [SerializeField]
    [Tooltip("Objective to start")]
    private GameObject m_Objective;

    [Header("Raycast for players in the room")]
    [SerializeField]
    [Tooltip("Width of circle cast")]
    private float m_CastRadius = 8f;

    [SerializeField]
    [Tooltip("Player layer")]
    private LayerMask m_Layer;
    #endregion

    #region Private Variables
    //Players in the objective room
    private List<int> m_Players;
    #endregion
    #endregion

    #region Objective State Funcs
    public void StartObjective() {
        Instantiate(m_Objective, transform);
        
        GetPlayersInRoom();
    }

    public void FailObjective() {
        foreach (int player in m_Players) {
            GameManager.GetSingleton().getPlayer(player).Die();
        }

        ResetRoom();
    }

    public void CompleteObjective() {
        Debug.Log("Objective complete");
        foreach (int player in m_Players) {
            //Add points here
        }
    }
    #endregion

    private void ResetRoom() {
        GetComponent<ObjectiveStarter>().enabled = true;
    }

    //Adds the id's of the players in the room to a list
    private void GetPlayersInRoom() {
        m_Players = new List<int>();
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 8, Vector2.zero, 360f, m_Layer);

        foreach (RaycastHit2D hit in hits) {
            m_Players.Add(hit.transform.gameObject.GetComponent<PlayerManager>().GetID());
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, 8);
    }
}
