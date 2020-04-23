using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobAttack : EnemyAttack {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(Consts.PLAYER_TAG)) {
            collision.GetComponent<PlayerManager>().TakeDamage(m_Manager.GetEnemyData().Damage);
            collision.GetComponent<PlayerMovement>().ApplyExternalForce(m_Manager.GetDir() * 100);
        }
    }
}
