using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

[DisallowMultipleComponent]
public class ItemManager : MonoBehaviour {
    #region Private Variables
    private static ItemManager p_Singleton;
    public static ItemManager GetSingleton() {
        return p_Singleton;
    }

    private BasePassiveItem[] p_PassiveItems;
    private BaseActiveItem[] p_ActiveItems;
    private BaseWeaponItem[] p_WeaponItems;
    #endregion

    #region Initialization
    private void Awake() {
        if (!p_Singleton) {
            p_Singleton = this;
        }
        else if (p_Singleton != this) {
            Destroy(p_Singleton.gameObject);
        }
    }

    private void Start() {
        p_PassiveItems = LoadItemGroup("passiveitems");
        p_PassiveItems = p_PassiveItems.OrderBy(item => item.GetID()).ToArray();
        //p_ActiveItems = (BasePassiveItem[])(LoadItemGroup("activeitems"));
        //p_WeaponItems = (BasePassiveItem[])(LoadItemGroup("weaponitems"));
    }

    private BasePassiveItem[] LoadItemGroup(string assetBundleToLoad) {
        string assetBundlesRoot = Application.dataPath;
        if (!Application.isEditor) {
            assetBundlesRoot = Application.streamingAssetsPath;
        }
        assetBundlesRoot = Path.Combine(assetBundlesRoot, Constants.ASSETBUNDLES_DIRECTORY);
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundlesRoot, Path.Combine("items", assetBundleToLoad)));
        if (myLoadedAssetBundle == null) {
            throw new System.TypeLoadException($"Failed to Load AssetBundle: {assetBundleToLoad}");
        }
        BasePassiveItem[] itemGroup = myLoadedAssetBundle.LoadAllAssets<BasePassiveItem>();

        if (itemGroup.Length == 0) {
            Debug.LogWarning($"AssetBundle {assetBundleToLoad} contained 0 items.");
        }
        return itemGroup;
    }
    #endregion
    
    #region Accessors
    public BaseItem GetItem(int ID) {
        if (IsPassiveItem(ID)) {
            return GetPassiveItem(ID);
        }
        if (IsActiveItem(ID)) {
            return GetActiveItem(ID);
        }
        return GetWeaponItem(ID);
    }

    public BasePassiveItem GetPassiveItem(int ID) {
        if (!IsPassiveItem(ID)) {
            Debug.LogError($"{ID} is not a passive item ID");
            return null;
        }
        if (p_PassiveItems == null) {
            Debug.LogError($"The passive items were not loaded in.");
            return null;
        }
        if (ID - Constants.MIN_PASSIVE_ITEM_ID >= p_PassiveItems.Length) {
            Debug.LogError($"There is no passive item with {ID}. The largest ID we have for passive items is {p_PassiveItems.Length - 1}");
            return null;
        }
        return p_PassiveItems[ID - Constants.MIN_PASSIVE_ITEM_ID];
    }

    public BaseActiveItem GetActiveItem(int ID) {
        if (!IsActiveItem(ID)) {
            Debug.LogError($"{ID} is not a active item ID");
            return null;
        }
        if (p_ActiveItems == null) {
            Debug.LogError($"The active items were not loaded in.");
            return null;
        }
        if (ID - Constants.MIN_ACTIVE_ITEM_ID >= p_ActiveItems.Length) {
            Debug.LogError($"There is no active item with {ID}. The largest ID we have for active items is {p_ActiveItems.Length - 1}");
            return null;
        }
        return p_ActiveItems[ID - Constants.MIN_ACTIVE_ITEM_ID];
    }

    public BaseWeaponItem GetWeaponItem(int ID) {
        if (!IsWeaponItem(ID)) {
            Debug.LogError($"{ID} is not a weapon item ID");
            return null;
        }
        if (p_WeaponItems == null) {
            Debug.LogError($"The weapon items were not loaded in.");
            return null;
        }
        if (ID - Constants.MIN_WEAPON_ITEM_ID >= p_WeaponItems.Length) {
            Debug.LogError($"There is no weapon item with {ID}. The largest ID we have for weapon items is {p_WeaponItems.Length - 1}");
            return null;
        }
        return p_WeaponItems[ID - Constants.MIN_WEAPON_ITEM_ID];
    }
    #endregion

    #region Checkers
    public static bool IsPassiveItem(int ID) {
        return Constants.MIN_PASSIVE_ITEM_ID <= ID && ID <= Constants.MAX_PASSIVE_ITEM_ID;
    }

    public static bool IsActiveItem(int ID) {
        return Constants.MIN_ACTIVE_ITEM_ID <= ID && ID <= Constants.MAX_ACTIVE_ITEM_ID;
    }

    public static bool IsWeaponItem(int ID) {
        return Constants.MIN_WEAPON_ITEM_ID <= ID && ID <= Constants.MAX_WEAPON_ITEM_ID;
    }
    #endregion
}
