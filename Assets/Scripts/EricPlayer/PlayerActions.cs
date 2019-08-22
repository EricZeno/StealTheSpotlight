using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.PlayerInput;

public class PlayerActions : MonoBehaviour
{

	private Vector2 m_Move;
	private float m_Speed = 2.5f;

    void Update()
	{
		Vector2 move = new Vector2(m_Move.x, m_Move.y) * Time.deltaTime * m_Speed;
		transform.Translate(move);
	}

	private void OnMove(InputValue value)
	{
		Debug.Log("Detected movement input");
		m_Move = value.Get<Vector2>();
	}

	private void OnAim(InputValue value)
	{
		Debug.Log("Detected aim input");
	}

	private void OnAttack()
	{
		Debug.Log("Detected attack input");
	}

	private void OnUseAbility()
	{
		Debug.Log("Detected ability input");
	}

	private void OnSwapWeapons()
	{
		Debug.Log("Detected swap input");
	}

	private void OnPause()
	{
		Debug.Log("Detected pause input");
	}

	private void OnCycleLeft()
	{
		Debug.Log("Detected left cycle input");
	}

	private void OnCycleRight()
	{
		Debug.Log("Detected right cycle input");
	}

	private void OnOpenInventory()
	{
		//PlayerControls.Gameplay.Disable();
		//PlayerControls.Inventory.Enable();
	}

	private void OnCloseInventory()
	{
		//PlayerControls.Inventory.Disable();
		//PlayerControls.Gameplay.Enable();
	}

	private void OnUseActive()
	{
		Debug.Log("Detected active input");
	}

	private void OnDropItem()
	{
		Debug.Log("Detected drop input");
	}

	private void OnInteract()
	{
		Debug.Log("Detected interact input");
	}
}
