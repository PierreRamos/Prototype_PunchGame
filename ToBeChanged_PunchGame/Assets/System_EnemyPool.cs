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

    [Header("Enemy Pool Settings")]
    [Space]
    [SerializeField]
    int _enemyPoolSize;

    List<GameObject> _easyEnemyPool = new List<GameObject>();
    List<GameObject> _mediumEnemyPool = new List<GameObject>();
    List<GameObject> _hardEnemyPool = new List<GameObject>();

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
        float random = UnityEngine.Random.Range(0f, 10f);

        if (random < 6.5f)
        {
            foreach (GameObject particleInstance in _easyEnemyPool)
            {
                if (!particleInstance.activeInHierarchy)
                {
                    particleInstance.transform.position = position;
                    particleInstance.SetActive(true);
                    return;
                }
            }
        }
        else if (random < 9f)
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
        else if (random < 10f)
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
    }
}
