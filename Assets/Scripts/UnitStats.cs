using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type { player, enemy, boss }
public enum Status { burn, freeze, paraylze, poison, slow }

[System.Serializable]
public abstract class UnitStats
{
    #region Private Attributes
    //Determines the type of the unit for external use
    private Type p_Type;

    //Stunned units cannot input movement, attack, ability, or inventory commands
    private bool p_IsStunned = false;
    public void Stun(float duration = -1)
    {
        p_IsStunned = true;

        if (duration != -1)
        {

        }
    }
    public void UnStun()
    {
        p_IsStunned = false;
    }

    //Rooted units cannot input movement commands
    private bool p_IsRooted = false;
    public void Root(float duration = -1)
    {
        p_IsRooted = true;

        if (duration != -1)
        {

        }
    }
    public void UnRoot()
    {
        p_IsRooted = false;
    }

    //Invulnerable units cannot lose health
    private bool p_IsInvuln = false;
    public void Invuln(float duration = -1)
    {
        p_IsInvuln = true;

        if (duration != -1)
        {

        }
    }
    public void UnInvuln()
    {
        p_IsInvuln = false;
    }

    //Determines whether a unit has attacks enabled
    private bool p_CanAttack = true;
    public void Disarm(float duration = -1)
    {
        p_CanAttack = false;

        if (duration != -1)
        {

        }
    }
    public void Rearm()
    {
        p_CanAttack = true;
    }

    //Currently not in use. Exists for potential use for Enemy and class power/health calculation
    private int p_Level;
    #endregion

    #region Public Attributes
    [SerializeField]
    [Tooltip("The power of a unit determines the base amount of damage it deals, before weapon bonuses")]
    private int p_Power;
    public int Power
    {
        get
        {
            return p_Power;
        }
        set
        {
            p_Power = value;
        }
    }

    [SerializeField]
    [Tooltip("The base movement speed of a unit, before any modifiers")]
    private int p_BaseMovementSpeed;
    public int BaseMovementSpeed
    {
        get
        {
            return p_BaseMovementSpeed;
        }
        set
        {
            p_BaseMovementSpeed = value;
        }
    }

    [SerializeField]
    [Tooltip("The current movement speed of a unit")]
    private int p_CurrMovementSpeed;
    public int CurrMovementSpeed
    {
        get
        {
            return p_CurrMovementSpeed;
        }
    }

    [SerializeField]
    [Tooltip("Tenacity reduces crowd control duration by a percentage.")]
    [Range(0, 1)]
    private int p_Tenacity;
    public int Tenacity
    {
        get
        {
            return p_Tenacity;
        }
        set
        {
            p_Tenacity = value;
        }
    }
    #endregion

    #region Health
    //Base health is the health a unit has by default, before any modifiers
    private int p_BaseHealth;

    [SerializeField]
    [Tooltip("Max health is after all modifiers. This is the effective health.")]
    private int p_MaxHealth;
    public int MaxHealth
    {
        get
        {
            return p_MaxHealth;
        }
        set
        {
            int tempMaxHealth = p_MaxHealth;
            p_MaxHealth = value;
            p_CurrHealth = p_CurrHealth * p_MaxHealth / tempMaxHealth;
        }
    }

    //The current amount of hitpoints the unit has
    private int p_CurrHealth;

    public void TakeDamage(int damage)
    {
        if(IsInvuln())
        {
            return;
        }

        p_CurrHealth -= damage;
        if (p_CurrHealth <= 0)
        {
            KillUnit();
        }
    }

    public void Heal(int healing)
    {
        p_CurrHealth += healing;
        if (p_CurrHealth > MaxHealth)
        {
            p_CurrHealth = MaxHealth;
        }
    }
    #endregion

    #region Status Effects
    //The variables below determine whether a unit is afflicted by any status effects
    private bool p_Burn = false;
    private bool p_Freeze = false;
    private bool p_Paralyze = false;
    private bool p_Poison = false;
    private bool p_Slow = false;

    public void AddStatus(Status status, float duration = -1)
    {
        switch(status)
        {
            case Status.burn: p_Burn = true;
                break;
            case Status.freeze: p_Freeze = true;
                break;
            case Status.paraylze: p_Paralyze = true;
                break;
            case Status.poison: p_Poison = true;
                break;
            case Status.slow: p_Slow = true;
                break;
        }

        if (duration != -1)
        {

        }
    }

    public void RemoveStatus(Status status)
    {
        switch (status)
        {
            case Status.burn:
                p_Burn = false;
                break;
            case Status.freeze:
                p_Freeze = false;
                break;
            case Status.paraylze:
                p_Paralyze = false;
                break;
            case Status.poison:
                p_Poison = false;
                break;
            case Status.slow:
                p_Slow = false;
                break;
        }
    }
    #endregion

    #region Constructor
    public UnitStats(Type unitType, int baseHealth)
    {
        p_Type = unitType;

        p_BaseHealth = baseHealth;
        MaxHealth = p_BaseHealth;
    }
    #endregion

    #region Checkers
    public bool IsPlayer()
    {
        return p_Type == Type.player;
    }
    public bool IsEnemy()
    {
        return p_Type == Type.enemy;
    }
    public bool IsBoss()
    {
        return p_Type == Type.boss;
    }

    public bool IsStunned()
    {
        return p_IsStunned;
    }
    public bool IsRooted()
    {
        return p_IsRooted;
    }
    public bool CanAttack()
    {
        return p_CanAttack;
    }
    public bool IsInvuln()
    {
        return p_IsInvuln;
    }

    public bool HasBurn()
    {
        return p_Burn;
    }
    public bool HasFreeze()
    {
        return p_Freeze;
    }
    public bool HasParalyze()
    {
        return p_Paralyze;
    }
    public bool HasPoison()
    {
        return p_Poison;
    }
    public bool HasSlow()
    {
        return p_Slow;
    }

    private bool IsAlive()
    {
        return p_CurrHealth > 0;
    }
    #endregion

    #region Misc
    private IEnumerator EffectDuration(float duration, bool isCrowdControl)
    {
        if (isCrowdControl)
        {
            duration = duration * (1 - Tenacity);
        }
        while (duration > 0) {
            duration -= Time.deltaTime;
            yield return null;
        }
    }
    private void KillUnit()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
