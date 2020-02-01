public struct Consts {
    #region Game Invariants
    public const int MAX_NUM_PLAYERS = 4;
    #endregion

    #region Scene Names
    public const string DUNGEON_SCENE_NAME = "Dungeon";
    #endregion

    #region Tags
    public const string GENERAL_ENEMY_TAG = "Enemy";
    public const string PLAYER_TAG = "Player";
    #endregion

    #region Layers
    public const string ITEM_PHYSICS_LAYER = "Item";
    public const string NO_ID_PLAYER_LAYER = "Player";
    public const string OBJECT_PHYSICS_LAYER = "Object";
    #endregion

    #region Player Animator Triggers
    public const string USE_WEAPON_ANIMATOR_TRIGGER = "UseWeapon";
    #endregion

    #region Player Multipliers
    public const float ATTACK_SPEED_MULTIPLIER = 1;
    #endregion

    #region General Player
    public const int NUM_MAX_PASSIVES_IN_INV = 5;
    public const int NUM_MAX_ACTIVES_IN_INV = 2;
    public const int NUM_MAX_WEAPONS_IN_INV = 1;

    public const int NUM_MAX_PLAYERS = 4;

    public const float TIME_INV_CYCLE_PAUSED = 0.2f;

    public const string WEAPON_OBJECT_NAME = "Weapon";

    public const float EXTERNAL_FORCE_REDUCTION_RATE = 0.8f;
    #endregion

    #region Bounding Consts
    public const float MINIMUM_ATTACK_SPEED = 0.001f;

    public const float SQR_MAG_CLOSE_TO_ONE_LOW = 0.95f;
    public const float SQR_MAG_CLOSE_TO_ONE_HIGH = 1.05f;
    public const float SQR_MAG_CLOSE_TO_ZERO_HIGH = 0.05f;

    public const float ALMOST_ZERO_THRESHOLD = 0.05f;
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
    public const string WEAPON_ITEM_BUNDLE_NAME = "weaponitems";
    #endregion

    #region Respawn
    public const int BASE_RESPAWN_TIME = 3;
    #endregion

    #region Points/Objectives
    public const string BOSS_NAME = "boss";
    public const string OBJROOM_NAME = "obj";
    public const string PK_GOLD_NAME = "pk_gold";
    public const string PK_SILVER_NAME = "pk_silver";
    public const string PK_BRONZE_NAME = "pk_bronze";
    public const string MONEY_GOLD_NAME = "money_gold";
    public const string MONEY_SILVER_NAME = "money_silver";
    public const string MONEY_BRONZE_NAME = "money_bronze";
    public const string ROOMS_GOLD_NAME = "rooms_gold";
    public const string ROOMS_SILVER_NAME = "rooms_silver";
    public const string ROOMS_BRONZE_NAME = "rooms_bronze";

    public const string BOSS_OBJECTIVE_TYPE = "boss";
    public const string OBJROOM_OBJECTIVE_TYPE = "obj";
    public const string MEDAL_PK_OBJECTIVE_TYPE = "medal_pk";
    public const string MEDAL_MONEY_OBJECTIVE_TYPE = "medal_money";
    public const string MEDAL_ROOMS_OBJECTIVE_TYPE = "medal_rooms";

    public const int BOSS_OBJECTIVE_POINTS = 10;
    public const int OBJROOM_OBJECTIVE_POINTS = 5;
    public const int MEDAL_GOLD_OBJECTIVE_POINTS = 3;
    public const int MEDAL_SILVER_OBJECTIVE_POINTS = 2;
    public const int MEDAL_BRONZE_OBJECTIVE_POINTS = 1;

    public const string MEDAL_FIRST = "gold";
    public const string MEDAL_SECOND = "silver";
    public const string MEDAL_THIRD = "bronze";

    //Can be changed
    public const int MOB_PARTICIPATION = 1;
    #endregion
}
