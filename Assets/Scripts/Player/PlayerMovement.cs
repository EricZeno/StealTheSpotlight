using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour {
    #region Constants
    private const float KNOCKBACK_REDUCTION_RATE = 0.8f;
    #endregion

    #region Variables
    #region Private Variables
    // This vector holds the player's current movement input before it's
    // translated to the rigidbody
    private Vector2 m_MoveDir;

    private Vector2 m_KnockbackForce;
    #endregion

    #region Cached Components
    // The player's rigidbody
    private Rigidbody2D m_Rb;
    private PlayerManager m_Manager;

    private Animator m_Animator;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_KnockbackForce = Vector2.zero;

        m_Rb = GetComponent<Rigidbody2D>();
        m_Manager = GetComponent<PlayerManager>();
        m_Animator = GetComponent<Animator>();
    }
    #endregion

    #region Main Updates
    private void Update() {
        UpdateKnockbackForce();
        // TODO: fix this
        //UpdateGFX(); Current weapon system does not work with player animations
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
    }
    #endregion

    #region Movement
    private void Move() {
        Vector2 delta = m_MoveDir * m_Manager.GetPlayerData().CurrMovementSpeed;
        delta += m_KnockbackForce;
        delta *= Time.fixedDeltaTime;
        m_Rb.MovePosition(m_Rb.position + delta);
    }
    #endregion

    #region Knockback
    public void ApplyKnockback(Vector2 initialForce) {
        m_KnockbackForce = initialForce;
    }

    private void UpdateKnockbackForce() {
        m_KnockbackForce = Vector2.Lerp(m_KnockbackForce, Vector2.zero, KNOCKBACK_REDUCTION_RATE);
    }
    #endregion

    #region Graphics
    private void UpdateGFX() {
        m_Animator.SetFloat("Horizontal", m_Manager.GetAimDir().x);
        m_Animator.SetFloat("Vertical", m_Manager.GetAimDir().y);
    }
    #endregion
}
