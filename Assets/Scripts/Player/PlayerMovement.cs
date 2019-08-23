using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Private Variables

    // Will likely have this sourced from player data, not set via inspector
    [SerializeField]
    [Tooltip("The player's movement speed.")]
    private float m_Speed;
                            

    // This vector holds the player's current movement input before it's
    // translated to the rigidbody
    private Vector2 p_Move;

    // The player's rigidbody
    private Rigidbody2D cc_rb;

    #endregion

    #region Startup

    // Start is called before the first frame update
    void Start()
    {
        cc_rb = GetComponent<Rigidbody2D>();
    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVector = new Vector3(p_Move.x, p_Move.y);
        moveVector *= Time.deltaTime * m_Speed;
        cc_rb.MovePosition(transform.position + moveVector);
    }

    #endregion

    #region Input Receivers

    // Detects left trigger movement input
    private void OnMove(InputValue value)
    {
        p_Move = value.Get<Vector2>();
    }

    // Detects right trigger aiming input, will be used to rotate the player
    // Potentially interacts with weapon?
    private void OnAim(InputValue value)
    {
        Debug.Log("Detected aim input");
    }

    private void OnOpenInventory()
    {
        p_Move = new Vector2(0f, 0f);
    }

    #endregion
}
