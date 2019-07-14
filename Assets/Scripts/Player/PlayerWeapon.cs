using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerWeaponInterface {
    void UnsetWeapon();
    void SetWeapon(int weaponID);
}

[DisallowMultipleComponent]
public class PlayerWeapon : MonoBehaviour, PlayerWeaponInterface {

    #region Input Receivers
    // Executes attack functionality when player attack input is received
    private void OnAttack() {
        Debug.Log("Detected attack input");
    }
    #endregion
}
