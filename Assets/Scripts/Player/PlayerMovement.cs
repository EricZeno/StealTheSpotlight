using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour {
    #region Editor Variables
    // Will likely have this sourced from player data, not set via inspector
    [SerializeField]
    [Tooltip("The player's movement speed.")]
    private float m_Speed;
    #endregion

    #region Private Variables
    // This vector holds the player's current movement input before it's
    // translated to the rigidbody
    private Vector2 p_Move;
    #endregion

    #region Cached Components
    // The player's rigidbody
    private Rigidbody2D cc_Rb;
    #endregion

    #region Initialization
    private void Awake() {
        cc_Rb = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region Main Updates
    private void Update() {
        float horiz = Input.GetAxis("Horizontal");
        p_Move.x = horiz;
        Debug.Log(p_Move);
    }

    private void FixedUpdate() {
        cc_Rb.MovePosition(cc_Rb.position + p_Move * Time.fixedDeltaTime * m_Speed);
    }
    #endregion

    #region Input Receivers
    // Detects left trigger movement input
    private void OnMove(InputValue value) {
        //p_Move = value.Get<Vector2>();
        //p_Move.Normalize();
        Debug.Log(value);
    }

    // Detects right trigger aiming input, will be used to rotate the player
    // Potentially interacts with weapon?
    private void OnAim(InputValue value) {
        Debug.Log("Detected aim input");
    }
    #endregion
}
