using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyPool : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    Transform _enemyPoolParent;

    [SerializeField]
    GameObject _easyEnemyPrefab;

    [SerializeField]
    GameObject _mediumEnemyPrefab;

    [SerializeField]
    GameObject _hardEnemyPrefab;

    [SerializeField]
    GameObject _eliteEnemyPrefab;

    [SerializeField]
    GameObject _dashEnemyPrefab;

    [SerializeField]
    GameObject _holdEnemyPrefab;

    [Header("Enemy Pool Settings")]
    [Space]
    [SerializeField]
    int _enemyPoolSize;

    List<GameObject> _easyEnemyPool = new List<GameObject>();
    List<GameObject> _mediumEnemyPool = new List<GameObject>();
    List<GameObject> _hardEnemyPool = new List<GameObject>();
    List<GameObject> _eliteEnemyPool = new List<GameObject>();
    List<GameObject> _dashEnemyPool = new List<GameObject>();
    List<GameObject> _holdEnemyPool = new List<GameObject>();

    private void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_SpawnEnemy += ActivateEnemy;

        // Create the pool of particle system instances
        for (int i = 0; i < _enemyPoolSize; i++)
        {
            PrefabInstantiation(_easyEnemyPrefab, _easyEnemyPool);
        }
        for (int i = 0; i < _enemyPoolSize; i++)
        {
            PrefabInstantiation(_mediumEnemyPrefab, _mediumEnemyPool);
        }
        for (int i = 0; i < _enemyPoolSize; i++)
        {
            PrefabInstantiation(_hardEnemyPrefab, _hardEnemyPool);
        }
        for (int i = 0; i < _enemyPoolSize; i++)
        {
            PrefabInstantiation(_eliteEnemyPrefab, _eliteEnemyPool);
        }
        for (int i = 0; i < _enemyPoolSize; i++)
        {
            PrefabInstantiation(_dashEnemyPrefab, _dashEnemyPool);
        }
        for (int i = 0; i < _enemyPoolSize; i++)
        {
            PrefabInstantiation(_holdEnemyPrefab, _holdEnemyPool);
        }
    }

    private void OnDisable()
    {
        EventHandler.Event_SpawnEnemy -= ActivateEnemy;
    }

    private void PrefabInstantiation(GameObject gameObject, List<GameObject> poolList)
    {
        GameObject particleInstance = Instantiate(gameObject, _enemyPoolParent);
        particleInstance.SetActive(false);
        poolList.Add(particleInstance);
    }

    // Find an inactive particle system in the pool and activate it
    public void ActivateEnemy(Vector3 position)
    {
        int random = UnityEngine.Random.Range(0, 101);

        //25%
        if (random < 25)
        {
            foreach (GameObject enemyInstance in _easyEnemyPool)
            {
                if (!enemyInstance.activeInHierarchy)
                {
                    enemyInstance.transform.position = position;
                    enemyInstance.SetActive(true);
                    return;
                }
            }
        }
        //25%
        else if (random < 50)
        {
            foreach (GameObject particleInstance in _mediumEnemyPool)
            {
                if (!particleInstance.activeInHierarchy)
                {
                    particleInstance.transform.position = position;
                    particleInstance.SetActive(true);
                    return;
                }
            }
        }
        //25%
        else if (random < 75)
        {
            foreach (GameObject particleInstance in _hardEnemyPool)
            {
                if (!particleInstance.activeInHierarchy)
                {
                    particleInstance.transform.position = position;
                    particleInstance.SetActive(true);
                    return;
                }
            }
        }
        //10%
        else if (random < 85)
        {
            foreach (GameObject particleInstance in _holdEnemyPool)
            {
                if (!particleInstance.activeInHierarchy)
                {
                    particleInstance.transform.position = position;
                    particleInstance.SetActive(true);
                    return;
                }
            }
        }
        //10%
        else if (random < 95)
        {
            foreach (GameObject particleInstance in _eliteEnemyPool)
            {
                if (!particleInstance.activeInHierarchy)
                {
                    particleInstance.transform.position = position;
                    particleInstance.SetActive(true);
                    return;
                }
            }
        }
        //5%
        else if (random < 100)
        {
            foreach (GameObject particleInstance in _dashEnemyPool)
            {
                if (!particleInstance.activeInHierarchy)
                {
                    particleInstance.transform.position = position;
                    particleInstance.SetActive(true);
                    return;
                }
            }
        }

        //Debug only
        // foreach (GameObject particleInstance in _dashEnemyPool)
        // {
        //     if (!particleInstance.activeInHierarchy)
        //     {
        //         particleInstance.transform.position = position;
        //         particleInstance.SetActive(true);
        //         return;
        //     }
        // }
    }
}
