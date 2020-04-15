using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour {
    #region Private Variables
    private int m_Damage;
    #endregion

    #region Cached Components
    private Rigidbody2D m_Rb;
    private SpriteRenderer m_Renderer;
    private BoxCollider2D m_Collider;
    private PlayerManager m_manager;
    #endregion

    #region Initialization
    private void Awake() {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Renderer = GetComponent<SpriteRenderer>();
        m_Collider = GetComponent<BoxCollider2D>();
    }
    #endregion

    #region Projectile Setup Methods
    public void Setup(int damage, Vector2 dir, float speed, Sprite sprite, Vector2 colliderSize) {
        SetDamage(damage);
        SetVelocity(dir, speed);
        SetSpriteAndCollider(sprite, colliderSize);
    }

    public void Setup(int damage, Vector2 dir, float speed) {
        SetDamage(damage);
        SetVelocity(dir, speed);
    }

    public void Setup(int damage, Vector2 dir, float speed, PlayerManager player) {
        SetDamage(damage);
        SetVelocity(dir, speed);
        m_manager = player;
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

    private void SetSpriteAndCollider(Sprite sprite, Vector2 colliderSize) {
        m_Renderer.sprite = sprite;
        m_Collider.size = colliderSize;
    }
    #endregion

    #region Collision Methods
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(Consts.PLAYER_TAG)) {
            other.GetComponent<PlayerManager>().TakeDamage(m_Damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            Destroy(gameObject);
        }
    }
    #endregion
}
