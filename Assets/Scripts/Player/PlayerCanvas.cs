using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerInventoryController))]
public class PlayerCanvas : MonoBehaviour {
    #region Interface
    public void OpenInventoryUI() {
        EnableInventoryUI();
        HighlightWedge(m_InventoryController.SelectedItemIndex);
    }

    public void CloseInventoryUI() {
        DisableInventoryUI();
    }

    public void UpdateUI() {
        SetInventoryImages();
        SelectItem();
    }

    public void DropSelectedItem() {
        int selected = m_InventoryController.SelectedItemIndex;
        DropSelected(selected);
        DisplayFlavorText(selected);
    }

    public void SelectItem() {
        HighlightWedge(m_InventoryController.SelectedItemIndex);
    }

    public void StartDeath(int respawnTime) {
        EnableDeathCanvas(respawnTime);
    }
    #endregion

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
    private int[] m_InventoryTracker;
    private Image[] m_WedgeImageArray;
    private Image[] m_InventoryImage;
    private int m_MaxHealth;
    #endregion

    #region Cached Components
    private PlayerManager m_Manager;
    private PlayerInventoryController m_InventoryController;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        SetupCachedComponents();
        SetupBasicVariables();
        SetupCachedReferences();
    }

    private void SetupCachedComponents() {
        m_Manager = GetComponent<PlayerManager>();
        m_InventoryController = GetComponent<PlayerInventoryController>();
    }

    private void SetupBasicVariables() {
        m_WedgeImageArray = new Image[m_InventoryController.MaxInventoryCapacity];
        m_InventoryImage = new Image[m_InventoryController.MaxInventoryCapacity];
    }

    private void SetupCachedReferences() {
        for (int i = 0; i < m_InventoryController.MaxInventoryCapacity; i++) {
            m_WedgeImageArray[i] = m_WedgeParent.GetChild(i).GetComponent<Image>();
            m_InventoryImage[i] = m_WedgeParent.GetChild(i).GetChild(0).GetComponent<Image>();
        }
    }
    #endregion

    #region InventoryUI
    #region Opening/Closing
    private void EnableInventoryUI() {
        m_InventoryParent.gameObject.SetActive(true);
        m_InventoryTracker = m_InventoryController.AllInventoryItems;
        ClearImages();
        SetInventoryImages();
    }

    private void DisableInventoryUI() {
        DehighlightWedges();
        ClearImages();
        m_InventoryParent.gameObject.SetActive(false);
    }

    private void ClearImages() {
        for (int i = 0; i < m_InventoryController.MaxInventoryCapacity; i++) {
            m_InventoryImage[i].enabled = false;
        }
    }

    private void SetInventoryImages() {
        for (int i = 0; i < m_InventoryController.MaxInventoryCapacity; i++) {
            int itemID = m_InventoryTracker[i];
            if (itemID != Consts.NULL_ITEM_ID) {
                m_InventoryImage[i].sprite = ItemManager.GetItem(itemID).GetIcon();
                m_InventoryImage[i].enabled = true;
            }
        }
    }
    #endregion

    #region Selection
    private void HighlightWedge(int wedgeNumber) {
        if (wedgeNumber >= m_InventoryController.MaxInventoryCapacity || wedgeNumber < 0) {
            throw new System.IndexOutOfRangeException($"Tried to highlight wedge at index {wedgeNumber}.");
        }
        DehighlightWedges();
        m_WedgeImageArray[wedgeNumber].sprite = m_WedgeHighlighted;
        DisplayFlavorText(wedgeNumber);
        m_CurrHighlighted = wedgeNumber;
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

    private void DehighlightWedges() {
        m_WedgeImageArray[m_CurrHighlighted].sprite = m_WedgeUnHighlighted;
    }

    private void DropSelected(int wedgeNumber) {
        m_InventoryImage[wedgeNumber].enabled = false;
    }
    #endregion
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
    private void EnableDeathCanvas(int respawnTime) {
        if (m_DeathPanel != null) {
            m_DeathPanel.gameObject.SetActive(true);
            m_CountdownText.gameObject.SetActive(true);
            DisableHealthSlider();
            DisableInventoryUI();
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
