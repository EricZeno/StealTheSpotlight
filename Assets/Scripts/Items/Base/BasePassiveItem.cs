using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPassiveItemActivationType {
    ON_DAMAGED,
    ON_ATTACK,
    PERMANENT,
    ON_DEATH
}

public interface IBasePassiveItem {
    void ApplyEffect(PlayerManager player);
    void RemoveEffect(PlayerManager player);
}

public abstract class BasePassiveItem : BaseItem, IBasePassiveItem {
    #region Editor Variables
    [SerializeField]
    [Tooltip("When the effect of this item should activate.")]
    private EPassiveItemActivationType m_ActivationType;
    #endregion
    
    #region Initialization
    private void Awake() {
        p_Type = EItemType.PASSIVE;
    }
    #endregion

    #region Accessors
    public EPassiveItemActivationType GetActivationType() {
        return m_ActivationType;
    }
    #endregion

    #region Base Passive Item Interface Methods
    public abstract void ApplyEffect(PlayerManager player);
    public abstract void RemoveEffect(PlayerManager player);
    #endregion
}
