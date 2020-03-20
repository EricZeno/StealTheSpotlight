using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much damage do you take")]
    private int m_damage;

    [SerializeField]
    [Tooltip("How much time between burst of shots")]
    private float m_interval;

    [SerializeField]
    [Tooltip("How much time to shoot X bullets")]
    private float m_timeframe;

    [SerializeField]
    [Tooltip("How much bullets per burst")]
    private int m_bullets;

    [SerializeField]
    [Tooltip("Degree to shoot at")]
    private int m_degree;

    [SerializeField]
    [Tooltip("The bullet prefab")]
    private GameObject m_bullet;

    [SerializeField]
    [Tooltip("How fast the bullets will travel")]
    private float m_shotspeed;
    #endregion

    #region Private Variable
    private float m_timer;
    #endregion

    #region Initialization
    private void Start() {
        m_timer = m_interval;
    }
    #endregion

    #region Update
    void Update() {
        if (m_timer > 0) {
            m_timer -= Time.deltaTime;
        }
        else {
            m_timer = m_interval;
            StartCoroutine(Shoot());
        }
    }
    #endregion

    #region Shoot
    private IEnumerator Shoot() {
        float timeBetweenShots = m_bullets / m_timeframe;
        Vector2 firingVector = Vector2.up;
        //Fix direction
        firingVector = Quaternion.Euler(0, 0, m_degree) * firingVector;
        firingVector.Normalize();

        for (int i = 0; i < m_bullets; i++) {
            GameObject bullet = Instantiate(m_bullet, transform);
            bullet.GetComponent<Projectile>().Setup(m_damage, firingVector, m_shotspeed);

            yield return new WaitForSeconds(timeBetweenShots);
        }
        yield return null;
    }
    #endregion
}
