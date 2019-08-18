using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType {
    PASSIVE,
    ACTIVE,
    WEAPON
}

public enum EItemRarity {
    COMMON,
    UNCOMMON,
    RARE,
    SECRET
}

public abstract class BaseItem : ScriptableObject {
    #region Private Variables
    private int p_ID;

    private string p_Name;
    private string p_Description;

    private EItemType p_Type;
    private EItemRarity p_Rarity;
    private int p_Limit;

    private Sprite p_Icon;
    #endregion
}
