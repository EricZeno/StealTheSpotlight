using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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

    private void Start()
    {
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
        m_GameManager.CallCoroutine();
        yield return new WaitForSeconds(3f);
        if (m_Level == Consts.DUNGEON_SCENE_NAME)
        {
            LoadDungeonEvent();
            SceneManager.LoadSceneAsync(m_Level);
            FloorTextEvent();
        }
        else
        {
            FloorChangeEvent(collision.GetComponent<PlayerManager>().GetID());
            SceneManager.LoadScene(m_Level);
            FloorTextEvent();
        }
    }

    //IEnumerator BeginOpening() {
    //    transitioning = true;
    //    bool startedOpen = false;
    //    while (transitioning)
    //    {
    //        if (!startedOpen)
    //        {
    //            StartCoroutine(OpenCurtains());
    //            startedOpen = true;
    //        }
    //        yield return null;
    //    }
    //    m_GameManager = FindObjectOfType<GameManager>();
    //    m_GameManager.transform.GetChild(0).gameObject.SetActive(false);
    //}

    //IEnumerator CloseCurtains() {
    //    float elapsedTime = 0;
    //    Vector3 minScale = LeftCurtain.localScale;
    //    Vector3 maxScale = new Vector3(3, 1, 0);
    //    while (LeftCurtain.localScale.x < 3) {
    //        LeftCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
    //        RightCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    transitioning = false;
    //}

    //IEnumerator OpenCurtains() {
    //    float elapsedTime = 0;
    //    Vector3 minScale = LeftCurtain.localScale;
    //    Vector3 maxScale = new Vector3(0, 1, 0);
    //    while (LeftCurtain.localScale.x > 0)
    //    {
    //        LeftCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
    //        RightCurtain.localScale = Vector3.Lerp(minScale, maxScale, elapsedTime / 3);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    transitioning = false;
    //}
    #endregion
}
