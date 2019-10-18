using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour {
    #region Private Variables
    // This vector holds the player's current movement input before it's
    // translated to the rigidbody
    private Vector2 m_MoveDir;
    #endregion

    #region Cached Components
    // The player's rigidbody
    private Rigidbody2D m_Rb;
    private PlayerManager m_Manager;

    private Animator m_Animator;
    #endregion

    #region Initialization
    private void Awake() {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Manager = GetComponent<PlayerManager>();
        m_Animator = GetComponent<Animator>();
    }
    #endregion

    #region Main Updates
    private void Update() {
        UpdateGFX();
    }

    private void FixedUpdate() {
        Move();
    }
    #endregion

    #region Input Receivers
    // Detects left trigger movement input
    private void OnMove(InputValue value) {
        m_MoveDir = value.Get<Vector2>();
        m_MoveDir.Normalize();
        Debug.Log(m_MoveDir);
    }
    #endregion

    #region Movement
    private void Move() {
        Vector2 delta = m_MoveDir * Time.fixedDeltaTime;
        delta *= m_Manager.GetPlayerData().CurrMovementSpeed;
        m_Rb.MovePosition(m_Rb.position + delta);
    }
    #endregion

    #region Graphics
    private void UpdateGFX() {
        m_Animator.SetFloat("Horizontal", m_Manager.GetAimDir().x);
        m_Animator.SetFloat("Vertical", m_Manager.GetAimDir().y);
    }
    #endregion
}
