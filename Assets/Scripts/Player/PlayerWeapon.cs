using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Input Receivers
    // Executes attack functionality when player attack input is received
    private void OnAttack() {
        Debug.Log("Detected attack input");
    }
    #endregion
}
