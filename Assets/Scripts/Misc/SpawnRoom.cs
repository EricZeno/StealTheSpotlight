using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnRoom : MonoBehaviour {
    #region Editor Variables
    [SerializeField]
    [Tooltip("The spawn locations in this room")]
    private Vector3[] m_SpawnLocations;
    #endregion

    #region Accessors
    public Vector3[] GetSpawnLocations() {
        return m_SpawnLocations;
    }
    #endregion

    #region Error Checking
    private void Start() {
        if (m_SpawnLocations.Length != Consts.MAX_NUM_PLAYERS) {
            throw new System.MissingFieldException("Spawn points not intialized");
        }
    }
    #endregion
}
