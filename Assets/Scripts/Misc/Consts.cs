public struct Consts
{
    #region Tags
    public const string GENERAL_ENEMY_TAG = "Enemy";

    public const string PLAYER_TAG = "Player";
    #endregion

    #region Layers
    public const string ITEM_PHYSICS_LAYER = "Item";
    #endregion

    #region Player Animator Triggers
    public const string USE_WEAPON_ANIMATOR_TRIGGER = "UseWeapon";
    #endregion

    #region Player Multipliers
    public const float ATTACK_SPEED_MULTIPLIER = 1;
    #endregion

    #region General Player
    public const string INVENTORY_INPUT_ACTION_MAP_NAME = "Inventory";
    public const string GAMEPLAY_INPUT_ACTION_MAP_NAME = "Gameplay";

    public const int NUM_MAX_PASSIVES_IN_INV = 10;

    public const float TIME_INV_CYCLE_PAUSED = 0.2f;
    #endregion

    #region Bounding Consts
    public const float MINIMUM_ATTACK_SPEED = 0.001f;

    public const float SQR_MAG_CLOSE_TO_ONE_LOW = 0.95f;
    public const float SQR_MAG_CLOSE_TO_ONE_HIGH = 1.05f;
    public const float SQR_MAG_CLOSE_TO_ZERO_HIGH = 0.05f;
    #endregion

    #region Buttons
    public const string TEST_BUTTON_1 = "TestButton1";
    public const string TEST_BUTTON_2 = "TestButton2";
    public const string TEST_BUTTON_3 = "TestButton3";

    public const float INV_CYCLE_THRESHOLD = 0.5f;
    #endregion

    #region Text File Paths
    public const string WEAPON_TYPES_DATA_FILE = "Items/WeaponTypesData";
    #endregion

    #region Sprite File Paths
    public const string TEST_SWORD_1_SPRITE_PATH = "Sprites/Weapons/ShortBlade/Sword-1";
    public const string TEST_SWORD_2_SPRITE_PATH = "Sprites/Weapons/ShortBlade/Sword-2";
    #endregion

    #region Null Values
    public const int NULL_ITEM_ID = -1;
    #endregion

    #region Item ID Boundaries
    public const int MIN_PASSIVE_ITEM_ID = 0;
    public const int MAX_PASSIVE_ITEM_ID = 3333;

    public const int MIN_ACTIVE_ITEM_ID = 3334;
    public const int MAX_ACTIVE_ITEM_ID = 6666;

    public const int MIN_WEAPON_ITEM_ID = 6667;
    public const int MAX_WEAPON_ITEM_ID = 9999;
    #endregion

    #region Item Type Names
    public const string PASSIVE_ITEM_NAME = "passive";
    public const string ACTIVE_ITEM_NAME = "active";
    public const string WEAPON_ITEM_NAME = "weapon";
    #endregion

    #region AssetBundles
    public const string ASSETBUNDLES_DIRECTORY = "AssetBundles";
    public const string RUNTIME_STREAMING_ASSETS_DIRECTORY = "MunchkinCrawler_Data/StreamingAssets";
    public const string PASSIVE_ITEM_BUNDLE_NAME = "passiveitems";
    public const string ACTIVE_ITEM_BUNDLE_NAME = "activeitems";
    #endregion

    #region Respawn
    public const int BASE_RESPAWN_TIME = 10;
    #endregion
}
