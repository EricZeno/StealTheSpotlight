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

    #region Private Variables
    public GameManager m_GameManager;
    private RectTransform LeftCurtain;
    private RectTransform RightCurtain;
    private bool transitioning;
    #endregion

    private void Start() {
        m_GameManager = FindObjectOfType<GameManager>();
        LeftCurtain = m_GameManager.transform.GetChild(0).GetChild(4).GetComponent<RectTransform>();
        RightCurtain = m_GameManager.transform.GetChild(0).GetChild(5).GetComponent<RectTransform>();
        transitioning = false;
    }

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        m_GameManager = FindObjectOfType<GameManager>();
        //How to reference PLAYER_TAG in Consts.cs
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            StartCoroutine(BeginClosing(collision));
        }
    }

    IEnumerator BeginClosing(Collider2D collision) {
        if (SceneManager.GetActiveScene().name == Consts.DUNGEON_SCENE_NAME) {
            FloorChangeEvent(collision.GetComponent<PlayerManager>().GetID());
            PointManager pointManager = FindObjectOfType<PointManager>();
            if (pointManager.GetSingleton().GoalReached()) {
                yield break;
            }
        }

        m_GameManager.CallCurtainsCoroutine();
        yield return new WaitForSeconds(3f);
        if (m_Level == Consts.DUNGEON_SCENE_NAME) {
            SceneManager.LoadSceneAsync(m_Level);
        }
        else {
            FloorChangeEvent(collision.GetComponent<PlayerManager>().GetID());
            SceneManager.LoadScene(m_Level);
        }
    }
    #endregion
}
