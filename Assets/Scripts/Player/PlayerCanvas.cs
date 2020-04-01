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

    #region Constants
    private const float LeftGemsScaleOne = 2.5f;
    private const float RightGemsScaleOne = 3f;

    private const float LeftOffsetTwo = 123.46f;
    private const float LeftGemsScaleTwo = 2.1f;
    private const float RightGemsScaleTwo = 2.52f;

    private const float LeftOffsetThree = 100f;
    private const float LeftGemsScaleThree = 1.75f;
    private const float RightGemsScaleThree = 2.1f;
    #endregion

    #region Variables
    #region Editor Variables
    //Combat and Health UI
    [SerializeField]
    [Tooltip("Image overlay for death countdown")]
    private Image m_DeathPanel;

    [SerializeField]
    [Tooltip("Text field for death countdown")]
    private Text m_CountdownText;

    [SerializeField]
    [Tooltip("Health Slider UI element")]
    private Slider m_HealthSlider;

    [SerializeField]
    [Tooltip("The parent object that holds all Combat UI elements")]
    private RectTransform m_CombatParent;


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
    private Image m_ClassAbilityImage;
    private Image m_ActiveItemImage;
    private Image m_WeaponOneImage;
    private Image m_WeaponTwoImage;
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

    private void OnEnable() {
        GameManager.PlayerAddedEvent += Resize;
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
        m_ClassAbilityImage = m_CombatParent.GetChild(0).GetChild(0).GetComponent<Image>();
        m_ActiveItemImage = m_CombatParent.GetChild(1).GetChild(0).GetComponent<Image>();
        m_WeaponOneImage = m_CombatParent.GetChild(3).GetChild(0).GetComponent<Image>();
        m_WeaponTwoImage = m_CombatParent.GetChild(2).GetChild(0).GetComponent<Image>();
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

    #region HealthUI
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

    #region CombatUI
    //This will set weapon slot 1's image to the itemID's image
    public void SetWeaponOneImage(int itemID) {
        if (itemID != Consts.NULL_ITEM_ID) {
            m_WeaponOneImage.sprite = ItemManager.GetItem(itemID).GetIcon();
            m_WeaponOneImage.enabled = true;
        }
    }

    //This will set weapon slot 2's image to the itemID's image
    public void SetWeaponTwoImage(int itemID) {
        if (itemID != Consts.NULL_ITEM_ID) {
            m_WeaponTwoImage.sprite = ItemManager.GetItem(itemID).GetIcon();
            m_WeaponTwoImage.enabled = true;
        }
    }

    //This suite of functions clears
    public void ClearWeaponOneImage() {
        m_WeaponOneImage.enabled = false;
    }

    public void ClearWeaponTwoImage() {
        m_WeaponTwoImage.enabled = false;
    }

    public void ClearActiveImage() {
        m_ActiveItemImage.enabled = false;
    }

    //This will swap weapon 1's image with weapon 2's
    public void SwapWeaponImage() {
        if (m_WeaponTwoImage.enabled == false) {
            m_WeaponTwoImage.enabled = true;
            m_WeaponOneImage.enabled = false;
        }
        else if (m_WeaponOneImage.enabled == false) {
            m_WeaponOneImage.enabled = true;
            m_WeaponTwoImage.enabled = false;
        }
        Sprite tempImage = m_WeaponOneImage.sprite;
        m_WeaponOneImage.sprite = m_WeaponTwoImage.sprite;
        m_WeaponTwoImage.sprite = tempImage;
    }

    public void SetActiveItemImage(int itemID) {
        if (itemID != Consts.NULL_ITEM_ID) {
            m_ActiveItemImage.sprite = ItemManager.GetItem(itemID).GetIcon();
            m_ActiveItemImage.enabled = true;
        }
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

    #region ResizeUI
    public void Resize(int numPlayers) {
        Debug.Log("Reached");
        if (numPlayers == 2)
        {
            m_CombatParent.GetChild(0).transform.localScale = new Vector3(LeftGemsScaleTwo, LeftGemsScaleTwo, LeftGemsScaleTwo);
            RectTransform active = m_CombatParent.GetChild(1).GetComponent<RectTransform>();
            active.localScale = new Vector3(LeftGemsScaleTwo, LeftGemsScaleTwo, LeftGemsScaleTwo);
            active.anchoredPosition = new Vector3(LeftOffsetTwo, 0, 0);

        }
        if (numPlayers == 3) {
            m_CombatParent.GetChild(0).transform.localScale = new Vector3(LeftGemsScaleThree, LeftGemsScaleThree, LeftGemsScaleThree);
            RectTransform active = m_CombatParent.GetChild(1).GetComponent<RectTransform>();
            active.localScale = new Vector3(LeftGemsScaleThree, LeftGemsScaleThree, LeftGemsScaleThree);
            active.anchoredPosition = new Vector3(LeftOffsetThree, 0, 0);
        }
    }
    #endregion
}
