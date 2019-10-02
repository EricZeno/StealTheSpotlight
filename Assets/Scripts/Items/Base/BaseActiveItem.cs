using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EActiveItemTargetingTypeGFX {
    TARGETED_AOE,
    LINEAR,
    NO_AIM
}

public interface IBaseActiveItem {
    void UseEffect(PlayerManager player);
    void StopEffect();
    void CancelEffect();
    void UpdateEndPosition(Vector2 pos);
}

public abstract class BaseActiveItem : BaseItem, IBaseActiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How many times this item can be used before it gets removed. " +
        "A stack of zero implies that this item can be used infinitely.")]
    private int m_Stack;

    [SerializeField]
    [Tooltip("How long to wait between uses of the item. A zero cooldown " +
        "implies that this item has no cooldown.")]
    private float m_Cooldown;
    #endregion

    #region Private Variables
    private bool m_IsBeingUsed;

    private EActiveItemTargetingTypeGFX m_TargetingType;
    #endregion

    #region Accessors
    public float GetCooldown() {
        return m_Cooldown;
    }
    #endregion

    #region Graphics Update Methods
    protected abstract void AimGFX();
    #endregion

    #region Base Active Item Interface
    public abstract void UseEffect(PlayerManager player);
    public abstract void StopEffect();
    public abstract void CancelEffect();

    public void UpdateEndPosition(Vector2 pos) {
        throw new System.NotImplementedException();
    }
    #endregion
}
