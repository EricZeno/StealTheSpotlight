using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerCanvas : MonoBehaviour {
    #region Variables
    #region Editor Variables
    //Combat UI
    [SerializeField]
    [Tooltip("Image overlay for death countdown")]
    private Image m_DeathPanel;

    [SerializeField]
    [Tooltip("Text field for death countdown")]
    private Text m_CountdownText;

    [SerializeField]
    [Tooltip("Health Slider UI element")]
    private Slider m_HealthSlider;

    //Inventory UI
    [SerializeField]
    [Tooltip("The GameObject that holds all inventory UI")]
    private RectTransform m_InventoryParent;

    [SerializeField]
    [Tooltip("The parent object that holds all inventory wedges")]
    private RectTransform m_WedgeParent;

    [SerializeField]
    [Tooltip("Unhighlighted Wedge Image")]
    private Sprite m_WedgeUnHighlighted;

    [SerializeField]
    [Tooltip("Highlighted Wedge Image")]
    private Sprite m_WedgeHighlighted;

    [SerializeField]
    [Tooltip("Text field for item descriptions")]
    private Text m_InventoryText;
    #endregion

    #region Private Variables
    private int m_CurrHighlighted = 0;
    private int m_PlayerID;
    private Inventory m_Inventory;
    private int[] m_InventoryTracker;
    private Image[] m_WedgeImageArray;
    private Image[] m_InventoryImage;
    private const int m_InventorySize = 8;
    private int m_MaxHealth;
    #endregion
    #endregion

    #region Initialization
    private void OnEnable() {
        PlayerManager.DeathEvent += EnableDeathCanvas;
        m_WedgeImageArray = new Image[m_InventorySize];
        m_InventoryImage = new Image[m_InventorySize];
        for (int i = 0; i < m_InventorySize; i++) {
            m_WedgeImageArray[i] = m_WedgeParent.GetChild(i).GetComponent<Image>();
            m_InventoryImage[i] = m_WedgeParent.GetChild(i).GetChild(0).GetComponent<Image>();
        }
    }
    #endregion

    #region Accessors and Setters
    public int PlayerID {
        get { return m_PlayerID; }
        set { m_PlayerID = value; }
    }
    #endregion

    #region OnDisable and other Enders
    private void OnDisable() {
        PlayerManager.DeathEvent -= EnableDeathCanvas;
    }
    #endregion

    #region InventoryUI
    public void EnableInventoryUI(int playerID, Inventory inventory) {
        if (m_InventoryParent != null && playerID == m_PlayerID) {
            m_InventoryParent.gameObject.SetActive(true);
            m_Inventory = inventory;
            m_InventoryTracker = inventory.GetInventoryList();
            ClearImages();
            SetInventoryImages(m_InventoryTracker);
            HighlightWedge(0);
        }
    }

    public void DisableInventoryUI(int playerID) {
        if (m_InventoryParent != null && playerID == m_PlayerID) {
            DehighlightWedges();
            m_CurrHighlighted = 0;
            ClearImages();
            m_InventoryParent.gameObject.SetActive(false);
        }
    }

    private void DisplayFlavorText(int wedgeNumber) {
        int itemID = m_InventoryTracker[wedgeNumber];
        if (itemID != Consts.NULL_ITEM_ID) {
            string itemText = ItemManager.GetItem(m_InventoryTracker[wedgeNumber]).GetDescription();
            m_InventoryText.text = itemText;
        }
        else {
            m_InventoryText.text = "";
        }
    }

    private void SetInventoryImages(int[] itemList) {
        for (int i = 0; i < m_Inventory.GetInventoryCapacity(); i++) {
            int itemID = itemList[i];
            if (itemID != Consts.NULL_ITEM_ID) {
                m_InventoryImage[i].sprite = ItemManager.GetItem(itemID).GetIcon();
                m_InventoryImage[i].enabled = true;
            }
        }
    }

    public void DropSelected(int wedgeNumber) {
        m_InventoryImage[wedgeNumber].enabled = false;
    }

    private void ClearImages() {
        for (int i = 0; i < m_InventorySize; i++) {
            m_InventoryImage[i].enabled = false;
        }
    }

    public void ClearImageAtIndex(int index) {
        m_InventoryImage[index].enabled = false;
    }

    public void HighlightWedge(int wedgeNumber) {
        if (wedgeNumber >= m_Inventory.GetInventoryCapacity() || wedgeNumber < 0) {
            throw new System.IndexOutOfRangeException($"Tried to highlight wedge at index {wedgeNumber}.");
        }
        m_WedgeImageArray[m_CurrHighlighted].sprite = m_WedgeUnHighlighted;
        m_WedgeImageArray[wedgeNumber].sprite = m_WedgeHighlighted;
        DisplayFlavorText(wedgeNumber);
        m_CurrHighlighted = wedgeNumber;
    }

    public void DehighlightWedges() {
        m_WedgeImageArray[m_CurrHighlighted].sprite = m_WedgeUnHighlighted;
    }
    #endregion

    #region CombatUI
    public void SetSlider(int maxHealth) {
        m_HealthSlider.maxValue = maxHealth;
        m_HealthSlider.value = maxHealth;
        m_MaxHealth = maxHealth;
    }

    public void SliderDamage(int damage) {
        m_HealthSlider.value -= damage;
    }

    public void SliderHeal(int heal) {
        m_HealthSlider.value += heal;
    }
    #endregion

    #region Respawn
    private void EnableDeathCanvas(int playerID, int respawnTime) {
        if (m_DeathPanel != null && playerID == m_PlayerID) {
            m_DeathPanel.gameObject.SetActive(true);
            m_CountdownText.gameObject.SetActive(true);
            DisableHealthSlider();
            DisableInventoryUI(playerID);
            StartCoroutine(TextUpdate(respawnTime));
        }
    }

    private void DisableHealthSlider() {
        m_HealthSlider.gameObject.SetActive(false);
    }

    private void EnableHealthSlider() {
        m_HealthSlider.gameObject.SetActive(true);
    }

    private void DisableDeathCanvas() {
        m_DeathPanel.gameObject.SetActive(false);
        m_CountdownText.gameObject.SetActive(false);
        SetSlider(m_MaxHealth);
        EnableHealthSlider();
    }

    private IEnumerator TextUpdate(int respawnTime) {
        for (; respawnTime > 0; respawnTime--) {
            m_CountdownText.text = respawnTime.ToString();
            yield return new WaitForSeconds(1);
        }
        DisableDeathCanvas();
    }
    #endregion
}
