using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EUnitType {
    Player,
    Enemy,
    Boss
}
public enum Status {
    //Burn does damage over time to the unit
    Burn,
    //Freeze immobilizes the unit. It can still attack and cast spells
    Freeze,
    //Paralyze disables attacks for the unit
    Paralyze,
    //Poison increases damage taken and disables abilities for the unit
    Poison,
    //Slow reduces attack speed and movement speed
    Slow,
    //Invulnerability prevents all damage
    Invulnerable
}

[System.Serializable]
public abstract class UnitStats
{

    #region Editor Variables
    [SerializeField]
    [Tooltip("The power of a unit determines the strength of a unit, including the base amount of damage it deals, before weapon bonuses, and any healing it does")]
    private int m_Power;
    public int GetPower() {
            return m_Power;
    }

    [SerializeField]
    [Tooltip("The base movement speed of a unit, before any modifiers")]
    private float m_BaseMovementSpeed;
    public float BaseMovementSpeed {
        get {
            return m_BaseMovementSpeed;
        }
        set {
            m_BaseMovementSpeed = value;
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
    protected EUnitType p_Type;

    //The current movement speed of a unit
    private float p_CurrMovementSpeed;
    public float CurrMovementSpeed {
        get {
            return p_CurrMovementSpeed;
        }
    }

    //Currently not in use. Exists for potential use for Enemy and class power/health calculation
    private int p_Level;
    #endregion

    #region Resetters
    public virtual void ResetAllStatsDefault() {
        p_CurrMovementSpeed = m_BaseMovementSpeed;
        p_MaxHealth = m_BaseHealth;
        p_CurrHealth = m_BaseHealth;

        m_StatusEffects = new Dictionary<Status, bool>();
        foreach (Status s in System.Enum.GetValues(typeof(Status))) {
            m_StatusEffects[s] = false;
        }
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
        return m_StatusEffects[Status.Invulnerable];
    }

    public bool HasBurn() {
        return m_StatusEffects[Status.Burn];
    }
    public bool HasFreeze() {
        return m_StatusEffects[Status.Freeze];
    }
    public bool HasParalyze() {
        return m_StatusEffects[Status.Paralyze];
    }
    public bool HasPoison() {
        return m_StatusEffects[Status.Poison];
    }
    public bool HasSlow() {
        return m_StatusEffects[Status.Slow];
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
            //StartCoroutine(EffectDuration(duration, true, UnStun, EStatus.Burn));
        }
    }
    public void UnStun(Status status) {
        p_IsStunned = false;
    }

    //Rooted units cannot input movement commands
    private bool p_IsRooted = false;
    public void Root(float duration = -1) {
        p_IsRooted = true;

        //Replace this to instead call UnitManager.RemoveEffect, which should start this coroutine
        if (duration != -1) {
            //StartCoroutine(EffectDuration(duration, true, UnRoot, EStatus.Burn));
        }
    }
    public void UnRoot(Status status) {
        p_IsRooted = false;
    }

    //Determines whether a unit has attacks enabled
    private bool p_CanAttack = true;
    public void Disarm(float duration = -1) {
        p_CanAttack = false;

        //Replace this to instead call UnitManager.RemoveEffect, which should start this coroutine
        if (duration != -1) {
            //StartCoroutine(EffectDuration(duration, true, Rearm, EStatus.Burn));
        }
    }
    public void Rearm(Status status) {
        p_CanAttack = true;
    }
    #endregion

    #region Health
    [SerializeField]
    [Tooltip("Base health is the health a unit has by default, before any modifiers.")]
    private int m_BaseHealth;

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
            throw new System.ArgumentException("Damage cannot be negative.");
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
            throw new System.ArgumentException("Healing cannot be negative.");
        }

        p_CurrHealth += healing;
        if (p_CurrHealth > MaxHealth) {
            p_CurrHealth = MaxHealth;
        }
    }

    public void AddXPercBaseHealth(float multiplier) {
        if (multiplier < 0) {
            throw new System.ArgumentException("Percent must be larger than or equal to 0.");
        }

        int tempMaxHealth = p_MaxHealth;
        p_MaxHealth += (int)(m_BaseHealth * (multiplier - 1));
        p_CurrHealth = p_CurrHealth * p_MaxHealth / tempMaxHealth;
    }

    public void SubtractXPercBaseHealth(float multiplier) {
        if (multiplier < 0) {
            throw new System.ArgumentException("Percent must be larger than or equal to 0.");
        }

        int tempMaxHealth = p_MaxHealth;
        p_MaxHealth -= (int)(m_BaseHealth * (multiplier - 1));
        p_CurrHealth = p_CurrHealth * p_MaxHealth / tempMaxHealth;
    }
    #endregion

    #region Status Effects
    private Dictionary<Status, bool> m_StatusEffects;

    public void AddStatus(Status status) {
        m_StatusEffects[status] = true;
    }

    public void RemoveStatus(Status status) {
        m_StatusEffects[status] = false;
    }
    #endregion

    #region Stat Modifiers
    public void AddXPercBaseSpeed(float multiplier) {
        if (multiplier < 0) {
            throw new System.ArgumentException("Percent must be larger than or equal to 0.");
        }

        p_CurrMovementSpeed += m_BaseMovementSpeed * multiplier;
    }

    public void SubtractXPercBaseSpeed(float multiplier) {
        if (multiplier < 0) {
            throw new System.ArgumentException("Percent must be larger than or equal to 0.");
        }

        p_CurrMovementSpeed -= m_BaseMovementSpeed * multiplier;
    }
    #endregion

    #region Misc
    //Move these to the UnitManager

    private delegate void EffectRemover(Status status);

    //This is a timer that will be started whenever an effect is applied to the unit.
    private IEnumerator EffectDuration(float duration, bool isCrowdControl, EffectRemover RemoveEffect, Status status) {
        if (isCrowdControl) {
            duration = duration * (1 - Tenacity);
        }
        yield return new WaitForSeconds(duration);
        RemoveEffect(status);
    }
    #endregion
}
