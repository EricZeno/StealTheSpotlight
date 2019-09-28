using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementTesting : MonoBehaviour
{
    #region Instance Variables
    private Vector2 p_MoveDir;
    #endregion

    #region Cached Components
    private PlayerManager cc_Manager;
    private Rigidbody2D cc_Rb;
    #endregion

    #region Initialization
    private void Awake() {
        cc_Manager = GetComponent<PlayerManager>();
        cc_Rb = GetComponent<Rigidbody2D>();
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
        p_MoveDir = new Vector2(x, y);
        if (p_MoveDir.sqrMagnitude > 1) {
            p_MoveDir.Normalize();
        }
    }

    private void Move() {
        cc_Rb.MovePosition(cc_Rb.position + p_MoveDir * cc_Manager.GetMoveSpeed() * Time.fixedDeltaTime);
    }
    #endregion
}
