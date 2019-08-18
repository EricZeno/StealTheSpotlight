using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ItemManager : MonoBehaviour {
    #region Private Variables
    private BasePassiveItem[] p_PassiveItems;
    private BaseActiveItem[] p_ActiveItems;
    private BaseWeaponItem[] p_WeaponItems;
    #endregion

    #region Accessors
    public BasePassiveItem GetPassiveItem(int ID) {
        throw new System.NotImplementedException();
    }

    public BaseActiveItem GetActiveItem(int ID) {
        throw new System.NotImplementedException();
    }

    public BaseWeaponItem GetWeaponItem(int ID) {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Checkers
    public bool IsPassiveItem(int ID) {
        throw new System.NotImplementedException();
    }

    public bool IsActiveItem(int ID) {
        throw new System.NotImplementedException();
    }

    public bool IsWeaponItem(int ID) {
        throw new System.NotImplementedException();
    }
    #endregion
}
