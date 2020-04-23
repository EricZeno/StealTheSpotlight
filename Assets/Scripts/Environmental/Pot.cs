using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    private Sprite[] m_BrokenShards;

    [SerializeField]
    [Tooltip("The enemy that can pop out of this pop when it's destroyed")]
    private GameObject m_Blob;

    [SerializeField]
    [Tooltip("The chance that an enemy will pop out of a pot")]
    [Range(0, 1)]
    private float m_MobChance;
    #endregion

    #region Pot
    public void Break() {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = m_BrokenShards[Random.Range(0, m_BrokenShards.Length)];

        if (Random.value < m_MobChance) {
            GameObject blob = Instantiate(m_Blob, transform.position, Quaternion.identity, transform.parent);
            EnemyManager blobManager = blob.GetComponent<EnemyManager>();
            Room room = GetComponentInParent<Room>();
            blobManager.SetRoom(room);
            room.AddEnemy(blobManager);
            blob.GetComponent<EnemyMovement>().SetSpawn(transform.position);
        }
    }
    #endregion
}
