using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDropSourceCategory {
    BOSS,
    MINIBOSS,
    HARD_ENEMY,
    MEDIUM_ENEMY,
    EASY_ENEMY,
    SWARM_ENEMY,
    CHEST,
    HARD_OBJECTIVE,
    MEDIUM_OBJECTIVE,
    EASY_OBJECTIVE
}

[DisallowMultipleComponent]
public class DropManager : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The item game object that gets spawned in the game world to represent an item.")]
    private GameObject m_ItemGO;
    #endregion

    #region Private Variables
    private static DropManager m_Singleton;

    private Dictionary<EItemRarity, float> m_RarityToDropRateMap;
    private Dictionary<EDropSourceCategory, BaseItem> m_DropCategoryLists;
    private Dictionary<EDropSourceCategory, float> m_DropCategoryDropRates;
    private Dictionary<int, int> m_UniqueItemsLeft;
    #endregion

    #region Initialization
    private void Awake() {
        if (m_Singleton == null) {
            m_Singleton = this;
        }
        else if (m_Singleton != this) {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Accessors
    public BaseItem GetDrop(Dictionary<int, float> customItemMap, bool includeDefaults) {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Dropping Methods
    public static void DropItem(int itemID, Vector3 position) {
        GameObject item = Instantiate(m_Singleton.m_ItemGO, position, Quaternion.identity);
        item.GetComponent<ItemGameObject>().SetupGameObject(itemID);
    }
    #endregion
}
