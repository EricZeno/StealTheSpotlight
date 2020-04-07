using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponCollision : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == transform.parent.gameObject.layer) {
            return;
        }

        if (collision.CompareTag(Consts.PLAYER_TAG) || collision.CompareTag(Consts.GENERAL_ENEMY_TAG)) {
            GetComponentInParent<PlayerWeapon>().HitEnemy(collision.gameObject);
        }

        if (collision.CompareTag(Consts.BUSH_PHYSICS_LAYER)) {
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag(Consts.POT_PHYSICS_LAYER)) {
            Destroy(collision.gameObject);
        }
    }
}
