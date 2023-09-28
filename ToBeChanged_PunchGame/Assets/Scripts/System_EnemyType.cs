using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyType : MonoBehaviour
{
    [SerializeField]
    EnemyType enemyType;

    void Awake()
    {
        // SelectEnemyType();
    }

    void SelectEnemyType()
    {
        int value = Random.Range(0, 100);

        if (value < 80)
            enemyType = EnemyType.easy;
        else if (value < 95)

            enemyType = EnemyType.medium;
        else if (value < 100)
            enemyType = EnemyType.hard;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }
}
