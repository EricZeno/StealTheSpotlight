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
    #region Private Variables
    private Dictionary<EItemRarity, float> p_RarityToDropRateMap;
    private Dictionary<EDropSourceCategory, BaseItem> p_DropCategoryLists;
    private Dictionary<EDropSourceCategory, float> p_DropCategoryDropRates;
    private Dictionary<int, int> p_UniqueItemsLeft;
    #endregion

    #region Accessors
    public BaseItem GetDrop(Dictionary<int, float> customItemMap, bool includeDefaults) {
        throw new System.NotImplementedException();
    }
    #endregion
}
