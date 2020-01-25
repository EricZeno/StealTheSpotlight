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
            }
            else {
                SceneChangeEvent();
                SceneManager.LoadScene(m_Level);
            }
        }
    }
    #endregion
}
