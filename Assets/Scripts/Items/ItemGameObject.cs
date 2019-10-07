using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class ItemGameObject : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The ID of the item. This MUST be unique. It is how the code will refer to the item.")]
    private int m_ID;
    #endregion

    #region Cached Components
    private SpriteRenderer m_Renderer;
    #endregion

    #region Initialization
    private void Awake() {
        m_Renderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        StartCoroutine(SetupIfReady());
    }

    private IEnumerator SetupIfReady() {
        yield return new WaitForSeconds(0.2f);

        if (m_ID != Consts.NULL_ITEM_ID) {
            SetupGameObject(m_ID);
        }
    }

    public void SetupGameObject(int itemID) {
        m_ID = itemID;
        BaseItem item = ItemManager.GetItem(itemID);
        m_Renderer.sprite = item.GetIcon();
    }
    #endregion

    #region Accessors
    public int GetID() {
        return m_ID;
    }
    #endregion
}
