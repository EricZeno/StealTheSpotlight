using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.PlayerInput;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerInventoryController))]
public class PlayerInteraction : MonoBehaviour {
    #region Cached Components
    private PlayerManager m_Manager;
    private PlayerInventoryController m_InventoryController;
    #endregion

    #region Initialization
    private void Awake() {
        SetupCachedComponents();
    }

    private void SetupCachedComponents() {
        m_Manager = GetComponent<PlayerManager>();
        m_InventoryController = GetComponent<PlayerInventoryController>();
    }
    #endregion

    #region Input Receivers
    private void OnInteract(InputValue value) {
        LayerMask layerMask = 1 << LayerMask.NameToLayer(Consts.ITEM_PHYSICS_LAYER);
        float radius = m_Manager.InteractionRadius;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, 0, layerMask);
        if (!hit) {
            return;
        }
        ItemGameObject item = hit.collider.GetComponent<ItemGameObject>();
        bool itemPickedUp = m_InventoryController.PickUpItem(item);
        if (itemPickedUp) {
            Destroy(item.gameObject);
        }
    }
    #endregion
}
