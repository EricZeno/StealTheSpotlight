using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionTrigger : MonoBehaviour {
    #region Events and Delegates
    public delegate void SceneChange();
    public static event SceneChange SceneChangeEvent;
    #endregion

    #region Editor Variables
    [SerializeField]
    [Tooltip("Each of the players currently in the game.")]
    private string level;
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        //How to reference PLAYER_TAG in Consts.cs
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            SceneChangeEvent();
            SceneManager.LoadScene(level);
        }
    }
    #endregion
}
