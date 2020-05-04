using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionTrigger : MonoBehaviour {
    #region Events and Delegates
    public delegate void FloorChange(int playerID);
    public static event FloorChange FloorChangeEvent;
    public delegate void LoadDungeon();
    public static event LoadDungeon LoadDungeonEvent;
    public delegate void FloorText();
    public static event FloorText FloorTextEvent;
    public delegate void Objective(int player, string objective);
    public static event Objective ObjectiveEvent;
    public delegate void RoomCleared(int player);
    public static event RoomCleared RoomClearedEvent;
    #endregion

    #region Editor Variables
    [SerializeField]
    [Tooltip("The name of the level to be loaded when this trigger is touched.")]
    private string m_Level;
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        //How to reference PLAYER_TAG in Consts.cs
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            if (m_Level == Consts.DUNGEON_SCENE_NAME) {
                LoadDungeonEvent();
                SceneManager.LoadSceneAsync(m_Level);
                Debug.Log("Event was called");
                FloorTextEvent();
            }
            else {
                FloorChangeEvent(collision.GetComponent<PlayerManager>().GetID());
                SceneManager.LoadScene(m_Level);
                Debug.Log("Event was called");
                FloorTextEvent();
            }
        }
    }
    #endregion
}
