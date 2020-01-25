using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDefenseTarget : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            GetComponentInParent<ZoneDefense>().FailObjective();
        }
    }
}
