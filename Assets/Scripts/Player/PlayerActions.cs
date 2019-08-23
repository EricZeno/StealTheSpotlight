using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.PlayerInput;

// THIS SCRIPT IS TO BE DEPRECATED

// Each of its functions should be placed in the relevant script
// (e.g. OnMove has been transferred to PlayerMovement)
// After being moved, the method should be deleted from this script

// The script is being maintained currently to test functionality that
// hasn't yet been implemented (e.g. Using Abilities)

public class PlayerActions : MonoBehaviour
{

    private void OnPause()
    {
        Debug.Log("Detected pause input");
    }

    private void OnUseActive()
    {
        Debug.Log("Detected active input");
    }

    // Interaction seems to intersect with A LOT of different systems
    // depending on what you're interacting with. Should have a discussion on
    // how to implement this.
    private void OnInteract()
    {
        Debug.Log("Detected interact input");
    }

    // Can do any join behavior we want here
    private void OnPlayerJoined()
	{

	}
}
