using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveStarter : MonoBehaviour
{
    #region Variables
    #region Private Variables
    private float m_StartTimerTotal = 3f;
    private float m_CurrentStartTimer;

    private bool m_TouchingPlayer;
    #endregion

    #region Cached Variables
    private ObjectiveManager m_ObjectiveManager;
    #endregion
    #endregion

    #region Unity Funcs
    private void Start() {
        m_ObjectiveManager = GetComponent<ObjectiveManager>();
    }

    private void Update() {
        if (!m_TouchingPlayer) {
            m_CurrentStartTimer = m_StartTimerTotal;
        } else {
            m_CurrentStartTimer -= Time.deltaTime;

            if (m_CurrentStartTimer <= 0) {
                StartObjective();
            }
        }
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            m_TouchingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            m_TouchingPlayer = false;
        }
    }
    #endregion

    #region OnEnable and OnDisable
    private void OnEnable() {
        m_CurrentStartTimer = m_StartTimerTotal;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OnDisable() {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    #endregion

    private void StartObjective() {
        m_ObjectiveManager.StartObjective();
        
        enabled = false;
    }
}
