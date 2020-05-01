using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much damage do you take")]
    private int m_damage;

    [SerializeField]
    [Tooltip("The gap between damage ticks")]
    private float m_DamageDelay;

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
    private PlayerManager[] players;
    private AudioManager m_AudioManager;
    private bool m_Damaging;
    private float m_CurrDmgTime;
    #endregion

    #region Initialization
    private void Start() {
        players = new PlayerManager[4];
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_spike = false;
        m_AudioManager = GetComponent<AudioManager>();
        m_CurrDmgTime = m_DamageDelay;
    }
    #endregion

    #region Update
    private void Update() {
        if (m_CurrDmgTime > 0) {
            m_CurrDmgTime -= Time.deltaTime;
        }

        if (m_CurrDmgTime <= 0) {
            m_Damaging = true;
        }

        if (m_spike && Time.timeScale != 0) {
            foreach (PlayerManager player in players) {
                if (player != null && m_Damaging) {
                    player.TakeDamage(m_damage);
                    m_Damaging = false;
                    m_CurrDmgTime = m_DamageDelay;
                }
            }
        }
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            players[collision.GetComponent<PlayerManager>().GetID()] = collision.GetComponent<PlayerManager>();
            if (!m_spike) {
                StartCoroutine(ActivateSpikes());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            players[collision.GetComponent<PlayerManager>().GetID()] = null;
        }
    }
    #endregion

    #region Enable
    private IEnumerator ActivateSpikes() {
        spriteRenderer.sprite = spike_half;
        yield return new WaitForSeconds(m_armtime);
        m_AudioManager.Play("spike");
        spriteRenderer.sprite = spike_enabled;
        m_spike = true;
        yield return new WaitForSeconds(m_disarmtime);
        m_AudioManager.Play("spike2");
        spriteRenderer.sprite = spike_disabled;
        m_spike = false;
    }
    #endregion

}
