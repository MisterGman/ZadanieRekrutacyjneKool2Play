using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] 
    private GameObject enemyPrefab;

    [field: SerializeField, Tooltip("Where to spawn enemy")] 
    private Transform spawnPoint;

    [field: SerializeField, Tooltip("How fast enemy should spawn")]
    private float delayBetweenSpawning;

    private void Start()
    {
        StartCoroutine(SpawningEnemies());
    }

    /// <summary>
    /// Get from pool and set pos and rot from spawnPoint
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawningEnemies()
    {
        while (true)
        {
            var currEnemy = ObjectPooling.Instance.GetObjectFromPool(enemyPrefab);

            currEnemy.transform.position = spawnPoint.position;
            currEnemy.transform.rotation = spawnPoint.rotation;

            yield return new WaitForSeconds(delayBetweenSpawning);
        }
    }
}
