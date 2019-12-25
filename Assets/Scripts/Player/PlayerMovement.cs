using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerGraphics))]
public class PlayerMovement : MonoBehaviour {
    #region Constants
    private const float EXTERNAL_FORCE_REDUCTION_RATE = 0.8f;
    #endregion

    #region Variables
    #region Private Variables
    // This vector holds the player's current movement input before it's
    // translated to the rigidbody
    private Vector2 m_MoveDir;

    private Vector2 m_ExternalForce;
    #endregion

    #region Cached Components
    // The player's rigidbody
    private Rigidbody2D m_Rb;
    private PlayerManager m_Manager;

    private PlayerGraphics m_Graphics;
    #endregion
    #endregion

    #region Initialization
    private void Awake() {
        m_ExternalForce = Vector2.zero;
        SetupCachedComponents();
    }

    private void SetupCachedComponents() {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Manager = GetComponent<PlayerManager>();
        m_Graphics = GetComponent<PlayerGraphics>();
    }
    #endregion

    #region Main Updates
    private void Update() {
        UpdateExternalForce();
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
    }
    #endregion

    #region Movement
    private void Move() {
        Vector2 delta = m_MoveDir * m_Manager.GetPlayerData().CurrMovementSpeed;
        delta += m_ExternalForce;
        delta *= Time.fixedDeltaTime;
        m_Rb.MovePosition(m_Rb.position + delta);
    }
    #endregion

    #region External Forces
    public void ApplyExternalForce(Vector2 initialForce) {
        m_ExternalForce += initialForce;
    }

    private void UpdateExternalForce() {
        m_ExternalForce = Vector2.Lerp(m_ExternalForce, Vector2.zero, EXTERNAL_FORCE_REDUCTION_RATE);
    }
    #endregion

    #region Graphics
    private void UpdateGFX() {
        m_Graphics.Move(m_MoveDir);
        m_Graphics.FacingDirection(m_Manager.GetAimDir());
    }
    #endregion
}
