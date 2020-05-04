using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gismo : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
     Gizmos.color = Color.red;
     Gizmos.DrawWireSphere(transform.position, 3.6f);
    }
}
