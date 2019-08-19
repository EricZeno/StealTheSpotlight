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
    #region Editor Variables
    [SerializeField]
    [Tooltip("The ID of the item. This MUST be unique. It is how the code will refer to the item.")]
    private int m_ID;

    [SerializeField]
    [Tooltip("The name of this item. When a player picks up this item, this name is dispalyed.")]
    private string m_ItemName;
    [SerializeField]
    [Tooltip("The description of this item. This is the flavor text. It should be short, concise, and " +
        "it should leave out percentages and numbers as much as possible. This description will be visible " +
        "to players in the game.")]
    private string m_Description;

    [SerializeField]
    private EItemRarity m_Rarity;
    [SerializeField]
    private int m_Limit;

    [SerializeField]
    private Sprite m_Icon;
    #endregion

    #region Private Variables
    // Whether this item is a passive, active, or a weapon.
    protected EItemType p_Type;
    #endregion
}
