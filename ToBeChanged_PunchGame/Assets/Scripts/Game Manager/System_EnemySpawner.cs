using System;
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

    [SerializeField]
    float _minSpawnInterval;

    [Header("Enemy Spawn Chance Settings")]
    [SerializeField]
    float _normalEnemySpawnRatio;

    [SerializeField]
    float _eliteEnemySpawnRatio;

    [SerializeField]
    float _dashEnemySpawnRatio;

    [SerializeField]
    float _holdEnemySpawnRatio;

    [Header("Debug Options")]
    [SerializeField]
    bool _spawnerOn = true;

    Coroutine _spawnEnemyTimer;
    Dictionary<EnemyType, int> _enemiesSpawnChance = new Dictionary<EnemyType, int>();

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;
    }

    void Start()
    {
        CalculateEnemySpawnChance();
        _spawnEnemyTimer = StartCoroutine(SpawnEnemyTimer());
    }

    void CalculateEnemySpawnChance()
    {
        var sumOfChances =
            _normalEnemySpawnRatio
            + _eliteEnemySpawnRatio
            + _dashEnemySpawnRatio
            + _holdEnemySpawnRatio;

        _enemiesSpawnChance.Add(
            EnemyType.Normal,
            (int)(_normalEnemySpawnRatio / sumOfChances * 100)
        );
        _enemiesSpawnChance.Add(EnemyType.Elite, (int)(_eliteEnemySpawnRatio / sumOfChances * 100));
        _enemiesSpawnChance.Add(EnemyType.Dash, (int)(_dashEnemySpawnRatio / sumOfChances * 100));
        _enemiesSpawnChance.Add(EnemyType.Hold, (int)(_holdEnemySpawnRatio / sumOfChances * 100));

        GlobalValues.SetEnemiesSpawnChance(_enemiesSpawnChance);
    }

    IEnumerator SpawnEnemyTimer()
    {
        while (_spawnerOn && GlobalValues.GetGameState() != GameState.Paused)
        {
            if (GlobalValues.GetGameState() == GameState.GameOver)
                StopCoroutine(_spawnEnemyTimer);

            SpawnEnemy();

            var spawnInterval = _baseSpawnInterval - GlobalValues.GetEnemySpawnModifier();
            var spawnSeconds = UnityEngine.Random.Range(
                spawnInterval - _spawnIntervalRange,
                spawnInterval + _spawnIntervalRange
            );

            if (spawnSeconds < _minSpawnInterval)
                spawnSeconds = _minSpawnInterval;

            yield return new WaitForSeconds(spawnSeconds);

            //Stop spawning while in certain scenarios
            while (
                GlobalValues.GetGameState() == GameState.SoloBattle
                || GlobalValues.GetGameState() == GameState.HoldBattle
            )
            {
                yield return null;
            }
        }
    }

    void SpawnEnemy()
    {
        int random = UnityEngine.Random.Range(0, 2);

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
