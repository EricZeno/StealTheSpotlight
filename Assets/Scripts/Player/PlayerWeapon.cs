using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerWeaponInterface {
    void Unequip();
    void Equip(int weaponID);

    void Use();
    void StopUse();
}

[DisallowMultipleComponent]
public class PlayerWeapon : MonoBehaviour, PlayerWeaponInterface {
    #region Private Variables
    private int p_Damage;

    private int p_AttackSpeed;

    private Sprite p_Sprite;

    private float p_WindupTime;

    private float p_AnimationLength;
    #endregion

    #region Interface Required Methods
    public void Equip(int weaponID) {
        throw new System.NotImplementedException();
    }

    public void Unequip() {
        throw new System.NotImplementedException();
    }

    public void Use() {
        throw new System.NotImplementedException();
    }

    public void StopUse() {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Input Receivers
    // Executes attack functionality when player attack input is received
    private void OnAttack() {
        Debug.Log("Detected attack input");
    }
    #endregion
}
