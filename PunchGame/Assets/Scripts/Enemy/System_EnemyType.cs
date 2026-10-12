using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyType : MonoBehaviour
{
    [SerializeField]
    EnemyType enemyType;

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }
}
