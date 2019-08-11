using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UnitStats : MonoBehaviour
{
    public GameObject unit;

    #region Attributes
    //Determines the type of the unit for external use
    enum Type {player, enemy, boss}
    private Type p_type;

    //Stunned units cannot input movement, attack, ability, or inventory commands
    private bool p_isStunned = false;

    //Rooted units cannot input movement commands
    private bool p_isRooted = false;
    #endregion

    #region Health
    //Base health is the health a unit has by default, before any modifiers
    private int p_BaseHealth;

    //Max health is after all modifiers. This is the effective health
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
        p_CurrHealth -= damage;
        if (p_CurrHealth < 0)
        {
            KillUnit();
        }
    }

    public void Heal(int healing)
    {
        TakeDamage(-healing);
        if (p_CurrHealth > MaxHealth)
        {
            p_CurrHealth = MaxHealth;
        }
    }
    #endregion

    #region Constructor
    public UnitStats(int unitType, int baseHealth)
    {
        p_type = TypeCheck(unitType);

        p_BaseHealth = baseHealth;
        MaxHealth = p_BaseHealth;
    }
    #endregion

    #region Checkers
    private bool IsPlayer()
    {
        return p_type == Type.player;
    }
    private bool IsEnemy()
    {
        return p_type == Type.enemy;
    }
    private bool IsBoss()
    {
        return p_type == Type.boss;
    }

    private bool IsStunned()
    {
        return p_isStunned;
    }
    private bool IsRooted()
    {
        return p_isRooted;
    }

    private bool IsAlive()
    {
        return p_CurrHealth > 0;
    }
    #endregion

    #region Misc
    private Type TypeCheck(int value)
    {
        switch(value)
        {
            case 0: return Type.player;
            case 1: return Type.enemy;
            case 2: return Type.boss;
            default: return Type.player;
        }
    }

    private void KillUnit()
    {
        unit.SetActive(false);
    }
    #endregion
}
