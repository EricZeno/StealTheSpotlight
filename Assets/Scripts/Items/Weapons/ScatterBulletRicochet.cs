using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterBulletRicochet : MonoBehaviour {
    #region Variables
    private Vector2 bulletVelocity;
    private int m_Bounces;
    private AudioManager m_AudioManager;
    #endregion

    #region Initialization
    private void Start() {
        bulletVelocity = GetComponentInParent<Rigidbody2D>().velocity;
        m_Bounces = GetComponentInParent<ScatterBullet>().Bounces;
        m_AudioManager = GetComponent<AudioManager>();
    }
    #endregion

    #region Bouncy Utils
    private void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") || other.gameObject.CompareTag("Turret") || other.gameObject.CompareTag("Rock")) {
            if (m_Bounces == 0) {
                m_AudioManager.Play("Scattershot Bounce");
                BeginDestroy();
                return;
            }
            Vector2 newDirection = Vector2.Reflect(bulletVelocity, collision.GetContact(0).normal);
            bulletVelocity = newDirection;
            GetComponentInParent<Rigidbody2D>().velocity = bulletVelocity;
            m_AudioManager.Play("Scattershot Bounce");
            m_Bounces--;
        }
    }

    private void BeginDestroy() {
        Destroy(transform.parent.gameObject);
    }
    #endregion
}
