using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemySpawner : MonoBehaviour
{
    [Header("Initialization")]
    [SerializeField]
    GameObject _enemyObject;

    [SerializeField]
    Transform _leftEnemySpawn;

    [SerializeField]
    Transform _rightEnemySpawn;

    [Header("Spawner Settings")]
    [SerializeField]
    float _spawnInterval;

    [SerializeField]
    float _spawnIntervalRange;

    bool _spawnerOn = true;

    void Start()
    {
        System_EventHandler.Instance.Event_SpawnEnemy += SpawnEnemy;

        StartCoroutine(SpawnEnemyTimer());
    }

    void OnDisable()
    {
        System_EventHandler.Instance.Event_SpawnEnemy -= SpawnEnemy;
    }

    IEnumerator SpawnEnemyTimer()
    {
        while (_spawnerOn)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(
                Random.Range(
                    _spawnInterval - _spawnIntervalRange,
                    _spawnInterval + _spawnIntervalRange
                )
            );
        }
    }

    void SpawnEnemy()
    {
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            Instantiate(_enemyObject, _leftEnemySpawn.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_enemyObject, _rightEnemySpawn.position, Quaternion.identity);
        }
    }

    void CancelSpawnEnemy()
    {
        CancelInvoke("SpawnEnemy");
    }
}
