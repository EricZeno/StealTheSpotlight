using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistAttack : EnemyAttack
{
	#region Private Variables
	private bool m_AttackLocked;
	private int m_Damage;
	#endregion

	#region Editor Variables
	[SerializeField]
	[Tooltip("The rune gameobject")]
	private GameObject m_Rune;

	[SerializeField]
	[Tooltip("How long it takes for the rune to do damage")]
	private float m_CastTime;

	[SerializeField]
	[Tooltip("Radius of the rune")]
	private float m_RuneRadius;

	[SerializeField]
	[Tooltip("The sprite that the rune flashes to when damaging")]
	private Sprite m_FlashSprite;
	#endregion

	#region Initialization
	private void Awake()
	{
		base.Awake();
		m_Damage = m_Manager.GetEnemyData().Damage;
	}
	#endregion

	#region Attack
	public override void Attack(int attackNum, Vector2 target)
	{
		if (!m_AttackLocked)
		{
			StartCoroutine(Attack(target));
		}
	}

	private IEnumerator Attack(Vector2 target)
	{
		m_AttackLocked = true;
		GameObject rune = Instantiate(m_Rune, target, Quaternion.identity);
		rune.transform.position = target;
		rune.GetComponent<RuneDestroyer>().time = m_CastTime + .4f;
		rune.SetActive(true);
		yield return new WaitForSeconds(m_CastTime);

		StartCoroutine(RuneFlash(rune));

		RaycastHit2D[] targets = Physics2D.CircleCastAll(rune.transform.position, m_RuneRadius, Vector3.up, 0.01f);
		foreach (RaycastHit2D hit in targets)
		{
			if (hit.collider.gameObject.tag == Consts.PLAYER_TAG)
			{
				PlayerManager player = hit.collider.gameObject.GetComponent<PlayerManager>();
				player.TakeDamage(m_Damage);
			}
		}

		yield return new WaitForSeconds(.4f);
		m_AttackLocked = false;
		((CultistMovement)m_Movement).Attacking = false;
	}

	private IEnumerator RuneFlash(GameObject rune)
	{
		float flashDelay = .2f;
		int numOfFlashes = 1;

		SpriteRenderer runeSR = rune.GetComponent<SpriteRenderer>();
		Sprite currSprite = runeSR.sprite;

		for (int i = 0; i < numOfFlashes; i++)
		{
			runeSR.sprite = m_FlashSprite;
			yield return new WaitForSeconds(flashDelay);
			runeSR.sprite = currSprite;
			yield return new WaitForSeconds(flashDelay);
		}
	}
	#endregion

}
