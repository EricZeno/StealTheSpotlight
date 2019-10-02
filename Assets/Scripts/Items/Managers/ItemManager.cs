using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

[DisallowMultipleComponent]
public class ItemManager : MonoBehaviour {
    #region Delegates
    private delegate bool ItemTypeCheckerDelegate(int id);
    #endregion

    #region Private Variables
    private static ItemManager p_Singleton;

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
        SetupItems(ref p_PassiveItems, Consts.PASSIVE_ITEM_BUNDLE_NAME);
	    SetupItems(ref p_ActiveItems, Consts.ACTIVE_ITEM_BUNDLE_NAME);
    }
    #endregion

    #region Loading Item Groups
    private void SetupItems<T>(ref T[] items, string bundleName) where T : BaseItem {
        items = LoadItemGroup<T>(bundleName);
        items = items.OrderBy(item => item.GetID()).ToArray();
    }

    private T[] LoadItemGroup<T>(string assetBundleToLoad) where T : BaseItem {
        string assetBundlesRoot = Application.dataPath;
        if (!Application.isEditor) {
            assetBundlesRoot = Application.streamingAssetsPath;
        }
        assetBundlesRoot = Path.Combine(
            assetBundlesRoot, 
            Consts.ASSETBUNDLES_DIRECTORY);
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(
            assetBundlesRoot, 
            Path.Combine("items", assetBundleToLoad)));
        if (myLoadedAssetBundle == null) {
            throw new System.TypeLoadException($"Failed to Load " +
                $"AssetBundle: {assetBundleToLoad}");
        }
        T[] itemGroup = myLoadedAssetBundle.LoadAllAssets<T>();

        if (itemGroup.Length == 0) {
            Debug.LogWarning($"AssetBundle {assetBundleToLoad} contained 0 " +
                $"items.");
        }
        return itemGroup;
    }
    #endregion

    #region Accessors
    public static BaseItem GetItem(int ID) {
        AssertItemManagerExists();
        if (IsPassiveItem(ID)) {
            return GetPassiveItem(ID);
        }
        if (IsActiveItem(ID)) {
            return GetActiveItem(ID);
        }
        if (IsWeaponItem(ID)) {
            return GetWeaponItem(ID);
        }
        throw new System.ArgumentException($"There is no item type with ID " +
            $"{ID}. It is completely out of range.");
    }

    public static BasePassiveItem GetPassiveItem(int ID) {
        return GetItem(ID, p_Singleton.p_PassiveItems, IsPassiveItem,
                Consts.PASSIVE_ITEM_NAME, Consts.MIN_PASSIVE_ITEM_ID);
    }

    public static BaseActiveItem GetActiveItem(int ID) {
        return GetItem(ID, p_Singleton.p_ActiveItems, IsActiveItem,
                Consts.ACTIVE_ITEM_NAME, Consts.MIN_ACTIVE_ITEM_ID);
    }

    public static BaseWeaponItem GetWeaponItem(int ID) {
        return GetItem(ID, p_Singleton.p_WeaponItems, IsWeaponItem,
                Consts.WEAPON_ITEM_NAME, Consts.MIN_WEAPON_ITEM_ID);
    }

    private static T GetItem<T>(int ID, T[] itemGroup,
        ItemTypeCheckerDelegate isCorrectType, string itemType, int minIDValue)
        where T : BaseItem {
        if (!isCorrectType(ID)) {
            throw new System.ArgumentOutOfRangeException($"{ID} is not a " +
                $"{itemType} item ID");
        }
        if (itemGroup == null) {
            throw new System.AccessViolationException($"The {itemType} " +
                $"items were not loaded in.");
        }
        if (ID - minIDValue >= itemGroup.Length) {
            throw new System.ArgumentOutOfRangeException($"There is no " +
                $"{itemType} item with {ID}. The largest ID we have for " +
                $"{itemType} items is {itemGroup.Length - 1}");
        }
        return itemGroup[ID - minIDValue];
    }
    #endregion

    #region Checkers
    public static bool IsPassiveItem(int ID) {
        return Consts.MIN_PASSIVE_ITEM_ID <= ID && 
            ID <= Consts.MAX_PASSIVE_ITEM_ID;
    }

    public static bool IsActiveItem(int ID) {
        return Consts.MIN_ACTIVE_ITEM_ID <= ID && 
            ID <= Consts.MAX_ACTIVE_ITEM_ID;
    }

    public static bool IsWeaponItem(int ID) {
        return Consts.MIN_WEAPON_ITEM_ID <= ID && 
            ID <= Consts.MAX_WEAPON_ITEM_ID;
    }

    private static void AssertItemManagerExists() {
        if (p_Singleton == null) {
            throw new System.AccessViolationException("There is no " +
                "ItemManager in the scene.");
        }
    }
    #endregion
}
