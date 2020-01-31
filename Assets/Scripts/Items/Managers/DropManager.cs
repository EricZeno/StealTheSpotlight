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

    //List of items available for a particular item drop source
    [Header("Drops")]
    [SerializeField]
    private int[] m_BossDrops;
    [SerializeField]
    private int[] m_MiniBossDrops;
    [SerializeField]
    private int[] m_HardEnemyDrops;
    [SerializeField]
    private int[] m_MedEnemyDrops;
    [SerializeField]
    private int[] m_EasyEnemyDrops;
    [SerializeField]
    private int[] m_SwarmEnemyDrops;
    [SerializeField]
    private int[] m_EasyObjectiveDrops;
    [SerializeField]
    private int[] m_MediumObjectiveDrops;
    [SerializeField]
    private int[] m_HardObjectiveDrops;
    [SerializeField]
    private int[] m_ChestDrops;

    [SerializeField]
    private float m_BossDropRate;
    [SerializeField]
    private float m_MinibossDropRate;
    [SerializeField]
    private float m_HardEnemyDropRate;
    [SerializeField]
    private float m_MediumEnemyDropRate;
    [SerializeField]
    private float m_EasyEnemyDropRate;
    [SerializeField]
    private float m_SwarmEnemyDropRate;
    [SerializeField]
    private float m_ChestDropRate;
    [SerializeField]
    private float m_HardObjectiveDropRate;
    [SerializeField]
    private float m_MediumObjectiveDropRate;
    [SerializeField]
    private float m_EasyObjectiveDropRate;

    [SerializeField]
    private float m_CommonDropRate;
    [SerializeField]
    private float m_UncommonDropRate;
    [SerializeField]
    private float m_RareDropRate;
    [SerializeField]
    private float m_EpicDropRate;
    [SerializeField]
    private float m_LegendaryDropRate;
    #endregion

    #region Private Variables
    private static DropManager m_Singleton;

    private static Dictionary<EItemRarity, float> m_RarityToDropRateMap;
    private static Dictionary<EDropSourceCategory, int[]> m_DropCategoryLists;
    private static Dictionary<EDropSourceCategory, float> m_DropCategoryDropRates;
    private static Dictionary<int, int> m_UniqueItemsLeft;
    #endregion

    #region Initialization
    private void Awake() {
        if (m_Singleton == null) {
            m_Singleton = this;
        }
        else if (m_Singleton != this) {
            Destroy(this.gameObject);
        }

        InitRarityToDR();
        InitDropCatLists();
        InitDropCatDR();
    }

    private void InitRarityToDR() {
        if (m_LegendaryDropRate + m_EpicDropRate + m_RareDropRate + m_UncommonDropRate + m_CommonDropRate != 1) {
            Debug.Log("Rarity drop rates don't add up to one");
        }
        m_RarityToDropRateMap = new Dictionary<EItemRarity, float>();
        m_RarityToDropRateMap[EItemRarity.COMMON] = m_CommonDropRate;
        m_RarityToDropRateMap[EItemRarity.UNCOMMON] = m_UncommonDropRate;
        m_RarityToDropRateMap[EItemRarity.RARE] = m_RareDropRate;
        m_RarityToDropRateMap[EItemRarity.EPIC] = m_EpicDropRate;
        m_RarityToDropRateMap[EItemRarity.LEGENDARY] = m_LegendaryDropRate;
    }

    private void InitDropCatLists() {
        m_DropCategoryLists = new Dictionary<EDropSourceCategory, int[]>();
        m_DropCategoryLists[EDropSourceCategory.BOSS] = m_BossDrops;
        m_DropCategoryLists[EDropSourceCategory.MINIBOSS] = m_MiniBossDrops;
        m_DropCategoryLists[EDropSourceCategory.HARD_ENEMY] = m_HardEnemyDrops;
        m_DropCategoryLists[EDropSourceCategory.MEDIUM_ENEMY] = m_MedEnemyDrops;
        m_DropCategoryLists[EDropSourceCategory.EASY_ENEMY] = m_EasyEnemyDrops;
        m_DropCategoryLists[EDropSourceCategory.SWARM_ENEMY] = m_SwarmEnemyDrops;
        m_DropCategoryLists[EDropSourceCategory.EASY_OBJECTIVE] = m_EasyObjectiveDrops;
        m_DropCategoryLists[EDropSourceCategory.MEDIUM_OBJECTIVE] = m_MediumObjectiveDrops;
        m_DropCategoryLists[EDropSourceCategory.HARD_OBJECTIVE] = m_HardObjectiveDrops;
        m_DropCategoryLists[EDropSourceCategory.CHEST] = m_ChestDrops;
    }

    private void InitDropCatDR() {
        m_DropCategoryDropRates = new Dictionary<EDropSourceCategory, float>();
        m_DropCategoryDropRates[EDropSourceCategory.BOSS] = m_BossDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.MINIBOSS] = m_MinibossDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.HARD_ENEMY] = m_HardEnemyDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.MEDIUM_ENEMY] = m_MediumEnemyDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.EASY_ENEMY] = m_EasyEnemyDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.SWARM_ENEMY] = m_SwarmEnemyDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.EASY_OBJECTIVE] = m_EasyObjectiveDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.MEDIUM_OBJECTIVE] = m_MediumObjectiveDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.HARD_OBJECTIVE] = m_HardObjectiveDropRate;
        m_DropCategoryDropRates[EDropSourceCategory.CHEST] = m_ChestDropRate;
    }
    #endregion

    #region Accessors
    public static int GetDrop(Dictionary<int, float> customItemMap, bool includeDefaults, EDropSourceCategory category) {
        //Determine whether an item drops
        float dropRate = m_DropCategoryDropRates[category];
        float randomPointer = Random.Range(0, 1);
        if (randomPointer > (dropRate)) {
            return Consts.NULL_ITEM_ID;
        }

        //Choose the item to drop

        //This is a list of available items, in the same order as selection list.
        List<int> commonItemList = new List<int>();
        List<int> uncommonItemList = new List<int>();
        List<int> rareItemList = new List<int>();
        List<int> epicItemList = new List<int>();
        List<int> legendaryItemList = new List<int>();

        Dictionary<EItemRarity, List<int>> itemLists = new Dictionary<EItemRarity, List<int>>();
        itemLists[EItemRarity.COMMON] = commonItemList;
        itemLists[EItemRarity.UNCOMMON] = uncommonItemList;
        itemLists[EItemRarity.RARE] = rareItemList;
        itemLists[EItemRarity.EPIC] = epicItemList;
        itemLists[EItemRarity.LEGENDARY] = legendaryItemList;

        //This is a variable used to segment items into a list. 
        /*For example, the first item will be placed at its drop chance,
         *  while the second will be placed at the 1st item plus the 2nd item's cumulative drop chances.
         */
        /*This makes it easy to pick items using a random number, 
         * since we can one by one check whether the random no is less than the first item, 
         * less than the second item, etc
         */
        if (includeDefaults) {
            int[] categoryItems = m_DropCategoryLists[category];

            float specialTotalWeight = 0;
            foreach (int value in customItemMap.Values) {
                specialTotalWeight += value;
            }

            //Determine whether to use special items or default items
            float j = Random.Range(0, 1);

            //If using default items
            if (j >= specialTotalWeight) {
                foreach (int item in categoryItems) {
                    EItemRarity rarity;
                    if (ItemManager.IsPassiveItem(item)) {
                        rarity = ItemManager.GetPassiveItem(item).Rarity;
                    }
                    else if (ItemManager.IsWeaponItem(item)) {
                        rarity = ItemManager.GetWeaponItem(item).Rarity;
                    }
                    else {
                        rarity = ItemManager.GetActiveItem(item).Rarity;
                    }
                    itemLists[rarity].Add(item);
                }

                List<int> rarityItemList;
                #region Drop Rarity Code - NEEDS REVISION
                //float selector = Random.Range(0, 1);
                //if (selector < m_RarityToDropRateMap[EItemRarity.LEGENDARY]) {
                //    rarityItemList = itemLists[EItemRarity.LEGENDARY];
                //}
                //else {
                //    selector -= m_RarityToDropRateMap[EItemRarity.LEGENDARY];
                //}
                //if (selector < m_RarityToDropRateMap[EItemRarity.EPIC]) {
                //    rarityItemList = itemLists[EItemRarity.EPIC];
                //}
                //else {
                //    selector -= m_RarityToDropRateMap[EItemRarity.EPIC];
                //}
                //if (selector < m_RarityToDropRateMap[EItemRarity.RARE]) {
                //    rarityItemList = itemLists[EItemRarity.RARE];
                //}
                //else {
                //    selector -= m_RarityToDropRateMap[EItemRarity.RARE];
                //}
                //if (selector < m_RarityToDropRateMap[EItemRarity.UNCOMMON]) {
                //    rarityItemList = itemLists[EItemRarity.UNCOMMON];
                //}
                #endregion

                rarityItemList = itemLists[EItemRarity.COMMON];
                return rarityItemList[Random.Range(0, rarityItemList.Count - 1)];
            }
            //If using special items
        }

        if (customItemMap.Keys.Count == 0) {
            return Consts.NULL_ITEM_ID;
        }
        //Add special items to selection list
        float dropChance = 0;
        List<float> selectionList = new List<float>();
        List<int> itemList = new List<int>();
        foreach (int item in customItemMap.Keys) {
            dropChance += customItemMap[item];
            selectionList.Add(dropChance);
            itemList.Add(item);
        }
        float random = Random.Range(0, dropChance);
        int i = 0;
        while (selectionList[i] < random) {
            i++;
        }
        return itemList[i];
    }

    public static int GetDrop(EDropSourceCategory category) {
        return GetDrop(new Dictionary<int, float>(), true, category);
    }
    #endregion

    #region Dropping Methods
    public static void DropItem(int itemID, Vector3 position) {
        GameObject item = Instantiate(m_Singleton.m_ItemGO, position, Quaternion.identity);
        item.GetComponent<ItemGameObject>().SetupGameObject(itemID);
    }
    #endregion
}
