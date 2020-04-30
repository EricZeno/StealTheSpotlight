using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FidgeProjectile : MonoBehaviour
{
    #region Private Variables
    private int m_Damage;
    private float m_Knockback;

    #endregion

    #region Cached Components
    private Rigidbody2D m_Rb;
    private SpriteRenderer m_Renderer;
    private BoxCollider2D m_Collider;
    private PlayerManager m_Manager;
    private BaseWeaponItem m_WeaponData;
    private AudioManager m_AudioManager;
    private Transform spinner;
    private bool m_Outbound;
    private Vector3 m_PlayerLocation;
    #endregion

    #region Initialization
    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_AudioManager = GetComponent<AudioManager>();
        spinner = GetComponent<Transform>();
    }
    #endregion

    #region Projectile Setup Methods

    public void Setup(int damage, Vector2 dir, float speed, PlayerManager player, float knockback )
    {
        SetDamage(damage);
        SetVelocity(dir, speed);
        m_Manager = player;
        m_Knockback = knockback;
        m_Outbound = true;
    }

    private void SetDamage(int damage)
    {
        if (damage < 0)
        {
            throw new System.ArgumentException("When setting the damage for " +
                "a projectile, make sure that it is larger than or equal to" +
                "zero");
        }
        m_Damage = damage;
    }

    IEnumerator BoomerangReturn() {
        yield return new WaitForSeconds(2);
        if (m_Outbound) {
            m_Outbound = false;
            m_Rb.velocity = Vector2.zero;
        }
    }

    private void SetVelocity(Vector2 dir, float speed)
    {
        if (dir.sqrMagnitude < Consts.SQR_MAG_CLOSE_TO_ONE_LOW ||
            dir.sqrMagnitude > Consts.SQR_MAG_CLOSE_TO_ONE_HIGH)
        {
            throw new System.ArgumentException("When setting the velocity " +
                "for a projectile, make sure that the direction has unit " +
                $"length. Dir: {dir} | Mag: {dir.magnitude}");
        }
        if (speed < 0)
        {
            throw new System.ArgumentException("When setting the velocity " +
                "for a projectile, make sure that the speed is larger than " +
                "or equal to zero");
        }

        m_Rb.velocity = dir * speed;
        StartCoroutine(BoomerangReturn());
    }

    private void Update()
    {
        m_PlayerLocation = m_Manager.transform.position;
        spinner.Rotate(0, 0, 500*Time.deltaTime);
        if (!m_Outbound) {
    
            spinner.position = Vector3.MoveTowards(spinner.position, m_PlayerLocation, 0.3f);
        }
    }
    #endregion

    #region Collision Methods


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.GetInstanceID() == m_Manager.gameObject.GetInstanceID())
        {
            if (m_Outbound)
            {
                return;
            }
            else {
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag(Consts.PLAYER_TAG))
        {
            other.GetComponent<PlayerManager>().TakeDamage(m_Damage);
            Vector2 dir = (other.GetComponent<PlayerManager>().transform.position - transform.position).normalized;
            other.GetComponent<PlayerMovement>().ApplyExternalForce(dir * m_Knockback);
        }
        else if (other.CompareTag(Consts.GENERAL_ENEMY_TAG))
        {
            EnemyManager enemyManager = other.GetComponent<EnemyManager>();
            enemyManager.TakeDamage(m_WeaponData, m_Damage, m_Manager.GetID());
            Vector2 dir = (enemyManager.transform.position - transform.position).normalized;
            enemyManager.GetComponent<EnemyMovement>().ApplyExternalForce(dir * m_Knockback);
        }

        if (other.CompareTag(Consts.BUSH_PHYSICS_LAYER))
        {
            m_AudioManager.Play("Scattershot Impact");
            Destroy(other.gameObject);
        }

        if (other.CompareTag(Consts.POT_PHYSICS_LAYER))
        {
            m_AudioManager.Play("potBreak");
            m_AudioManager.Play("Scattershot Impact");
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") || other.CompareTag("Rock") || other.CompareTag("Turret")) {
            m_Outbound = false;
            m_Rb.velocity = Vector2.zero;
        }
    }
    #endregion
}
