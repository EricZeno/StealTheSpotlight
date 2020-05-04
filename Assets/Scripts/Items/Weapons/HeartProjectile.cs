using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class HeartProjectile : MonoBehaviour {
    #region Private Variables
    private int m_Damage;
    private float m_Knockback;

    [SerializeField]
    private GameObject uwuLarge;
    [SerializeField]
    private GameObject uwuSmall;

    #endregion

    #region Cached Components
    private Rigidbody2D m_Rb;
    private SpriteRenderer m_Renderer;
    private BoxCollider2D m_Collider;
    private PlayerManager m_Manager;
    private BaseWeaponItem m_WeaponData;
    private float m_Scale;
    private float m_Radius;
    private AudioManager m_AudioManager;
    private bool big;
    #endregion

    #region Initialization
    private void Awake() {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_AudioManager = GetComponent<AudioManager>();
    }
    #endregion

    #region Projectile Setup Methods

    public void Setup(int damage, Vector2 dir, float speed, PlayerManager player, float knockback, BaseWeaponItem data, float scale, float radius, bool big) {
        SetDamage(damage);
        SetVelocity(dir, speed);
        m_Manager = player;
        m_Knockback = knockback;
        m_WeaponData = data;
        m_Scale = scale;
        m_Radius = radius;

    }

    private void SetDamage(int damage) {
        if (damage < 0) {
            throw new System.ArgumentException("When setting the damage for " +
                "a projectile, make sure that it is larger than or equal to" +
                "zero");
        }
        m_Damage = damage;
    }

    private void SetVelocity(Vector2 dir, float speed) {
        if (dir.sqrMagnitude < Consts.SQR_MAG_CLOSE_TO_ONE_LOW ||
            dir.sqrMagnitude > Consts.SQR_MAG_CLOSE_TO_ONE_HIGH) {
            throw new System.ArgumentException("When setting the velocity " +
                "for a projectile, make sure that the direction has unit " +
                $"length. Dir: {dir} | Mag: {dir.magnitude}");
        }
        if (speed < 0) {
            throw new System.ArgumentException("When setting the velocity " +
                "for a projectile, make sure that the speed is larger than " +
                "or equal to zero");
        }

        m_Rb.velocity = dir * speed;
    }
    #endregion

    #region Collision Methods


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetInstanceID() == m_Manager.gameObject.GetInstanceID()) {
            return;
        }
        else if (other.CompareTag(Consts.PLAYER_TAG)) {
            m_AudioManager.Play("Wand Hit");
            Explode(other);
        }
        else if (other.CompareTag(Consts.GENERAL_ENEMY_TAG)) {
            m_AudioManager.Play("Wand Hit");
            Explode(other);
        }

        if (other.CompareTag(Consts.BUSH_PHYSICS_LAYER)) {
            m_AudioManager.Play("Wand Hit");
            Explode(other);
        }

        if (other.CompareTag(Consts.POT_PHYSICS_LAYER)) {
            m_AudioManager.Play("Wand Hit");
            Explode(other);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") || other.CompareTag("Rock") || other.CompareTag("Turret")) {
            m_AudioManager.Play("Wand Hit");
            Destroy(gameObject);
        }
    }

    private void Explode(Collider2D other) {
        Vector3 position = other.transform.position;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, m_Radius * m_Scale);
        if (big)
        {
            GameObject particles = Instantiate(uwuLarge, transform.position, Quaternion.identity);
        }
        else {
            GameObject particles = Instantiate(uwuLarge, transform.position, Quaternion.identity);
        }
        

        foreach (Collider2D colls in hitColliders) {

            if (colls.CompareTag(Consts.GENERAL_ENEMY_TAG)) {

                EnemyManager enemyManager = colls.GetComponent<EnemyManager>();
                enemyManager.TakeDamage(m_WeaponData, (int)(m_Damage * (m_Scale * 2)), m_Manager.GetID());

            }
            if (colls.CompareTag(Consts.PLAYER_TAG)) {
                colls.GetComponent<PlayerManager>().TakeDamage((int)(m_Damage * (m_Scale * 2)));
            }

            if (colls.CompareTag(Consts.BUSH_PHYSICS_LAYER)) {
                Destroy(colls.gameObject);
            }

            if (colls.CompareTag(Consts.POT_PHYSICS_LAYER)) {
                m_AudioManager.Play("potBreak");
                m_AudioManager.Play("Wand Hit");
                Destroy(colls.gameObject);
            }
        }
        Destroy(gameObject);
    }
    #endregion
}
