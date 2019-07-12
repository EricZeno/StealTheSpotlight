using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Variables
    // Which controller is associated with this player - editable in inspector
    [System.Serializable]
    private int m_PlayerNum;

    // String version of player num to facilitate dynamic controller selection
    private string m_PlayerString;

    // Contains left joystick input, which can be passed to separate movement function
    private Vector2 movementVector;

    // Contains right joystick input, which can be passed to separate aiming function
    private Vector2 aimVector;
    #endregion

    #region Start and Update
    private void Start()
    {
        m_PlayerString = m_PlayerNum.ToString();
    }

    void Update() {

        if (Input.GetButtonDown("Options")) {
            Pause();
        }

        if (IsInInventory()) {
            float xInput = Input.GetAxis("LeftStickX_P" + m_PlayerString;

            if (xInput > 0f) {
                CycleRight();
            } else if (xInput < 0f) {
                CycleLeft();
            }

            if (Input.GetButtonDown("Circle_P" + m_PlayerString)) {
                DropItem();
            }

            if (Input.GetButtonDown("R1_P" + m_PlayerString)) {
                CloseInventory();
            }
        } else {
            movementVector.x = Input.GetAxis("LeftStickX_P" + m_PlayerString);
            movementVector.y = Input.GetAxis("LeftStickY_P" + m_PlayerString);
            Move(movementVector);

            // Could also just rotate the player here instead of aiming separately (assuming all players have the same rotation speed)
            aimVector.x = Input.GetAxis("RightStickX_P" + m_PlayerString);
            aimVector.y = Input.GetAxis("RightStickY_P" + m_PlayerString);
            Aim(aimVector);

            if (Input.GetButton("R2_P" + m_PlayerString)) {
                Attack();
            }

            if (Input.GetButtonDown("L2_P" + m_PlayerString)) {
                UseAbility();
            }

            if (Input.GetButtonDown("R1_P" + m_PlayerString)) {
                OpenInventory();
            }

            if (Input.GetButtonDown("L1_P" + m_PlayerString)) {
                UseActive();
            }

            if (Input.GetButtonDown("Triangle_P" + m_PlayerString)) {
                SwapWeapons();
            }

            if (Input.GetButtonDown("X_P" + m_PlayerString)) {
                Interact();
            }
        }

    }
    #endregion

    #region Control Execution Methods

    // Placeholder methods that will eventually call other scripts

    // I see this script's function solely as input processing, not execution or filtering
    // Therefore, logic for determining whether an action is legal will be handled in the
    // relevant action script, as opposed to this script.

    // Probably means that these methods won't actually exist here, and I'll just call
    // the relevant methods (found in other scripts) directly in Update
     
    // Still will probably have to integrate whether or not inventory is open
    // (as right joystick will be cycle as opposed to aim, and attack might be disabled?)
    // Might require a separate control section specifically for navigating inventory

    // Will discuss this further at a relevant meeting.

    // Also block comments are broken for me ;-; pls help
    // Also, will delete all of my comments when I push to master, but convenient for me to think this way


    // All of these methods are pointless right now, just have them here to indicate what eventually needs to be referenced
    private void Move(Vector2 p_Movement) {
        throw new System.NotImplementedException();
    }

    private void Aim(Vector2 p_AimDirection) {
        throw new System.NotImplementedException();
    }

    private void Attack() {
        throw new System.NotImplementedException();
    }

    private void UseAbility() {
        throw new System.NotImplementedException();
    }

    private void SwapWeapons() {
        throw new System.NotImplementedException();
    }

    private bool IsInInventory() {
        throw new System.NotImplementedException();
    }

    private void Pause() {
        throw new System.NotImplementedException();
    }

    private void CycleLeft() {
        throw new System.NotImplementedException();
    }

    private void CycleRight() {
        throw new System.NotImplementedException();
    }

    private void OpenInventory() {
        throw new System.NotImplementedException();
    }

    private void CloseInventory() {
        throw new System.NotImplementedException();
    }

    private void UseActive() {
        throw new System.NotImplementedException();
    }
    #endregion
}
