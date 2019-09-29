using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ERarityMert {
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[DisallowMultipleComponent]
public class DropManagerMert : MonoBehaviour
{
    public void Awake() {
        p_Random = new Random();
    }

    #region Editor Variables

    //List of items available for a particular item drop source
    //TODO: Add a dictionary to encompass all drop lists and an enumerator
    public int[] p_BossDrops;
    public int[] p_HardEnemyDrops;
    public int[] p_MedEnemyDrops;
    public int[] p_EasyEnemyDrops;
    public int[] p_ObjectiveDrops;
    #endregion

    #region Private Variables
    [SerializeField]
    [Tooltip("This is a reference to the ItemManager")]
    private ItemManager m_ItemManager;

    Random p_Random;
    private int p_Floor = 0;

    //Maps rarities to the drop chances of each rarity for the current
    private Dictionary<ERarityMert, float> p_RarityValues;

    //Maps unique items to the remaining number of times they can drop
    private Dictionary<int, int> p_UniqueItemCount;

    //Maps Drop Category item lists to item drop chance for each category
    private Dictionary<int, float> p_DropCatDropRates;
    #endregion

    #region Methods
    //Returns a random item ID based on given rules
    public int GetDrop(Dictionary<int, float> specialItems, bool useDefault, int category) {

        //Determine whether an item drops
        float dropRate = p_DropCatDropRates[category];
        int randomPointer = Random.Range(0, 100);
        if(randomPointer <= (dropRate * 100)) {
            return -1;
        }

        //Choose the item to drop

        //Selection list is a list of drop rates for each item to enable easy selection. See dropChance to understand how this works
        List<float> selectionList = new List<float>();
        //This is a list of available items, in the same order as selection list.
        List<int> itemList = new List<int>();

        //This is a variable used to segment items into a list. 
        //For example, the first item will be placed at its drop chance, while the second will be placed at the 1st item plus the 2nd item's cumulative drop chances.
        //This makes it easy to pick items using a random number, since we can one by one check whether the random no is less than the first item, less than the second item, etc
        float dropChance = 0;
        if (useDefault) {
            int[] categoryItems;
            switch (category) {
                case 0:
                    categoryItems = p_BossDrops;
                    break;
                case 1:
                    categoryItems = p_HardEnemyDrops;
                    break;
                case 2:
                    categoryItems = p_MedEnemyDrops;
                    break;
                case 3:
                    categoryItems = p_EasyEnemyDrops;
                    break;
                case 4:
                    categoryItems = p_ObjectiveDrops;
                    break;
                default:
                    categoryItems = p_EasyEnemyDrops;
                    break;
            }

            float specialTotalWeight = 0;
            foreach (int value in specialItems.Values) {
                specialTotalWeight += value;
            }

            //If there are no special item rules
            if (specialTotalWeight == 0) {
                foreach (int item in categoryItems) {
                    if (ItemManager.IsPassiveItem(item)) {
                        ERarityMert rarity = m_ItemManager.GetPassiveItem(item).Rarity;
                        dropChance += p_RarityValues[rarity];
                    } else if (ItemManager.IsWeaponItem(item)) {
                        ERarityMert rarity = m_ItemManager.GetWeaponItem(item).Rarity;
                        dropChance += p_RarityValues[rarity];
                    } else {
                        ERarityMert rarity = m_ItemManager.GetActiveItem(item).Rarity;
                        dropChance += p_RarityValues[rarity];
                    }
                    selectionList.Add(dropChance);
                    itemList.Add(item);
                }

            //If there are special item rules
            } else {

                //Add special items to selection list
                foreach (int item in specialItems.Keys) {
                    dropChance += specialItems[item];
                    selectionList.Add(dropChance);
                    itemList.Add(item);
                }

                //Add default items to selection list
                foreach (int item in categoryItems) {
                    if (ItemManager.IsPassiveItem(item)) {
                        ERarityMert rarity = m_ItemManager.GetPassiveItem(item).Rarity;
                        dropChance += p_RarityValues[rarity] * (1-specialTotalWeight);
                    } else if (ItemManager.IsWeaponItem(item)) {
                        ERarityMert rarity = m_ItemManager.GetWeaponItem(item).Rarity;
                        dropChance += p_RarityValues[rarity] * (1 - specialTotalWeight);
                    } else {
                        ERarityMert rarity = m_ItemManager.GetActiveItem(item).Rarity;
                        dropChance += p_RarityValues[rarity] * (1 - specialTotalWeight);
                    }
                    selectionList.Add(dropChance);
                    itemList.Add(item);
                }
            }

        //Only special items
        } else {
            foreach (int item in specialItems.Keys) {
                dropChance += specialItems[item];
                selectionList.Add(dropChance);
                itemList.Add(item);
            }
        }
        if (dropChance != 1) {
            Debug.Log("Drop chance does not add up to 100%");
            return -1;
        }
        float random = Random.Range(0, dropChance);
        int i = 0;
        while (selectionList[i] < random) {
            i++;
        }
        return itemList[i];
    }

    //Called by floor manager to update rarity drop chances for floor
    public void UpdateRarityTable(float common, float uncommon, float rare, float epic, float legendary) {
        if(common + uncommon + rare + epic + legendary != 1) {
            Debug.Log("Total rarity droprate does not add up to 1");
        }
        p_RarityValues[ERarityMert.Common] = common;
        p_RarityValues[ERarityMert.Uncommon] = uncommon;
        p_RarityValues[ERarityMert.Rare] = rare;
        p_RarityValues[ERarityMert.Epic] = epic;
        p_RarityValues[ERarityMert.Legendary] = legendary;
    }
    #endregion

    //Makes all items an available drop for every drop source
    //This is for testing purposes, especially early in development
    public void AddAll() {
        List<int> allItems = new List<int>();

        //foreach (int item in m_ItemManager.passiveItems) {
        //    allItems.Add(item);
        //}
        //foreach (int item in m_ItemManager.activeItems) {
        //    allItems.Add(item);
        //}
        //foreach (int item in m_ItemManager.weapons) {
        //    allItems.Add(item);
        //}

        //p_BossDrops = allItems;
        //p_HardEnemyDrops = allItems;
        //p_MedEnemyDrops = allItems;
        //p_EasyEnemyDrops = allItems;
        //p_ObjectiveDrops = allItems;
    }
}
