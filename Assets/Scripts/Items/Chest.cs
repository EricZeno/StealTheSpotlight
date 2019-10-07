using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Ids of the items this chest will drop in addition to the standard items.")]
    private int[] m_ItemIds;
    [SerializeField]
    [Tooltip("Drop rates of the items in the same order as the additional items.")]
    private float[] m_ItemDropRates;
    [SerializeField]
    [Tooltip("Include items from the default chest pool?")]
    private bool m_useDefault;
    #endregion

    #region Private Variables
    private Dictionary<int, float> m_SpecialItems;
    private int m_ItemCount = 0;
    private EDropSourceCategory m_Type = EDropSourceCategory.CHEST;
    #endregion

    #region Initialization
    private void Awake()
    {
        if (m_ItemIds.Length != m_ItemDropRates.Length) {
            Debug.LogException(new Exception("Item and drop rate numbers don't match"));
        }
        m_ItemCount = m_ItemIds.Length;

        m_SpecialItems = new Dictionary<int, float>();

        for(int i = 0; i < m_ItemCount; i++) {
            m_SpecialItems[m_ItemIds[i]] = m_ItemDropRates[i];
        }
    }
    #endregion

    #region Drop Methods
    private void DropItem() {
        int itemId = DropManager.GetDrop(m_SpecialItems, m_useDefault, m_Type);
        if (itemId == -1) {
            return;
        }
        DropManager.DropItem(itemId, transform.position);
        //Play a chest animation here
        Destroy(gameObject);
    }

    public void Interact() {
        DropItem();
    }
    #endregion

}
