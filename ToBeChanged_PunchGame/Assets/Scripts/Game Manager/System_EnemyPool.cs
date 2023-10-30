using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyPool : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    Transform _enemyPoolParent;

    [Space]
    [SerializeField]
    GameObject _normalEnemyPrefab;

    [SerializeField]
    GameObject _eliteEnemyPrefab;

    [SerializeField]
    GameObject _dashEnemyPrefab;

    [SerializeField]
    GameObject _holdEnemyPrefab;

    [Header("Enemy Pool Settings")]
    [SerializeField]
    int _enemyPoolSize;

    List<GameObject> _normalEnemyPool = new List<GameObject>();
    List<GameObject> _eliteEnemyPool = new List<GameObject>();
    List<GameObject> _dashEnemyPool = new List<GameObject>();
    List<GameObject> _holdEnemyPool = new List<GameObject>();

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_SpawnEnemy += ActivateEnemy;
        EventHandler.Event_SpecialActive += (specialActive, specialDuration) =>
        {
            if (!specialActive)
                ClearEnemiesOutsideCamera();
        };
    }

    void OnDisable()
    {
        EventHandler.Event_SpawnEnemy -= ActivateEnemy;
    }

    void Start()
    {
        // Create the pool of particle system instances
        for (int i = 0; i < _enemyPoolSize; i++)
        {
            PrefabInstantiation(_normalEnemyPrefab, _normalEnemyPool);
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

    void PrefabInstantiation(GameObject gameObject, List<GameObject> poolList)
    {
        GameObject particleInstance = Instantiate(gameObject, _enemyPoolParent);
        particleInstance.SetActive(false);
        poolList.Add(particleInstance);
    }

    // Find an inactive particle system in the pool and activate it
    public void ActivateEnemy(Vector3 position)
    {
        int random = UnityEngine.Random.Range(0, 101);

        float chanceValue = GlobalValues.GetEnemiesSpawnChance(EnemyType.Normal);
        if (random < chanceValue)
        {
            foreach (GameObject enemyInstance in _normalEnemyPool)
            {
                if (!enemyInstance.activeInHierarchy)
                {
                    enemyInstance.transform.position = position;
                    enemyInstance.SetActive(true);
                    return;
                }
            }
        }

        chanceValue += GlobalValues.GetEnemiesSpawnChance(EnemyType.Elite);
        if (random < chanceValue)
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

        chanceValue += GlobalValues.GetEnemiesSpawnChance(EnemyType.Hold);
        if (random < chanceValue)
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

        chanceValue += GlobalValues.GetEnemiesSpawnChance(EnemyType.Dash);
        if (random < 100)
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
    }

    private void ClearEnemiesOutsideCamera()
    {
        Camera camera = Camera.main;

        float leftCameraEdge = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float rightCameraEdge = camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;

        RemoveEnemyType(leftCameraEdge, rightCameraEdge, _normalEnemyPool);
        RemoveEnemyType(leftCameraEdge, rightCameraEdge, _dashEnemyPool);
        RemoveEnemyType(leftCameraEdge, rightCameraEdge, _holdEnemyPool);
        RemoveEnemyType(leftCameraEdge, rightCameraEdge, _eliteEnemyPool);

        bool CheckIfInViewport(float enemyPosition)
        {
            if (enemyPosition < leftCameraEdge || enemyPosition > rightCameraEdge)
                return false;
            else
                return true;
        }

        void RemoveEnemyType(
            float leftCameraEdge,
            float rightCameraEdge,
            List<GameObject> enemyPool
        )
        {
            foreach (var enemy in enemyPool)
            {
                if (enemy.activeSelf && !CheckIfInViewport(enemy.transform.position.x))
                    EventHandler.Event_RemoveEnemy?.Invoke(enemy);
            }
        }
    }
}
