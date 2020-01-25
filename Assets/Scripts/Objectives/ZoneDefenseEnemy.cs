using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDefenseEnemy : MonoBehaviour
{
    private GameObject m_ZoneDefenseObj;

    private Vector3 m_Target;
    private float m_MoveSpeed = .05f;

    private void Update() {
        transform.position = Vector2.MoveTowards(transform.position, m_Target, m_MoveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(Consts.PLAYER_TAG)) {
            m_ZoneDefenseObj.GetComponent<ZoneDefense>().KillEnemy();
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject obj) {
        m_ZoneDefenseObj = obj;

        m_Target = m_ZoneDefenseObj.GetComponentInChildren<ZoneDefenseTarget>().transform.position;
    }
}
