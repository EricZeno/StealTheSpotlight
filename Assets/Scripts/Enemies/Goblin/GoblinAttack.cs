using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAttack : EnemyAttack {
    #region Attack Colliders
    private void OnTriggerEnter2D(Collider2D collision) {
        GameObject target = collision.gameObject;
        if (target.CompareTag(Consts.PLAYER_TAG)) {
            target.GetComponent<PlayerManager>().TakeDamage(m_Manager.GetEnemyData().Damage);
            target.GetComponent<PlayerMovement>().ApplyExternalForce(m_Manager.GetDir() * 200);
        }
    }
    #endregion
}
