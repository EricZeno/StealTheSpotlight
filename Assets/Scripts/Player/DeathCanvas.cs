using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCanvas : MonoBehaviour {
    #region Private Variables
    private int m_PlayerID;
    #endregion

    #region Editor Variables
    [SerializeField]
    private Image m_DeathPanel;

    [SerializeField]
    private Text m_CountdownText;
    #endregion

    #region Initialization
    private void OnEnable() {
        PlayerManager.DeathEvent += EnableCanvas;
    }
    #endregion

    #region Accessors and Setters
    public int PlayerID {
        get { return m_PlayerID; }
        set { m_PlayerID = value; }
    }
    #endregion

    #region Respawn
    private void EnableCanvas (int playerID, int respawnTime) {
        if (m_DeathPanel != null && playerID == m_PlayerID) {
            m_DeathPanel.gameObject.SetActive(true);
            m_CountdownText.gameObject.SetActive(true);
            StartCoroutine(TextUpdate(respawnTime));
        }
    }

    private void DisableCanvas() {
        m_DeathPanel.gameObject.SetActive(false);
        m_CountdownText.gameObject.SetActive(false);
    }

    private IEnumerator TextUpdate(int respawnTime) {
        for (; respawnTime > 0; respawnTime--) {
            m_CountdownText.text = respawnTime.ToString();
            yield return new WaitForSeconds(1);
        }

        DisableCanvas();
    }
    #endregion

    #region OnDisable and other Enders
    private void OnDisable() {
        PlayerManager.DeathEvent -= EnableCanvas;
    }
    #endregion
}
