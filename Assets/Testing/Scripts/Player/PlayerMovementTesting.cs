using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementTesting : MonoBehaviour
{
    #region Instance Variables
    private Vector2 m_MoveDir;
    #endregion

    #region Cached Components
    private PlayerManager m_Manager;
    private Rigidbody2D m_Rb;
    #endregion

    #region Initialization
    private void Awake() {
        m_Manager = GetComponent<PlayerManager>();
        m_Rb = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region Main Updates
    private void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        SetMoveDir(x, y);
    }

    private void FixedUpdate() {
        Move();
    }
    #endregion

    #region Movement Methods
    private void SetMoveDir(float x, float y) {
        // TODO: Error checking on x and y
        m_MoveDir = new Vector2(x, y);
        if (m_MoveDir.sqrMagnitude > 1) {
            m_MoveDir.Normalize();
        }
    }

    private void Move() {
        m_Rb.MovePosition(m_Rb.position + m_MoveDir * m_Manager.GetMoveSpeed() * Time.fixedDeltaTime);
    }
    #endregion
}
