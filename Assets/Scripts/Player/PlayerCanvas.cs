using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour {
    #region Private Variables
    private int m_CurrHighlighted;
    private int m_PlayerID;
    #endregion

    #region Editor Variables
    [SerializeField]
    private Image m_DeathPanel;

    [SerializeField]
    private Text m_CountdownText;

    [SerializeField]
    [Tooltip("The GameObject that holds all inventory UI")]
    private GameObject m_InventoryParent;

    [SerializeField]
    [Tooltip("The parent object that holds all inventory wedges")]
    private GameObject wedgeParent;
    #endregion

    #region Initialization

    private void OnEnable() {
        PlayerManager.DeathEvent += EnableDeathCanvas;
    }
    #endregion

    #region Accessors and Setters
    public int PlayerID {
        get { return m_PlayerID; }
        set { m_PlayerID = value; }
    }

    public int CurrHighlighted {
        get { return m_CurrHighlighted; }
        set { CurrHighlighted = value;}
    }
    #endregion

    #region InventoryUI
    private void EnableInventoryUI(int playerID) {
        if (m_InventoryParent != null && playerID == m_PlayerID) {
            m_InventoryParent.SetActive(true); 
        }
    }

    private void DisableInventoryUI(int playerID) {
        if (m_InventoryParent != null && playerID == m_PlayerID) {
            m_InventoryParent.SetActive(false);
        }
    }

    private void DisplayFlavorText() {
        
    }

    public void HighlightWedge(int wedgeNumber) {
        GameObject selectedWedge = wedgeParent.transform.GetChild(wedgeNumber).gameObject;
        GameObject previousWedge = wedgeParent.transform.GetChild(m_CurrHighlighted).gameObject;
       // previousWedge.GetComponent<Image> = ;
        //selectedWedge.GetComponent<Image> = ;
        m_CurrHighlighted = wedgeNumber;
        
    }

    #endregion

    #region Respawn
    private void EnableDeathCanvas (int playerID, int respawnTime) {
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
        PlayerManager.DeathEvent -= EnableDeathCanvas;
    }
    #endregion
}
