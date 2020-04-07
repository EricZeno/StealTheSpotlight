using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much damage do you take")]
    private int m_damage;

    [SerializeField]
    [Tooltip("How much spike takes to arm")]
    private float m_armtime;

    [SerializeField]
    [Tooltip("How much spike takes to arm")]
    private float m_disarmtime;

    [SerializeField]
    [Tooltip("Disabled spikes")]
    private Sprite spike_disabled;

    [SerializeField]
    [Tooltip("Half spikes")]
    private Sprite spike_half;

    [SerializeField]
    [Tooltip("Enabled spikes")]
    private Sprite spike_enabled;
    #endregion

    #region Private Variables
    private SpriteRenderer spriteRenderer;
    private bool m_spike;
    #endregion

    #region Initialization
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_spike = false;
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG) && !m_spike) {
            StartCoroutine(ActivateSpikes());
        }
        else if (collision.CompareTag(Consts.PLAYER_TAG) && m_spike) {
            collision.GetComponent<PlayerManager>().TakeDamage(5);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG) && m_spike) {
            collision.GetComponent<PlayerManager>().TakeDamage(5);
        }
    }
    #endregion

    #region Enable
    private IEnumerator ActivateSpikes() {
        spriteRenderer.sprite = spike_half;
        yield return new WaitForSeconds(m_armtime);
        spriteRenderer.sprite = spike_enabled;
        m_spike = true;
        yield return new WaitForSeconds(m_disarmtime);
        spriteRenderer.sprite = spike_disabled;
        m_spike = false;
    }
    #endregion

}
