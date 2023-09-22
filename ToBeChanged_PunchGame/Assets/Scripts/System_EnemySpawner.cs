using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemySpawner : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [SerializeField]
    Transform _leftEnemySpawn;

    [SerializeField]
    Transform _rightEnemySpawn;

    [Header("Spawner Settings")]
    [SerializeField]
    float _baseSpawnInterval;

    [SerializeField]
    float _spawnIntervalRange;

    bool _spawnerOn = true;

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;
    }

    void Start()
    {
        StartCoroutine(SpawnEnemyTimer());
    }

    IEnumerator SpawnEnemyTimer()
    {
        while (_spawnerOn)
        {
            SpawnEnemy();

            var spawnInterval = _baseSpawnInterval - GlobalValues.GetEnemySpawnModifier();

            yield return new WaitForSeconds(
                Random.Range(
                    spawnInterval - _spawnIntervalRange,
                    spawnInterval + _spawnIntervalRange
                )
            );
        }
    }

    void SpawnEnemy()
    {
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            EventHandler.Event_SpawnEnemy?.Invoke(_leftEnemySpawn.position);
        }
        else
        {
            EventHandler.Event_SpawnEnemy?.Invoke(_rightEnemySpawn.position);
        }
    }
}
