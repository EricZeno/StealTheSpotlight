using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagnifyingGlass", menuName = "Items/Passives/MagnifyingGlass")]
public class MagnifyingGlass : BasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("Every edge of the player's collider will be multiplied by this value.")]
    private float m_ColliderEdgeMultiplier;
    #endregion

    #region Interface Methods
    public override void ApplyEffect(PlayerManager player) {
        player.transform.localScale *= m_ColliderEdgeMultiplier;
    }

    public override void RemoveEffect(PlayerManager player) {
        player.transform.localScale /= m_ColliderEdgeMultiplier;
    }
    #endregion
}
