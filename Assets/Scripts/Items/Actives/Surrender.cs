using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Surrender", menuName = "Items/Actives/Surrender")]
public class Surrender : BaseActiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How long the effect should last for.")]
    private float m_EffectLength;
    #endregion

    #region Interface Methods
    public override void UseEffect(PlayerManager player) {
        player.AddStatusEffectForXSec(Status.Invulnerable, m_EffectLength);
    }

    public override void StopEffect() { }

    public override void CancelEffect() { }

    protected override void AimGFX() { }
    #endregion
}
