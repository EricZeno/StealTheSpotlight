using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionTrigger : MonoBehaviour {
    #region Events and Delegates
    public delegate void SceneChange();
    public static event SceneChange SceneChangeEvent;
    public delegate void LoadDungeon();
    public static event LoadDungeon LoadDungeonEvent;
    public delegate void Objective(int player, string objective);
    public static event Objective ObjectiveEvent;
    public delegate void RoomCleared(int player);
    public static event RoomCleared RoomClearedEvent;
    #endregion

    #region Editor Variables
    [SerializeField]
    [Tooltip("The name of the level to be loaded when this trigger is touched.")]
    private string m_Level;
    [SerializeField]
    private int chooser;
    //0 - Scene Change
    //1 - Boss
    //2 - Obj Room
    //3 - PK
    //4 - Room Cleared
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        //How to reference PLAYER_TAG in Consts.cs
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            if (m_Level == Consts.DUNGEON_SCENE_NAME) {
                LoadDungeonEvent();
                SceneManager.LoadSceneAsync(m_Level);
            }
            else {
                switch (chooser) {
                    case 0:
                        SceneChangeEvent();
                        SceneManager.LoadScene(m_Level);
                        break;
                    case 1:
                        ObjectiveEvent(collision.gameObject.GetComponent<PlayerManager>().GetID(), Consts.BOSS_NAME);
                        break;
                    case 2:
                        ObjectiveEvent(collision.gameObject.GetComponent<PlayerManager>().GetID(), Consts.OBJROOM_NAME);
                        break;
                    case 4:
                        RoomClearedEvent(collision.gameObject.GetComponent<PlayerManager>().GetID());
                        break;
                }
            }
        }
    }
    #endregion
}
