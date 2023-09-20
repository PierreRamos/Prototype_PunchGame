using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyHealth : MonoBehaviour
{
    [SerializeField]
    int _easyEnemyHealth;

    [SerializeField]
    int _mediumEnemyHealth;

    [SerializeField]
    int _hardEnemyHealth;

    int _enemyHealth;

    System_EventHandler EventHandler;

    void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_EnemyHit += EnemyHitHealthCheck;

        //Sets enemy health value according to enemy type
        var enemyType = gameObject.GetComponent<System_EnemyType>().GetEnemyType();

        if (enemyType == EnemyType.easy)
            _enemyHealth = _easyEnemyHealth;
        else if (enemyType == EnemyType.medium)
            _enemyHealth = _mediumEnemyHealth;
        else if (enemyType == EnemyType.hard)
            _enemyHealth = _hardEnemyHealth;

        EventHandler.Event_EnemyHealthValueChange?.Invoke(gameObject, _enemyHealth);
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHit -= EnemyHitHealthCheck;
    }

    public int GetEnemyHealth()
    {
        return _enemyHealth;
    }

    //Checks if this is the enemy hit and removes health
    void EnemyHitHealthCheck(GameObject gameObject)
    {
        if (this.gameObject == gameObject)
        {
            _enemyHealth--;
            if (_enemyHealth <= 0)
            {
                //temporary
                Destroy(gameObject);
                return;
            }
            EventHandler.Event_EnemyHealthValueChange?.Invoke(gameObject, _enemyHealth);
        }
    }
}
