using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EActiveItemTargetingTypeGFX {
    TARGETED_AOE,
    LINEAR,
    NO_AIM
}

public interface IBaseActiveItem {
    void UseEffect();
    void StopEffect();
    void UpdateEndPosition(Vector2 pos);
}

public abstract class BaseActiveItem : BaseItem, IBaseActiveItem {
    #region Private Variables
    private int p_Stack;
    private float p_Cooldown;
    private bool p_IsBeingUsed;
    private EActiveItemTargetingTypeGFX p_TargetingType;
    #endregion

    #region Graphics Update Methods
    protected abstract void AimGFX();
    #endregion

    #region Base Active Item Interface
    public abstract void UseEffect();
    public abstract void StopEffect();

    public void UpdateEndPosition(Vector2 pos) {
        throw new System.NotImplementedException();
    }
    #endregion
}
