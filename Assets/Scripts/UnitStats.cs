using UnityEngine;

public enum EUnitType {
    Player,
    Enemy,
    Boss
}
public enum EStatus {
    //Burn does damage over time to the unit
    Burn,
    //Freeze immobilizes the unit. It can still attack and cast spells
    Freeze,
    //Paralyze disables attacks for the unit
    Paralyze,
    //Poison increases damage taken and disables abilities for the unit
    Poison,
    //Slow reduces attack speed and movement speed
    Slow
}

//[System.Serializable]
public abstract class UnitStats
{

    #region Editor Variables
    [SerializeField]
    [Tooltip("The power of a unit determines the strength of a unit, including the base amount of damage it deals, before weapon bonuses, and any healing it does")]
    private int p_Power;
    public int Power {
        get {
            return p_Power;
        }
        set {
            p_Power = value;
        }
    }

    [SerializeField]
    [Tooltip("The base movement speed of a unit, before any modifiers")]
    private int p_BaseMovementSpeed;
    public int BaseMovementSpeed {
        get {
            return p_BaseMovementSpeed;
        }
        set {
            p_BaseMovementSpeed = value;
        }
    }

    [SerializeField]
    [Tooltip("Tenacity multiplicatively reduces the duration of crowd control (stun, root...) applied to the unit")]
    [Range(0, 1)]
    private int p_Tenacity;
    public int Tenacity {
        get {
            return p_Tenacity;
        }
        set {
            p_Tenacity = value;
        }
    }
    #endregion

    #region Private Variables
    //Determines the type of the unit for external use
    private EUnitType p_Type;

    //The current movement speed of a unit
    private int p_CurrMovementSpeed;
    public int CurrMovementSpeed {
        get {
            return p_CurrMovementSpeed;
        }
    }

    //Currently not in use. Exists for potential use for Enemy and class power/health calculation
    private int p_Level;
    #endregion

    #region Constructor

    protected UnitStats(EUnitType unitType, int baseHealth) {
        p_Type = unitType;

        p_BaseHealth = baseHealth;
        MaxHealth = p_BaseHealth;
    }
    #endregion

    #region Checkers
    public bool IsPlayer() {
        return p_Type == EUnitType.Player;
    }
    public bool IsEnemy() {
        return p_Type == EUnitType.Enemy;
    }
    public bool IsBoss() {
        return p_Type == EUnitType.Boss;
    }

    public bool IsStunned() {
        return p_IsStunned;
    }
    public bool IsRooted() {
        return p_IsRooted;
    }
    public bool CanAttack() {
        return p_CanAttack;
    }
    public bool IsInvuln() {
        return p_IsInvuln;
    }

    public bool HasBurn() {
        return p_Burn;
    }
    public bool HasFreeze() {
        return p_Freeze;
    }
    public bool HasParalyze() {
        return p_Paralyze;
    }
    public bool HasPoison() {
        return p_Poison;
    }
    public bool HasSlow() {
        return p_Slow;
    }

    private bool IsAlive() {
        return p_CurrHealth > 0;
    }
    #endregion

    #region Crowd Control
    //Stunned units cannot input movement, attack, ability, or inventory commands
    private bool p_IsStunned = false;
    public void Stun(float duration = -1) {
        p_IsStunned = true;

        //Replace this to instead call UnitManager.RemoveEffect, which should start this coroutine
        if (duration != -1) {
            StartCoroutine(EffectDuration(duration, true, UnStun, EStatus.Burn));
        }
    }
    public void UnStun(EStatus status) {
        p_IsStunned = false;
    }

    //Rooted units cannot input movement commands
    private bool p_IsRooted = false;
    public void Root(float duration = -1) {
        p_IsRooted = true;

        //Replace this to instead call UnitManager.RemoveEffect, which should start this coroutine
        if (duration != -1) {
            StartCoroutine(EffectDuration(duration, true, UnRoot, EStatus.Burn));
        }
    }
    public void UnRoot(EStatus status) {
        p_IsRooted = false;
    }

    //Invulnerable units cannot lose health
    private bool p_IsInvuln = false;
    public void Invuln(float duration = -1) {
        p_IsInvuln = true;

        //Replace this to instead call UnitManager.RemoveEffect, which should start this coroutine
        if (duration != -1) {
            StartCoroutine(EffectDuration(duration, true, UnInvuln, EStatus.Burn));
        }
    }
    public void UnInvuln(EStatus status) {
        p_IsInvuln = false;
    }

    //Determines whether a unit has attacks enabled
    private bool p_CanAttack = true;
    public void Disarm(float duration = -1) {
        p_CanAttack = false;

        //Replace this to instead call UnitManager.RemoveEffect, which should start this coroutine
        if (duration != -1) {
            StartCoroutine(EffectDuration(duration, true, Rearm, EStatus.Burn));
        }
    }
    public void Rearm(EStatus status) {
        p_CanAttack = true;
    }
    #endregion

    #region Health
    //Base health is the health a unit has by default, before any modifiers
    private int p_BaseHealth;

    //Max health is after all modifiers. This is the effective health.
    private int p_MaxHealth;
    public int MaxHealth {
        get {
            return p_MaxHealth;
        }
    }

    //The current amount of hitpoints the unit has
    private int p_CurrHealth;
    public int CurrHealth {
        get {
            return p_CurrHealth;
        }
    }

    public void TakeDamage(int damage) {
        if (damage < 0) {
            Debug.Log("damage can't be negative");
            return;
        }

        if (IsInvuln()) {
            return;
        }

        p_CurrHealth -= damage;
        if (p_CurrHealth <= 0) {
            p_CurrHealth = 0;
            //Call UnitManager.KillUnit here.
        }
    }

    public void Heal(int healing) {
        if (healing < 0) {
            Debug.Log("healing can't be negative");
            return;
        }

        p_CurrHealth += healing;
        if (p_CurrHealth > MaxHealth) {
            p_CurrHealth = MaxHealth;
        }
    }

    public void AddHealth(int health) {
        int tempMaxHealth = p_MaxHealth;
        p_MaxHealth += health;
        p_CurrHealth = p_CurrHealth * p_MaxHealth / tempMaxHealth;
    }
    #endregion

    #region Status Effects
    //This will be refactored into a dictionary with keys = EStatus and value = Bool later
    //The variables below determine whether a unit is afflicted by any status effects
    private bool p_Burn = false;
    private bool p_Freeze = false;
    private bool p_Paralyze = false;
    private bool p_Poison = false;
    private bool p_Slow = false;

    public void AddStatus(EStatus status, bool isCrowdControl, float duration = -1) {
        switch (status) {
            case EStatus.Burn:
                p_Burn = true;
                break;
            case EStatus.Freeze:
                p_Freeze = true;
                break;
            case EStatus.Paralyze:
                p_Paralyze = true;
                break;
            case EStatus.Poison:
                p_Poison = true;
                break;
            case EStatus.Slow:
                p_Slow = true;
                break;
        }

        //Replace this to instead call UnitManager.RemoveEffect, which should start this coroutine
        if (duration != -1) {
            StartCoroutine(EffectDuration(duration, isCrowdControl, RemoveStatus, status));
        }
    }

    private void RemoveStatus(EStatus status) {
        switch (status) {
            case EStatus.Burn:
                p_Burn = false;
                break;
            case EStatus.Freeze:
                p_Freeze = false;
                break;
            case EStatus.Paralyze:
                p_Paralyze = false;
                break;
            case EStatus.Poison:
                p_Poison = false;
                break;
            case EStatus.Slow:
                p_Slow = false;
                break;
        }
    }
    #endregion

    #region Misc
    //Move these to the UnitManager

    private delegate void EffectRemover(EStatus status);

    //This is a timer that will be started whenever an effect is applied to the unit.
    private IEnumerator EffectDuration(float duration, bool isCrowdControl, EffectRemover RemoveEffect, EStatus status) {
        if (isCrowdControl) {
            duration = duration * (1 - Tenacity);
        }
        yield return new WaitForSeconds(duration);
        RemoveEffect(status);
    }
    #endregion
}
