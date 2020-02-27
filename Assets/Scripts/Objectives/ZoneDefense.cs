using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDefense : MonoBehaviour {
    #region Variables
    #region Editor Variables
    [SerializeField]
    [Tooltip("Target that the player needs to protect")]
    private GameObject m_Target;

    [SerializeField]
    [Tooltip("Enemies that will be spawned")]
    private GameObject[] m_Enemies;

    [SerializeField]
    [Tooltip("Where the enemies will spawn")]
    private GameObject[] m_SpawnLocations;

    [SerializeField]
    [Tooltip("Number of enemies to spawn per wave")]
    private int[] m_EnemiesPerWave;

    [SerializeField]
    [Tooltip("Enemy spawn during wave will be delayed by a random number from 0 to this number")]
    private float m_SpawnDelay;

    [SerializeField]
    [Tooltip("Delay between wave spawns")]
    private float m_SpawnWaveDelay;
    #endregion

    #region Private Variables
    private int m_Countdown = 3;

    private HashSet<GameObject> m_SpawnedEnemies;
    private int m_EnemiesKilled;

    private bool m_FinishedSpawn;
    #endregion

    #region Cached Variables
    private ObjectiveManager m_ObjectiveManager;
    #endregion
    #endregion

    #region Initialization and Countdown
    private void Start() {
        m_ObjectiveManager = GetComponentInParent<ObjectiveManager>();

        m_SpawnedEnemies = new HashSet<GameObject>();

        Instantiate(m_Target, transform);
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown() {
        while (m_Countdown > 0) {
            Debug.Log(m_Countdown);
            m_Countdown--;

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("START");
        StartObjective();
    }
    #endregion

    #region Update
    private void Update() {
        if (IsObjectiveComplete()) {
            CompleteObjective();
        }
    }
    #endregion

    #region Objective Methods
    private void StartObjective() {
        StartCoroutine(SpawnWaves());
    }

    public void FailObjective() {
        foreach (GameObject enemy in m_SpawnedEnemies) {
            Destroy(enemy);
        }

        m_ObjectiveManager.FailObjective();

        Destroy(gameObject);
    }

    public void CompleteObjective() {
        m_ObjectiveManager.CompleteObjective();

        Destroy(gameObject);
    }

    private bool IsObjectiveComplete() {
        return m_FinishedSpawn && m_EnemiesKilled == m_SpawnedEnemies.Count;
    }
    #endregion

    #region Spawn Methods
    private IEnumerator SpawnWaves() {
        for (int i = 0; i < m_EnemiesPerWave.Length; i++) {
            yield return SpawnEnemies(i);

            yield return new WaitForSeconds(m_SpawnWaveDelay);
        }

        m_FinishedSpawn = true;
    }

    private IEnumerator SpawnEnemies(int wave) {
        for (int i = 0; i < m_EnemiesPerWave[wave]; i++) {
            int enemyIndex = Random.Range(0, m_Enemies.Length);
            int locationIndex = Random.Range(0, m_SpawnLocations.Length);

            GameObject spawnedEnemy = Instantiate(m_Enemies[enemyIndex], m_SpawnLocations[locationIndex].transform.position, Quaternion.identity);

            // Zone defense will be broken as an objective until this is uncommented.
            // We should have a specific enemy type for the objective though, which gives us more control over its behavior
            //spawnedEnemy.GetComponent<EnemyMovement>().Target = gameObject;
            spawnedEnemy.transform.SetParent(transform);

            m_SpawnedEnemies.Add(spawnedEnemy);

            yield return new WaitForSeconds(Random.Range(0, m_SpawnDelay));
        }
    }

    public void KillEnemy() {
        m_EnemiesKilled++;
    }
    #endregion
}
