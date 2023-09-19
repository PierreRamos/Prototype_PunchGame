using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject _enemyObject;

    [SerializeField]
    Transform _leftEnemySpawn;

    [SerializeField]
    Transform _rightEnemySpawn;

    void Start()
    {
        System_EventHandler.Instance.Event_SpawnEnemy += SpawnEnemy;
    }

    void OnDisable()
    {
        System_EventHandler.Instance.Event_SpawnEnemy -= SpawnEnemy;
    }

    public void SpawnEnemy()
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
}
