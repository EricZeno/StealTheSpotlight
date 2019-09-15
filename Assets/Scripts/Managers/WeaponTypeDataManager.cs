using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AllWeaponTypes {
    [System.Serializable]
    public struct WeaponType {
        public string Name;
        public float WindupTime;
        public float AttackTime;
        public float TotalAnimationTime;
    }

    public WeaponType[] AllTypes;
}

public class WeaponTypeDataManager : MonoBehaviour
{
    #region Static Variables
    public static WeaponTypeDataManager singleton {
        get;
        private set;
    }
    #endregion

    #region Private Variables
    private AllWeaponTypes typesData;
    #endregion

    #region Initialization
    private void Awake() {
        if (singleton == null) {
            singleton = this;
        }
        else if (singleton != this) {
            Destroy(this.gameObject);
        }

        typesData = Utility.CreateObjectFromJSON<AllWeaponTypes>(Consts.WEAPON_TYPES_DATA_FILE);
    }
    #endregion

    #region Accessors
    public float WindupTime(string type) {
        return GetTypeData(type).WindupTime;
    }

    public float AttackTime(string type) {
        return GetTypeData(type).AttackTime;
    }

    public float AnimationTime(string type) {
        return GetTypeData(type).TotalAnimationTime;
    }

    private AllWeaponTypes.WeaponType GetTypeData(string type) {
        foreach (var weaponType in typesData.AllTypes) {
            if (weaponType.Name == type) {
                return weaponType;
            }
        }
        throw new System.ArgumentException("The weapon type " + type + " is undefined.");
    }
    #endregion
}