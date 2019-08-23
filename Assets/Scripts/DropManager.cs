using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Rarity {
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public class DropManager : MonoBehaviour
{
    #region Public Variables
    public ItemManager ItemManager;

    //List of items available for a particular item drop source
    public List<int> p_BossDrops;
    public List<int> p_HardEnemyDrops;
    public List<int> p_MedEnemyDrops;
    public List<int> p_EasyEnemyDrops;
    public List<int> p_ObjectiveDrops;
    #endregion

    #region Private Variables
    Random random = new Random();
    private int floor = 0;

    //Maps rarities to the drop chances of each rarity for the current
    private Dictionary<Rarity, float> p_RarityValues;

    //Maps unique items to the remaining number of times they can drop
    private Dictionary<int, int> p_UniqueItemCount;

    //Maps Drop Category item lists to item drop chance for each category
    private Dictionary<List<int>, float> p_DropCatDropRates;
    #endregion

    #region Methods
    //Returns a random item ID based on given rules
    public int GetDrop(Dictionary<int, float> specialItems, bool useDefault, int category) {
        List<float> selectionList = new List<float>();
        List<int> itemList = new List<int>();
        
        if (useDefault) {
            List<int> categoryItems;
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
                float dropChance = 0;
                foreach (int item in categoryItems) {
                    if (ItemManager.isPassive(item)) {
                        Rarity rarity = ItemManager.GetPassiveInformation(item).rarity;
                        dropChance += p_RarityValues[rarity];
                    } else if (ItemManager.isWeapon(item)) {
                        Rarity rarity = ItemManager.GetWeaponInformation(item).rarity;
                        dropChance += p_RarityValues[rarity];
                    } else {
                        Rarity rarity = ItemManager.GetActiveInformation(item).rarity;
                        dropChance += p_RarityValues[rarity];
                    }
                    selectionList.Add(dropChance);
                    itemList.Add(item);
                }
                float random = Random.Range(0, dropChance);
                int i = 0;
                while(selectionList[i] < random) {
                    i++;
                }
                return itemList[i];

            //If there are special item rules
            } else {

                //Add special items to selection list
                float dropChance = 0;
                foreach (int item in specialItems.Keys) {
                    dropChance += specialItems[item];
                    selectionList.Add(dropChance);
                    itemList.Add(item);
                }

                //Add default items to selection list
                foreach (int item in categoryItems) {
                    if (item <= 3333) {
                        Rarity rarity = ItemManager.GetPassiveInformation(item).rarity;
                        dropChance += p_RarityValues[rarity] * (1-specialTotalWeight);
                    } else if (item > 6666) {
                        Rarity rarity = ItemManager.GetWeaponInformation(item).rarity;
                        dropChance += p_RarityValues[rarity] * (1 - specialTotalWeight);
                    } else {
                        Rarity rarity = ItemManager.GetActiveInformation(item).rarity;
                        dropChance += p_RarityValues[rarity] * (1 - specialTotalWeight);
                    }
                    selectionList.Add(dropChance);
                    itemList.Add(item);
                }

                //Select item
                float random = Random.Range(0, dropChance);
                int i = 0;
                while (selectionList[i] < random) {
                    i++;
                }
                return itemList[i];
            }

        //Only special items
        } else {
            float dropChance = 0;
            foreach (int item in specialItems.Keys) {
                dropChance += specialItems[item];
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
    }

    //Called by floor manager to update rarity drop chances for floor
    public void Update(float common, float uncommon, float rare, float epic, float legendary) {
        p_RarityValues[Rarity.Common] = common;
        p_RarityValues[Rarity.Uncommon] = uncommon;
        p_RarityValues[Rarity.Rare] = rare;
        p_RarityValues[Rarity.Epic] = epic;
        p_RarityValues[Rarity.Legendary] = legendary;
    }
    #endregion

    //Makes all items an available drop for every drop source
    public void AddAll() {
        List<int> allItems = new List<int>();

        foreach(int item in ItemManager.passiveItems) {
            allItems.Add(item);
        }
        foreach (int item in ItemManager.activeItems) {
            allItems.Add(item);
        }
        foreach (int item in ItemManager.weapons) {
            allItems.Add(item);
        }

        p_BossDrops = allItems;
        p_HardEnemyDrops = allItems;
        p_MedEnemyDrops = allItems;
        p_EasyEnemyDrops = allItems;
        p_ObjectiveDrops = allItems;
    }
}
