using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ItemManager : MonoBehaviour {
    #region Private Variables
    private BasePassiveItem[] p_PassiveItems;
    private BaseActiveItem[] p_ActiveItems;
    private BaseWeaponItem[] p_WeaponItems;
    #endregion

    #region Initialization
    private void Start() {
        p_PassiveItems = (BasePassiveItem[])(LoadItemGroup("passiveitems"));
        //p_ActiveItems = (BasePassiveItem[])(LoadItemGroup("activeitems"));
        //p_WeaponItems = (BasePassiveItem[])(LoadItemGroup("weaponitems"));
    }

    private BaseItem[] LoadItemGroup(string assetBundleToLoad) {
        string assetBundlesRoot = Application.dataPath;
        if (!Application.isEditor) {
            assetBundlesRoot = Application.streamingAssetsPath;
        }
        assetBundlesRoot = Path.Combine(assetBundlesRoot, Constants.ASSETBUNDLES_DIRECTORY);
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetBundlesRoot, Path.Combine("items", assetBundleToLoad)));
        if (myLoadedAssetBundle == null) {
            throw new System.TypeLoadException($"Failed to Load AssetBundle: {assetBundleToLoad}");
        }
        BaseItem[] itemGroup = myLoadedAssetBundle.LoadAllAssets<BaseItem>();
        if (itemGroup.Length == 0) {
            Debug.LogWarning($"AssetBundle {assetBundleToLoad} contained 0 items.");
        }
        return itemGroup;
    }
    #endregion
    
    #region Accessors
    public BasePassiveItem GetPassiveItem(int ID) {
        if (!IsPassiveItem(ID)) {
            Debug.LogError($"{ID} is not a passive item ID");
            return null;
        }
        if (ID >= p_PassiveItems.Length) {
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
        if (ID >= p_ActiveItems.Length) {
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
        if (ID >= p_WeaponItems.Length) {
            Debug.LogError($"There is no weapon item with {ID}. The largest ID we have for weapon items is {p_WeaponItems.Length - 1}");
            return null;
        }
        return p_WeaponItems[ID - Constants.MIN_WEAPON_ITEM_ID];
    }
    #endregion

    #region Checkers
    public bool IsPassiveItem(int ID) {
        return Constants.MIN_PASSIVE_ITEM_ID <= ID && ID <= Constants.MAX_PASSIVE_ITEM_ID;
    }

    public bool IsActiveItem(int ID) {
        return Constants.MIN_ACTIVE_ITEM_ID <= ID && ID <= Constants.MAX_ACTIVE_ITEM_ID;
    }

    public bool IsWeaponItem(int ID) {
        return Constants.MIN_WEAPON_ITEM_ID <= ID && ID <= Constants.MAX_WEAPON_ITEM_ID;
    }
    #endregion
}
