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
    [Tooltip("How rare this item is. This attribute of the item affects how often it will be seen within a session.")]
    private EItemRarity m_Rarity;
    [SerializeField]
    [Tooltip("How many times this item may drop in total throughout all floors of a game session. For \"infinite\", use 0.")]
    [Range(0, 999)]
    private int m_Limit;

    [SerializeField]
    [Tooltip("The representation of this item as a drop on the floor and the icon that will appear in a player's inventory.")]
    private Sprite m_Icon;
    #endregion

    #region Private Variables
    // Whether this item is a passive, active, or a weapon.
    protected EItemType p_Type;
    #endregion

    #region Accessors
    public string GetItemName() {
        return m_ItemName;
    }

    public int GetID() {
        return m_ID;
    }

    public ERarityMert Rarity {
        get { return ERarityMert.Common; }
    }

    public Sprite GetIcon() {
        return m_Icon;
    }
    #endregion
}
