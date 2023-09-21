using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EventHandler : MonoBehaviour
{
    public static System_EventHandler Instance;

    //Game events
    public Action<Vector3> Event_SpawnEnemy;

    public Action Event_DefeatedEnemy;

    //Player controller events
    public Action Event_AttackLeft;
    public Action Event_AttackRight;

    //Player Status events
    public Action Event_TriggerStun;

    //Attack events
    public Action Event_SlowTime;
    public Action Event_StopSlowTime;
    public Action<GameObject> Event_EnemyHit;
    public Action<Vector3> Event_MoveToEnemy;

    //Value change events

    public Action<int> Event_PlayerHealthValueChange;
    public Action<int> Event_EnemyDefeatedValueChange;

    public Action<int> Event_DifficultyValueChange;
    public Action<GameObject, int> Event_EnemyHealthValueChange;

    //Effect events
    public Action<Vector3> Event_HitEffect;
    public Action<Vector3> Event_ExclamationEffect;

    //Hit indicator events
    public Action<int> Event_PlayerHit;
    public Action<bool> Event_HasEnemyLeft;
    public Action<bool> Event_HasEnemyRight;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
